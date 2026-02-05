using MediatR;
using OrderManagement.Application.DTOs.Orders;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDetailDto?>;
