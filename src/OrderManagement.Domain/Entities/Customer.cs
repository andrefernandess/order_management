using OrderManagement.Domain.Common;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class Customer : EntityBase, IAuditableEntity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string? Phone { get; private set; }
    public Address? Address { get; private set; }
    public bool IsActive { get; private set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Navegação
    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    
    // EF Core precisa de construtor sem parâmetros
    private Customer() { }
    
    private Customer(string name, Email email, string? phone)
    {
        Name = name;
        Email = email;
        Phone = phone;
        IsActive = true;
    }
    
    public static Customer Create(string name, string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required.");
        
        if (name.Length > 200)
            throw new DomainException("Customer name cannot exceed 200 characters.");
        
        var emailVo = Email.Create(email);
        
        return new Customer(name.Trim(), emailVo, phone?.Trim());
    }
    
    public void Update(string name, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required.");
        
        Name = name.Trim();
        Phone = phone?.Trim();
    }
    
    public void UpdateEmail(string email)
    {
        Email = Email.Create(email);
    }
    
    public void SetAddress(Address address)
    {
        Address = address ?? throw new DomainException("Address cannot be null.");
    }
    
    public void Activate()
    {
        IsActive = true;
    }
    
    public void Deactivate()
    {
        IsActive = false;
    }
}
