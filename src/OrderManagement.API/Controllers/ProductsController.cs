using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Application.Features.Products.Commands.CreateProduct;
using OrderManagement.Application.Features.Products.Queries.GetProductById;
using OrderManagement.Application.Features.Products.Queries.GetProducts;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os produtos com paginação e filtros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? search = null,
        [FromQuery] bool onlyActive = true,
        CancellationToken ct = default)
    {
        var query = new GetProductsQuery(pageNumber, pageSize, category, search, onlyActive);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Busca um produto por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> GetById(string id, CancellationToken ct = default)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query, ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
