using MediatR;
using OrderManagement.Application.DTOs.Customers;

namespace OrderManagement.Application.Features.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string? Phone
) : IRequest<CustomerDto>;
