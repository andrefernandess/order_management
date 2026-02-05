using AutoMapper;
using MediatR;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<OrderDetailDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Valida se o cliente existe
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw new NotFoundException("Customer", request.CustomerId);

        // Cria o endereço de entrega
        var shippingAddress = Address.Create(
            request.ShippingAddress.Street,
            request.ShippingAddress.Number,
            request.ShippingAddress.Complement,
            request.ShippingAddress.Neighborhood,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.ZipCode
        );

        // Cria o pedido
        var order = Order.Create(request.CustomerId, shippingAddress);

        // Adiciona os itens
        foreach (var itemCmd in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemCmd.ProductId, cancellationToken)
                ?? throw new NotFoundException("Product", itemCmd.ProductId);

            if (!product.IsActive)
                throw new DomainException($"Product '{product.Name}' is not available.");

            if (product.StockQuantity < itemCmd.Quantity)
                throw new DomainException($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}");

            order.AddItem(
                product.Id,
                product.Name,
                itemCmd.Quantity,
                product.GetPrice()
            );

            // Diminui o estoque
            product.DecreaseStock(itemCmd.Quantity);
            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        if (request.Notes is not null)
        {
            order.SetNotes(request.Notes);
        }

        // Persiste o pedido
        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Carrega o pedido com os dados completos para retornar
        var createdOrder = await _unitOfWork.Orders.GetByIdWithItemsAsync(order.Id, cancellationToken);

        return _mapper.Map<OrderDetailDto>(createdOrder);
    }
}
