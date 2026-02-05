using MediatR;
using OrderManagement.Application.DTOs.Products;

namespace OrderManagement.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(string Id) : IRequest<ProductDetailDto?>;
