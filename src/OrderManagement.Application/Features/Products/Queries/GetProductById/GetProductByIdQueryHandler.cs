using AutoMapper;
using MediatR;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Domain.Interfaces.Services;

namespace OrderManagement.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ProductDetailDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{request.Id}";

        // Tenta buscar do cache primeiro
        var cachedProduct = await _cacheService.GetAsync<ProductDetailDto>(cacheKey, cancellationToken);
        if (cachedProduct is not null)
        {
            return cachedProduct;
        }

        // Busca do MongoDB
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product is null)
            return null;

        var dto = _mapper.Map<ProductDetailDto>(product);

        // Armazena no cache por 10 minutos
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), cancellationToken);

        return dto;
    }
}
