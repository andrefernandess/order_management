using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PaginatedList<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Order> orders;

        if (request.CustomerId.HasValue)
        {
            orders = await _unitOfWork.Orders.GetByCustomerIdAsync(request.CustomerId.Value, cancellationToken);
        }
        else if (request.Status.HasValue)
        {
            orders = await _unitOfWork.Orders.GetByStatusAsync(request.Status.Value, cancellationToken);
        }
        else
        {
            orders = await _unitOfWork.Orders.GetRecentOrdersAsync(100, cancellationToken);
        }

        var dtos = _mapper.Map<IEnumerable<OrderDto>>(orders);

        return PaginatedList<OrderDto>.Create(dtos, request.PageNumber, request.PageSize);
    }
}
