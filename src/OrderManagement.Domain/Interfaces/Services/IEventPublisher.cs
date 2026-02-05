using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Interfaces.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : IDomainEvent;
    Task PublishManyAsync(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}
