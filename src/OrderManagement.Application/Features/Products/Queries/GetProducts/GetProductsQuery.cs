using MediatR;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Products;

namespace OrderManagement.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Category = null,
    string? SearchTerm = null,
    bool OnlyActive = true
) : IRequest<PaginatedList<ProductDto>>;
