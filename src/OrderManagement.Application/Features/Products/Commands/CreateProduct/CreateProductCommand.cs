using MediatR;
using OrderManagement.Application.DTOs.Products;

namespace OrderManagement.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    string Category,
    decimal Price,
    int StockQuantity,
    string? ImageUrl,
    List<string>? Tags
) : IRequest<ProductDto>;
