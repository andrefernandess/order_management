using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Events;

namespace OrderManagement.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Processing OrderCreatedEvent - OrderId: {OrderId}, CustomerId: {CustomerId}, Total: {Total}",
            @event.OrderId,
            @event.CustomerId,
            @event.TotalAmount);

        // Aqui você pode:
        // - Enviar email de confirmação
        // - Notificar sistema de estoque
        // - Atualizar relatórios
        // - Integrar com ERP
        // etc.

        return Task.CompletedTask;
    }
}
