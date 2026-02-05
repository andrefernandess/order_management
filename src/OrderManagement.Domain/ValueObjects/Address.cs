using OrderManagement.Domain.Common;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street { get; }
    public string Number { get; }
    public string? Complement { get; }
    public string Neighborhood { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }
    
    private Address(
        string street,
        string number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        string country)
    {
        Street = street;
        Number = number;
        Complement = complement;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }
    
    public static Address Create(
        string street,
        string number,
        string? complement,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        string country = "Brazil")
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street is required.");
        
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("Number is required.");
        
        if (string.IsNullOrWhiteSpace(neighborhood))
            throw new DomainException("Neighborhood is required.");
        
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");
        
        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State is required.");
        
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("ZipCode is required.");
        
        return new Address(
            street.Trim(),
            number.Trim(),
            complement?.Trim(),
            neighborhood.Trim(),
            city.Trim(),
            state.Trim().ToUpperInvariant(),
            zipCode.Trim().Replace("-", ""),
            country.Trim());
    }
    
    public string GetFullAddress()
    {
        var complement = string.IsNullOrEmpty(Complement) ? "" : $", {Complement}";
        return $"{Street}, {Number}{complement} - {Neighborhood}, {City}/{State} - {ZipCode}";
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return Number;
        yield return Complement;
        yield return Neighborhood;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }
    
    public override string ToString() => GetFullAddress();
}
