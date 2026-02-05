using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Application.Common.Models;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Application.Features.Orders.Commands.CancelOrder;
using OrderManagement.Application.Features.Orders.Commands.ConfirmOrder;
using OrderManagement.Application.Features.Orders.Commands.CreateOrder;
using OrderManagement.Application.Features.Orders.Queries.GetOrderById;
using OrderManagement.Application.Features.Orders.Queries.GetOrders;
using OrderManagement.Domain.Enums;

namespace OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lista todos os pedidos com paginação e filtros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<OrderDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? customerId = null,
        [FromQuery] OrderStatus? status = null,
        CancellationToken ct = default)
    {
        var query = new GetOrdersQuery(pageNumber, pageSize, customerId, status);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    /// Busca um pedido por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailDto>> GetById(Guid id, CancellationToken ct = default)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query, ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDetailDto>> Create(
        [FromBody] CreateOrderCommand command,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Confirma um pedido (pagamento)
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailDto>> Confirm(
        Guid id,
        [FromBody] ConfirmOrderRequest request,
        CancellationToken ct = default)
    {
        var command = new ConfirmOrderCommand(id, request.PaymentMethod);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    /// <summary>
    /// Cancela um pedido
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Cancel(
        Guid id,
        [FromBody] CancelOrderRequest request,
        CancellationToken ct = default)
    {
        var command = new CancelOrderCommand(id, request.Reason);
        await _mediator.Send(command, ct);
        return Ok(new { message = "Order cancelled successfully" });
    }
}

public record ConfirmOrderRequest(PaymentMethod PaymentMethod);
public record CancelOrderRequest(string Reason);
