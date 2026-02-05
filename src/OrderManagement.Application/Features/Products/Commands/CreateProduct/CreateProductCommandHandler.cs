using AutoMapper;
using MediatR;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.StockQuantity,
            request.ImageUrl
        );

        // Adiciona tags se houver
        if (request.Tags is not null)
        {
            foreach (var tag in request.Tags)
            {
                product.AddTag(tag);
            }
        }

        await _productRepository.AddAsync(product, cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}
