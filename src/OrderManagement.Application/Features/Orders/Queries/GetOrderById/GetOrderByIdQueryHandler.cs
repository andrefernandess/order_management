using AutoMapper;
using MediatR;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderDetailDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithItemsAsync(request.Id, cancellationToken);
        
        return order is null ? null : _mapper.Map<OrderDetailDto>(order);
    }
}
