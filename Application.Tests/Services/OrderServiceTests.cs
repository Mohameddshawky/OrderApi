using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Core.Entities;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<ICouponRepository> _mockCouponRepo;
    private readonly Mock<IOrderStatusHistoryService> _mockHistoryService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockCouponRepo = new Mock<ICouponRepository>();
        _mockHistoryService = new Mock<IOrderStatusHistoryService>();
        _mockMapper = new Mock<IMapper>();

        _orderService = new OrderService(
            _mockOrderRepo.Object,
            _mockProductRepo.Object,
            _mockCustomerRepo.Object,
            _mockCouponRepo.Object,
            _mockHistoryService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetOrdersAsync_ReturnsMappedListOfOrders()
    {
        // Arrange
        var queryParams = new OrderQueryParameters { Page = 1, PageSize = 10 };
        var orders = new List<Order> { new Order { Id = 1 } };
        var orderDtos = new List<OrderDto> { new OrderDto { Id = 1 } };

        _mockOrderRepo.Setup(r => r.GetFilteredAndPagedOrdersAsync(queryParams))
            .ReturnsAsync((orders, 1));
        
        _mockMapper.Setup(m => m.Map<IEnumerable<OrderDto>>(orders))
            .Returns(orderDtos);

        // Act
        var result = await _orderService.GetOrdersAsync(queryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetOrderWithDetailsAsync_OrderExists_ReturnsMappedOrder()
    {
        // Arrange
        var orderId = 1;
        var order = new Order { Id = orderId };
        var orderDto = new OrderDto { Id = orderId };

        _mockOrderRepo.Setup(r => r.GetOrderWithDetailsAsync(orderId))
            .ReturnsAsync(order);
            
        _mockMapper.Setup(m => m.Map<OrderDto>(order))
            .Returns(orderDto);

        // Act
        var result = await _orderService.GetOrderWithDetailsAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task GetOrderWithDetailsAsync_OrderDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var orderId = 1;
        _mockOrderRepo.Setup(r => r.GetOrderWithDetailsAsync(orderId))
            .ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetOrderWithDetailsAsync(orderId));
    }

    [Fact]
    public async Task CreateOrderAsync_ValidRequest_CreatesOrderAndReturnsDto()
    {
        // Arrange
        var createDto = new CreateOrderDto { CustomerId = 1 };
        var orderItems = new List<OrderItem> 
        { 
            new OrderItem { ProductId = 1, Quantity = 2 } 
        };
        var order = new Order { CustomerId = 1, OrderItems = orderItems };
        var orderDto = new OrderDto { Id = 1 };
        
        var customer = new Customer { Id = 1, IsActive = true };
        var product = new Product { Id = 1, IsActive = true, StockQuantity = 10, UnitPrice = 100 };

        _mockMapper.Setup(m => m.Map<Order>(createDto)).Returns(order);
        _mockCustomerRepo.Setup(r => r.GetByIdIncludingInactiveAsync(1)).ReturnsAsync(customer);
        _mockProductRepo.Setup(r => r.GetByIdIncludingInactiveAsync(1)).ReturnsAsync(product);
        
        _mockOrderRepo.Setup(r => r.GetOrderWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(order);
        _mockMapper.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto);

        // Act
        var result = await _orderService.CreateOrderAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        
        _mockOrderRepo.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Once);
        _mockOrderRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_CustomerDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var createDto = new CreateOrderDto { CustomerId = 1 };
        var order = new Order { CustomerId = 1, OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1 } } };
        
        _mockMapper.Setup(m => m.Map<Order>(createDto)).Returns(order);
        _mockCustomerRepo.Setup(r => r.GetByIdIncludingInactiveAsync(1)).ReturnsAsync((Customer?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _orderService.CreateOrderAsync(createDto));
    }

    [Fact]
    public async Task CreateOrderAsync_InsufficientStock_ThrowsInsufficientStockException()
    {
        // Arrange
        var createDto = new CreateOrderDto { CustomerId = 1 };
        var orderItems = new List<OrderItem> 
        { 
            new OrderItem { ProductId = 1, Quantity = 5 } 
        };
        var order = new Order { CustomerId = 1, OrderItems = orderItems };
        
        var customer = new Customer { Id = 1, IsActive = true };
        var product = new Product { Id = 1, IsActive = true, StockQuantity = 2 }; // Requested 5, stock 2

        _mockMapper.Setup(m => m.Map<Order>(createDto)).Returns(order);
        _mockCustomerRepo.Setup(r => r.GetByIdIncludingInactiveAsync(1)).ReturnsAsync(customer);
        _mockProductRepo.Setup(r => r.GetByIdIncludingInactiveAsync(1)).ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockException>(() => _orderService.CreateOrderAsync(createDto));
    }
}
