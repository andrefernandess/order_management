using MediatR;
using OrderManagement.Application.DTOs.Orders;

namespace OrderManagement.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    AddressCommand ShippingAddress,
    List<OrderItemCommand> Items,
    string? Notes
) : IRequest<OrderDetailDto>;

public record AddressCommand(
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode
);

public record OrderItemCommand(
    string ProductId,
    int Quantity
);
