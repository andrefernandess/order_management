using OrderManagement.Domain.Common;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class OrderItem : EntityBase
{
    public Guid OrderId { get; private set; }
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money TotalPrice => UnitPrice * Quantity;
    
    // Navegação
    public Order Order { get; private set; } = null!;
    
    // EF Core
    private OrderItem() { }
    
    internal OrderItem(
        Guid orderId,
        string productId,
        string productName,
        int quantity,
        Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new DomainException("ProductId is required.");
        
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("ProductName is required.");
        
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");
        
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    
    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");
        
        Quantity = quantity;
    }
}
