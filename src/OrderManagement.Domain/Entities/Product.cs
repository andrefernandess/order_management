using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = string.Empty;
    
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "BRL";
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public string? ImageUrl { get; private set; }
    public List<string> Tags { get; private set; } = new();
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Construtor para MongoDB
    private Product() { }
    
    public static Product Create(
        string name,
        string description,
        string category,
        decimal price,
        int stockQuantity,
        string? imageUrl = null,
        string currency = "BRL")
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateStock(stockQuantity);
        
        return new Product
        {
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
            Category = category?.Trim() ?? string.Empty,
            Price = Math.Round(price, 2),
            Currency = currency,
            StockQuantity = stockQuantity,
            IsActive = true,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Update(string name, string description, string category, decimal price, string? imageUrl)
    {
        ValidateName(name);
        ValidatePrice(price);
        
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Category = category?.Trim() ?? string.Empty;
        Price = Math.Round(price, 2);
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateStock(int quantity)
    {
        ValidateStock(quantity);
        StockQuantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");
        
        if (StockQuantity < quantity)
            throw new DomainException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}");
        
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");
        
        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    
    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag.ToLower()))
            Tags.Add(tag.ToLower().Trim());
    }
    
    public void RemoveTag(string tag)
    {
        Tags.Remove(tag.ToLower().Trim());
    }
    
    public Money GetPrice() => Money.Create(Price, Currency);
    
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");
        
        if (name.Length > 200)
            throw new DomainException("Product name cannot exceed 200 characters.");
    }
    
    private static void ValidatePrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("Price cannot be negative.");
    }
    
    private static void ValidateStock(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");
    }
}
