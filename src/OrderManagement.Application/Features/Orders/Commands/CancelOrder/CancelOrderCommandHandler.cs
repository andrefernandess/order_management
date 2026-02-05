using MediatR;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Domain.Interfaces.Services;

namespace OrderManagement.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IEventPublisher _eventPublisher;

    public CancelOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IEventPublisher eventPublisher)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order", request.OrderId);

        // Cancela o pedido (regra de negócio na entidade)
        order.Cancel(request.Reason);

        // Devolve o estoque dos produtos
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is not null)
            {
                product.IncreaseStock(item.Quantity);
                await _productRepository.UpdateAsync(product, cancellationToken);
            }
        }

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publica eventos
        await _eventPublisher.PublishManyAsync(order.DomainEvents, cancellationToken);
        order.ClearDomainEvents();

        return true;
    }
}
