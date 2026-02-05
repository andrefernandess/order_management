using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Events;

namespace OrderManagement.Infrastructure.Messaging.Consumers;

public class OrderStatusChangedConsumer : IConsumer<OrderStatusChangedEvent>
{
    private readonly ILogger<OrderStatusChangedConsumer> _logger;

    public OrderStatusChangedConsumer(ILogger<OrderStatusChangedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
    {
        var @event = context.Message;

        _logger.LogInformation(
            "Processing OrderStatusChangedEvent - OrderId: {OrderId}, From: {OldStatus}, To: {NewStatus}",
            @event.OrderId,
            @event.OldStatus,
            @event.NewStatus);

        // Aqui você pode:
        // - Enviar notificação push
        // - Atualizar dashboard em tempo real
        // - Disparar webhook para integrações
        // etc.

        return Task.CompletedTask;
    }
}
