using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Events;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.UnitTests.Domain.Entities;

public class OrderTests
{
    private static Address CreateValidAddress() => Address.Create(
        "Rua Teste", "123", null, "Centro", "Assis", "SP", "19800-000", "Brasil");

    private static Money CreateMoney(decimal amount) => Money.Create(amount, "BRL");

    #region Create

    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var address = CreateValidAddress();

        // Act
        var order = Order.Create(customerId, address);

        // Assert
        order.Should().NotBeNull();
        order.CustomerId.Should().Be(customerId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.OrderNumber.Should().StartWith("ORD-");
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ShouldThrowDomainException()
    {
        // Arrange
        var emptyCustomerId = Guid.Empty;
        var address = CreateValidAddress();

        // Act
        var act = () => Order.Create(emptyCustomerId, address);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*CustomerId*");
    }

    #endregion

    #region AddItem

    [Fact]
    public void AddItem_ToNewOrder_ShouldAddItemAndCalculateTotal()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());

        // Act
        order.AddItem("PROD-001", "Produto Teste", 2, CreateMoney(50m));

        // Assert
        order.Items.Should().HaveCount(1);
        order.SubTotal.Amount.Should().Be(100m);
        order.Total.Amount.Should().Be(100m);
    }

    [Fact]
    public void AddItem_SameProductTwice_ShouldIncreaseQuantity()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());
        order.AddItem("PROD-001", "Produto Teste", 2, CreateMoney(50m));

        // Act
        order.AddItem("PROD-001", "Produto Teste", 3, CreateMoney(50m));

        // Assert
        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(5);
        order.SubTotal.Amount.Should().Be(250m);
    }

    [Fact]
    public void AddItem_ToConfirmedOrder_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateConfirmedOrder();

        // Act
        var act = () => order.AddItem("PROD-002", "Outro Produto", 1, CreateMoney(30m));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot modify*");
    }

    #endregion

    #region Confirm

    [Fact]
    public void Confirm_WithItems_ShouldChangeStatusAndRaiseDomainEvents()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());
        order.AddItem("PROD-001", "Produto Teste", 1, CreateMoney(100m));

        // Act
        order.Confirm(PaymentMethod.CreditCard);

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.PaymentMethod.Should().Be(PaymentMethod.CreditCard);
        order.PaidAt.Should().NotBeNull();
        
        // Verifica Domain Events
        order.DomainEvents.Should().HaveCount(2);
        order.DomainEvents.Should().ContainItemsAssignableTo<OrderCreatedEvent>();
        order.DomainEvents.Should().ContainItemsAssignableTo<OrderStatusChangedEvent>();
    }

    [Fact]
    public void Confirm_WithoutItems_ShouldThrowDomainException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());

        // Act
        var act = () => order.Confirm(PaymentMethod.CreditCard);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*without items*");
    }

    [Fact]
    public void Confirm_AlreadyConfirmedOrder_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateConfirmedOrder();

        // Act
        var act = () => order.Confirm(PaymentMethod.Pix);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Cannot confirm*");
    }

    #endregion

    #region Cancel

    [Fact]
    public void Cancel_PendingOrder_ShouldChangeStatusAndRaiseEvent()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());
        order.AddItem("PROD-001", "Produto Teste", 1, CreateMoney(100m));

        // Act
        order.Cancel("Cliente desistiu");

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.CancellationReason.Should().Be("Cliente desistiu");
        order.CancelledAt.Should().NotBeNull();
        order.DomainEvents.Should().ContainItemsAssignableTo<OrderStatusChangedEvent>();
    }

    [Fact]
    public void Cancel_DeliveredOrder_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateDeliveredOrder();

        // Act
        var act = () => order.Cancel("Quero cancelar");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*delivered*");
    }

    #endregion

    #region ApplyDiscount

    [Fact]
    public void ApplyDiscount_ValidDiscount_ShouldReduceTotal()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());
        order.AddItem("PROD-001", "Produto", 2, CreateMoney(100m)); // Total: 200

        // Act
        order.ApplyDiscount(CreateMoney(50m));

        // Assert
        order.Discount.Amount.Should().Be(50m);
        order.Total.Amount.Should().Be(150m);
    }

    [Fact]
    public void ApplyDiscount_GreaterThanSubtotal_ShouldThrowDomainException()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());
        order.AddItem("PROD-001", "Produto", 1, CreateMoney(100m));

        // Act
        var act = () => order.ApplyDiscount(CreateMoney(150m));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*greater than subtotal*");
    }

    #endregion

    #region Helpers

    private static Order CreateConfirmedOrder()
    {
        var order = Order.Create(Guid.NewGuid(), CreateValidAddress());
        order.AddItem("PROD-001", "Produto", 1, CreateMoney(100m));
        order.Confirm(PaymentMethod.CreditCard);
        order.ClearDomainEvents();
        return order;
    }

    private static Order CreateDeliveredOrder()
    {
        var order = CreateConfirmedOrder();
        order.StartProcessing();
        order.Ship();
        order.Deliver();
        order.ClearDomainEvents();
        return order;
    }

    #endregion
}
