using FluentAssertions;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    #region Create

    [Fact]
    public void Create_WithValidValues_ShouldCreateMoney()
    {
        // Act
        var money = Money.Create(100.50m, "BRL");

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Act
        var act = () => Money.Create(-10m, "BRL");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*negative*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidCurrency_ShouldThrowDomainException(string? currency)
    {
        // Act
        var act = () => Money.Create(100m, currency!);

        // Assert
        act.Should().Throw<DomainException>();
    }

    #endregion

    #region Zero

    [Fact]
    public void Zero_ShouldReturnMoneyWithZeroAmount()
    {
        // Act
        var zero = Money.Zero();

        // Assert
        zero.Amount.Should().Be(0);
        zero.Currency.Should().Be("BRL");
    }

    #endregion

    #region Operators

    [Fact]
    public void Add_TwoMoneyWithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        var money1 = Money.Create(50m, "BRL");
        var money2 = Money.Create(30m, "BRL");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(80m);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Add_DifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var brl = Money.Create(50m, "BRL");
        var usd = Money.Create(30m, "USD");

        // Act
        var act = () => brl + usd;

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*currencies*");
    }

    [Fact]
    public void Subtract_TwoMoneyWithSameCurrency_ShouldReturnDifference()
    {
        // Arrange
        var money1 = Money.Create(100m, "BRL");
        var money2 = Money.Create(30m, "BRL");

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70m);
    }

    [Fact]
    public void Multiply_ByQuantity_ShouldReturnProduct()
    {
        // Arrange
        var unitPrice = Money.Create(25.50m, "BRL");

        // Act
        var result = unitPrice * 4;

        // Assert
        result.Amount.Should().Be(102m);
    }

    #endregion

    #region Equality

    [Fact]
    public void Equals_SameValues_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100m, "BRL");
        var money2 = Money.Create(100m, "BRL");

        // Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100m, "BRL");
        var money2 = Money.Create(200m, "BRL");

        // Assert
        money1.Should().NotBe(money2);
        (money1 != money2).Should().BeTrue();
    }

    #endregion

    #region Comparison

    [Fact]
    public void GreaterThan_ShouldCompareCorrectly()
    {
        // Arrange
        var bigger = Money.Create(100m, "BRL");
        var smaller = Money.Create(50m, "BRL");

        // Assert
        (bigger > smaller).Should().BeTrue();
        (smaller > bigger).Should().BeFalse();
    }

    [Fact]
    public void LessThan_ShouldCompareCorrectly()
    {
        // Arrange
        var bigger = Money.Create(100m, "BRL");
        var smaller = Money.Create(50m, "BRL");

        // Assert
        (smaller < bigger).Should().BeTrue();
        (bigger < smaller).Should().BeFalse();
    }

    #endregion
}

