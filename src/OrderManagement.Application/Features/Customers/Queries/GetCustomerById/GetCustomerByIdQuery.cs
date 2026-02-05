using MediatR;
using OrderManagement.Application.DTOs.Customers;

namespace OrderManagement.Application.Features.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDetailDto?>;
