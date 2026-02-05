using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrders;

public record GetOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CustomerId = null,
    OrderStatus? Status = null
) : IRequest<PaginatedList<OrderDto>>;
