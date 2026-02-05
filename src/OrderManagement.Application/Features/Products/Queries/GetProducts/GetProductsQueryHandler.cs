using AutoMapper;
using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Product> products;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            products = await _productRepository.SearchAsync(request.SearchTerm, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.Category))
        {
            products = await _productRepository.GetByCategoryAsync(request.Category, cancellationToken);
        }
        else if (request.OnlyActive)
        {
            products = await _productRepository.GetActiveProductsAsync(cancellationToken);
        }
        else
        {
            products = await _productRepository.GetAllAsync(cancellationToken);
        }

        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        return PaginatedList<ProductDto>.Create(dtos, request.PageNumber, request.PageSize);
    }
}
