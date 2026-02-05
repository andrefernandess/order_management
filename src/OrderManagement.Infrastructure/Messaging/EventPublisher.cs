using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Interfaces.Services;

namespace OrderManagement.Infrastructure.Messaging;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IPublishEndpoint publishEndpoint, ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IDomainEvent
    {
        try
        {
            _logger.LogInformation(
                "Publishing event {EventType} occurred at {OccurredAt}",
                typeof(T).Name,
                @event.OccurredAt);

            await _publishEndpoint.Publish(@event, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventType}", typeof(T).Name);
            throw;
        }
    }

    public async Task PublishManyAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default)
    {
        foreach (var @event in events)
        {
            await PublishAsync(@event, ct);
        }
    }
}

