using AutoMapper;
using MediatR;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Verifica se email já existe
        var emailExists = await _unitOfWork.Customers.EmailExistsAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            throw new Application.Common.Exceptions.ValidationException(
                "Email", $"Email '{request.Email}' is already registered.");
        }

        // Cria o cliente usando o método de fábrica da entidade
        var customer = Customer.Create(request.Name, request.Email, request.Phone);

        // Persiste
        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Retorna DTO
        return _mapper.Map<CustomerDto>(customer);
    }
}
