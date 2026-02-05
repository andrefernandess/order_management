using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCustomersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = request.IsActive.HasValue && request.IsActive.Value
            ? await _unitOfWork.Customers.GetActiveCustomersAsync(cancellationToken)
            : await _unitOfWork.Customers.GetAllAsync(cancellationToken);

        var dtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);

        return PaginatedList<CustomerDto>.Create(dtos, request.PageNumber, request.PageSize);
    }
}
