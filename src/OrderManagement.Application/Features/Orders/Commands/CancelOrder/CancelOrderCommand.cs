using MediatR;

namespace OrderManagement.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(
    Guid OrderId,
    string Reason
) : IRequest<bool>;
