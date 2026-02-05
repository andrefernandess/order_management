using System.Text.RegularExpressions;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }
    
    // Regex simples para validação de email
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private Email(string value)
    {
        Value = value;
    }
    
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.");
        
        email = email.Trim().ToLowerInvariant();
        
        if (email.Length > 256)
            throw new DomainException("Email cannot exceed 256 characters.");
        
        if (!EmailRegex.IsMatch(email))
            throw new DomainException($"'{email}' is not a valid email address.");
        
        return new Email(email);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public override string ToString() => Value;
    
    // Conversão implícita para string
    public static implicit operator string(Email email) => email.Value;
}
