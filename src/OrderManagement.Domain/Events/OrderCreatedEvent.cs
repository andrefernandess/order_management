using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Events;

public sealed class OrderCreatedEvent : IDomainEvent
{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime OccurredAt { get; private set; }
    
    public OrderCreatedEvent() { }
    public OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalAmount)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OccurredAt = DateTime.UtcNow;
    }
}
