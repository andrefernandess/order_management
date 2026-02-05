using OrderManagement.Domain.Common;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Events;

public sealed class OrderStatusChangedEvent : IDomainEvent
{
    public Guid OrderId { get; init; }
    public OrderStatus OldStatus { get; init; }
    public OrderStatus NewStatus { get; init; }
    public DateTime OccurredAt { get; init; }
    
    public OrderStatusChangedEvent() { }
    public OrderStatusChangedEvent(Guid orderId, OrderStatus oldStatus, OrderStatus newStatus)
    {
        OrderId = orderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        OccurredAt = DateTime.UtcNow;
    }
}
