using MediatR;
using OrderManagement.Application.DTOs.Customers;

namespace OrderManagement.Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(
    string Name,
    string Email,
    string? Phone
) : IRequest<CustomerDto>;
