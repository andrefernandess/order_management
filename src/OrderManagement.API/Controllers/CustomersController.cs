using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Application.Features.Customers.Commands.CreateCustomer;
using OrderManagement.Application.Features.Customers.Commands.UpdateCustomer;
using OrderManagement.Application.Features.Customers.Queries.GetCustomerById;
using OrderManagement.Application.Features.Customers.Queries.GetCustomers;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Descomente para habilitar autenticação
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os clientes com paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<CustomerDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        CancellationToken ct = default)
    {
        var query = new GetCustomersQuery(pageNumber, pageSize, isActive);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Busca um cliente por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDetailDto>> GetById(Guid id, CancellationToken ct = default)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query, ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Create(
        [FromBody] CreateCustomerCommand command,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Update(
        Guid id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateCustomerCommand(id, request.Name, request.Phone);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}

public record UpdateCustomerRequest(string Name, string? Phone);
