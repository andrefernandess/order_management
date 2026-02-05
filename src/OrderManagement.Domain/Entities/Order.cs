using OrderManagement.Domain.Common;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class Order : EntityBase, IAuditableEntity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public Money SubTotal { get; private set; } = Money.Zero();
    public Money Discount { get; private set; } = Money.Zero();
    public Money ShippingCost { get; private set; } = Money.Zero();
    public Money Total { get; private set; } = Money.Zero();
    public string? Notes { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navegação
    public Customer Customer { get; private set; } = null!;
    
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    
    // EF Core
    private Order() { }
    
    private Order(Guid customerId, Address shippingAddress)
    {
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        OrderNumber = GenerateOrderNumber();
    }
    
    public static Order Create(Guid customerId, Address shippingAddress)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("CustomerId is required.");
        
        var order = new Order(customerId, shippingAddress);
        
        return order;
    }
    
    public void AddItem(string productId, string productName, int quantity, Money unitPrice)
    {
        EnsureCanModify();
        
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var item = new OrderItem(Id, productId, productName, quantity, unitPrice);
            _items.Add(item);
        }
        
        RecalculateTotals();
    }
    
    public void RemoveItem(Guid itemId)
    {
        EnsureCanModify();
        
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException($"Item with id '{itemId}' not found.");
        
        _items.Remove(item);
        RecalculateTotals();
    }
    
    public void UpdateItemQuantity(Guid itemId, int quantity)
    {
        EnsureCanModify();
        
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException($"Item with id '{itemId}' not found.");
        
        if (quantity <= 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.UpdateQuantity(quantity);
        }
        
        RecalculateTotals();
    }
    
    public void ApplyDiscount(Money discount)
    {
        EnsureCanModify();
        
        if (discount.Amount > SubTotal.Amount)
            throw new DomainException("Discount cannot be greater than subtotal.");
        
        Discount = discount;
        RecalculateTotals();
    }
    
    public void SetShippingCost(Money shippingCost)
    {
        EnsureCanModify();
        ShippingCost = shippingCost;
        RecalculateTotals();
    }
    
    public void SetNotes(string? notes)
    {
        Notes = notes?.Trim();
    }
    
    public void Confirm(PaymentMethod paymentMethod)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot confirm order with status '{Status}'.");
        
        if (!_items.Any())
            throw new DomainException("Cannot confirm order without items.");
        
        var oldStatus = Status;
        PaymentMethod = paymentMethod;
        Status = OrderStatus.Confirmed;
        PaidAt = DateTime.UtcNow;
        
        AddDomainEvent(new OrderCreatedEvent(Id, CustomerId, Total.Amount));
        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, Status));
    }
    
    public void StartProcessing()
    {
        ChangeStatus(OrderStatus.Confirmed, OrderStatus.Processing);
    }
    
    public void Ship()
    {
        ChangeStatus(OrderStatus.Processing, OrderStatus.Shipped);
        ShippedAt = DateTime.UtcNow;
    }
    
    public void Deliver()
    {
        ChangeStatus(OrderStatus.Shipped, OrderStatus.Delivered);
        DeliveredAt = DateTime.UtcNow;
    }
    
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered)
            throw new DomainException("Cannot cancel a delivered order.");
        
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Order is already cancelled.");
        
        var oldStatus = Status;
        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;
        
        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, Status));
    }
    
    private void ChangeStatus(OrderStatus expectedStatus, OrderStatus newStatus)
    {
        if (Status != expectedStatus)
            throw new DomainException($"Cannot change status from '{Status}' to '{newStatus}'.");
        
        var oldStatus = Status;
        Status = newStatus;
        
        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, newStatus));
    }
    
    private void EnsureCanModify()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot modify order with status '{Status}'.");
    }
    
    private void RecalculateTotals()
    {
        SubTotal = _items.Aggregate(
            Money.Zero(), 
            (acc, item) => acc + item.TotalPrice);
        
        Total = SubTotal - Discount + ShippingCost;
    }
    
    private static string GenerateOrderNumber()
    {
        // Formato: ORD-YYYYMMDD-XXXXX
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(10000, 99999);
        return $"ORD-{date}-{random}";
    }
}
