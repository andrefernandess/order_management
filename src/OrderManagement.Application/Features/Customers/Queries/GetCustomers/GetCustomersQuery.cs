using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Customers;

namespace OrderManagement.Application.Features.Customers.Queries.GetCustomers;

public record GetCustomersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsActive = null
) : IRequest<PaginatedList<CustomerDto>>;
