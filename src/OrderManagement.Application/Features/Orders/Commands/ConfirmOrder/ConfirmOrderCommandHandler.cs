using AutoMapper;
using MediatR;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Domain.Interfaces.Services;

namespace OrderManagement.Application.Features.Orders.Commands.ConfirmOrder;

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, OrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;

    public ConfirmOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
    }

    public async Task<OrderDetailDto> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order", request.OrderId);

        // Confirma o pedido (regra de negócio na entidade)
        order.Confirm(request.PaymentMethod);

        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publica os eventos de domínio
        await _eventPublisher.PublishManyAsync(order.DomainEvents, cancellationToken);
        order.ClearDomainEvents();

        return _mapper.Map<OrderDetailDto>(order);
    }
}
