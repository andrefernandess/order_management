using MediatR;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Commands.ConfirmOrder;

public record ConfirmOrderCommand(
    Guid OrderId,
    PaymentMethod PaymentMethod
) : IRequest<OrderDetailDto>;
