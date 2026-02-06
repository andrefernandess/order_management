using AutoMapper;
using FluentAssertions;
using Moq;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Application.Features.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Exceptions;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.UnitTests.Application.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();

        // Configura UnitOfWork para retornar os repositórios mockados
        _unitOfWorkMock.Setup(u => u.Orders).Returns(_orderRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Customers).Returns(_customerRepositoryMock.Object);

        _handler = new CreateOrderCommandHandler(
            _unitOfWorkMock.Object,
            _productRepositoryMock.Object,
            _mapperMock.Object);
    }

    #region Success Cases

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = CreateValidCommand(customerId);
        var customer = CreateCustomer(customerId);
        var product = CreateProduct("PROD-001", "Produto Teste", 100m, 10);

        SetupCustomerExists(customerId, customer);
        SetupProductExists(product);
        SetupOrderCreation();
        SetupMapper();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        _orderRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _productRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleItems_ShouldAddAllItemsToOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = CreateCommandWithMultipleItems(customerId);
        var customer = CreateCustomer(customerId);
        var product1 = CreateProduct("PROD-001", "Produto 1", 50m, 10);
        var product2 = CreateProduct("PROD-002", "Produto 2", 75m, 20);

        SetupCustomerExists(customerId, customer);
        SetupProductExists(product1);
        SetupProductExists(product2);
        SetupOrderCreation();
        SetupMapper();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        _productRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    #endregion

    #region Customer Validation

    [Fact]
    public async Task Handle_CustomerNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = CreateValidCommand(customerId);

        SetupCustomerNotFound(customerId);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Customer*");

        _orderRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    #endregion

    #region Product Validation

    [Fact]
    public async Task Handle_ProductNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = CreateValidCommand(customerId);
        var customer = CreateCustomer(customerId);

        SetupCustomerExists(customerId, customer);
        SetupProductNotFound("PROD-001");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Product*");
    }

    [Fact]
    public async Task Handle_ProductInactive_ShouldThrowDomainException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = CreateValidCommand(customerId);
        var customer = CreateCustomer(customerId);
        var inactiveProduct = CreateProduct("PROD-001", "Produto Inativo", 100m, 10, isActive: false);

        SetupCustomerExists(customerId, customer);
        SetupProductExists(inactiveProduct);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*not available*");
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldThrowDomainException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = CreateValidCommand(customerId, quantity: 100);
        var customer = CreateCustomer(customerId);
        var product = CreateProduct("PROD-001", "Produto", 50m, stockQuantity: 5);

        SetupCustomerExists(customerId, customer);
        SetupProductExists(product);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Insufficient stock*");
    }

    #endregion

    #region Stock Management

    [Fact]
    public async Task Handle_ValidOrder_ShouldDecreaseProductStock()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var quantityOrdered = 3;
        var initialStock = 10;
        var command = CreateValidCommand(customerId, quantity: quantityOrdered);
        var customer = CreateCustomer(customerId);
        var product = CreateProduct("PROD-001", "Produto", 100m, initialStock);

        SetupCustomerExists(customerId, customer);
        SetupProductExists(product);
        SetupOrderCreation();
        SetupMapper();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        product.StockQuantity.Should().Be(initialStock - quantityOrdered);
    }

    #endregion

    #region Helper Methods

    private static CreateOrderCommand CreateValidCommand(Guid customerId, int quantity = 2)
    {
        return new CreateOrderCommand(
            CustomerId: customerId,
            ShippingAddress: new AddressCommand(
                Street: "Rua Teste",
                Number: "123",
                Complement: "Apto 1",
                Neighborhood: "Centro",
                City: "Assis",
                State: "SP",
                ZipCode: "19800-000"),
            Items: new List<OrderItemCommand>
            {
                new("PROD-001", quantity)
            },
            Notes: "Teste de pedido");
    }

    private static CreateOrderCommand CreateCommandWithMultipleItems(Guid customerId)
    {
        return new CreateOrderCommand(
            CustomerId: customerId,
            ShippingAddress: new AddressCommand(
                Street: "Rua Teste",
                Number: "123",
                Complement: null,
                Neighborhood: "Centro",
                City: "Assis",
                State: "SP",
                ZipCode: "19800-000"),
            Items: new List<OrderItemCommand>
            {
                new("PROD-001", 2),
                new("PROD-002", 3)
            },
            Notes: null);
    }

    private static Customer CreateCustomer(Guid id)
    {
        return Customer.Create(
            name: "Cliente Teste",
            email: $"cliente-{id}@teste.com",
            phone: "18999999999");
    }

    private static Product CreateProduct(
        string id,
        string name,
        decimal price,
        int stockQuantity,
        bool isActive = true)
    {
        var product = Product.Create(
            name: name,
            description: $"Descrição de {name}",
            category: "Categoria Teste",
            price: price,
            stockQuantity: stockQuantity);
        
        // Setar o Id via reflection (é private set)
        typeof(Product)
            .GetProperty("Id")!
            .SetValue(product, id);
        
        if (!isActive)
            product.Deactivate();
            
        return product;
    }
    
    private void SetupCustomerExists(Guid customerId, Customer customer)
    {
        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
    }

    private void SetupCustomerNotFound(Guid customerId)
    {
        _customerRepositoryMock
            .Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);
    }

    private void SetupProductExists(Product product)
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
    }
    private void SetupProductNotFound(string productId)
    {
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);
    }

    private void SetupOrderCreation()
    {
        _orderRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order order, CancellationToken _) => order);

        _orderRepositoryMock
            .Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) =>
            {
                var order = Order.Create(Guid.NewGuid(), Address.Create(
                    "Rua", "1", null, "Bairro", "Cidade", "SP", "00000-000"));
                return order;
            });

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private void SetupMapper()
    {
        _mapperMock
            .Setup(m => m.Map<OrderDetailDto>(It.IsAny<Order>()))
            .Returns(new OrderDetailDto());
    }

    #endregion
}
