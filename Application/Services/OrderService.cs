using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Application.Exceptions;


namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IOrderStatusHistoryService _historyService;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ICustomerRepository customerRepository, ICouponRepository couponRepository, IOrderStatusHistoryService historyService, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _couponRepository = couponRepository;
        _historyService = historyService;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrderDto>> GetOrdersAsync(OrderQueryParameters parameters)
    {
        var (orders, totalCount) = await _orderRepository.GetFilteredAndPagedOrdersAsync(parameters);
        
        return new PagedResult<OrderDto>
        {
            Items = _mapper.Map<IEnumerable<OrderDto>>(orders),
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }

    public async Task<OrderDto> GetOrderWithDetailsAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(id);
        if (order == null) throw new NotFoundException($"Order with ID {id} not found.");
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        var order = _mapper.Map<Order>(createOrderDto);
        
        if (order.OrderItems == null || !order.OrderItems.Any())
        {
            throw new BadRequestException("Cannot create an order with zero items.");
        }

        var customer = await _customerRepository.GetByIdIncludingInactiveAsync(order.CustomerId);
        if (customer == null)
        {
            throw new NotFoundException($"Customer with ID {order.CustomerId} does not exist.");
        }
        if (!customer.IsActive)
        {
            throw new BadRequestException($"Customer with ID {order.CustomerId} is inactive.");
        }
        
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdIncludingInactiveAsync(item.ProductId);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {item.ProductId} does not exist.");
            }
            if (!product.IsActive)
            {
                throw new BadRequestException($"Product with ID {item.ProductId} is inactive.");
            }
            
            // RACE CONDITION DISCUSSION:
            // If two users try to order the last unit of this product concurrently, both might read
            // the same StockQuantity (e.g., 1) and pass this condition. This leads to overselling
            // (stock going negative).
            // Possible solutions:
            // 1. Optimistic Concurrency: Use a concurrency token (e.g., RowVersion) in EF Core.
            //    It throws DbUpdateConcurrencyException if the row was modified by another transaction.
            // 2. Pessimistic Concurrency: Use database transactions with an appropriate isolation
            //    level (like Serializable) or row-level locking (e.g., SELECT ... WITH (UPDLOCK)).
            if (product.StockQuantity < item.Quantity)
            {
                throw new InsufficientStockException($"Insufficient stock for Product '{product.Name}'. Requested: {item.Quantity}, Available: {product.StockQuantity}");
            }
            
            product.StockQuantity -= item.Quantity;
            item.UnitPrice = product.UnitPrice;
        }
        
        order.OrderDate = DateTime.UtcNow;
        var originalTotal = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
        order.TotalAmount = originalTotal;
        
        if (!string.IsNullOrWhiteSpace(createOrderDto.CouponCode))
        {
            var coupon = await _couponRepository.GetByCodeAsync(createOrderDto.CouponCode);
            if (coupon == null || !coupon.IsActive || coupon.ExpiryDate < DateTime.UtcNow)
            {
                throw new InvalidOperationException($"Coupon code '{createOrderDto.CouponCode}' is invalid, inactive, or expired.");
            }
            
            order.CouponId = coupon.Id;
            var discount = originalTotal * (coupon.DiscountPercentage / 100m);
            order.TotalAmount = originalTotal - discount;
        }
        
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();
        
        await _historyService.RecordStatusChangeAsync(order.Id, null, order.Status);
        
        var createdOrder = await _orderRepository.GetOrderWithDetailsAsync(order.Id);
        return _mapper.Map<OrderDto>(createdOrder ?? order);
    }

    public async Task ShipOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) throw new NotFoundException($"Order with ID {id} not found.");
        
        if (order.Status != OrderStatus.Pending)
        {
            throw new BadRequestException($"Cannot ship order with status {order.Status}. Only Pending orders can be shipped.");
        }
        
        var oldStatus = order.Status;
        order.Status = OrderStatus.Shipped;
        await _orderRepository.SaveChangesAsync();
        
        await _historyService.RecordStatusChangeAsync(order.Id, oldStatus, order.Status);
    }

    public async Task DeliverOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) throw new NotFoundException($"Order with ID {id} not found.");
        
        if (order.Status != OrderStatus.Shipped)
        {
            throw new BadRequestException($"Cannot deliver order with status {order.Status}. Only Shipped orders can be delivered.");
        }
        
        var oldStatus = order.Status;
        order.Status = OrderStatus.Delivered;
        await _orderRepository.SaveChangesAsync();
        
        await _historyService.RecordStatusChangeAsync(order.Id, oldStatus, order.Status);
    }

    public async Task CancelOrderAsync(int id)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(id);
        if (order == null) throw new NotFoundException($"Order with ID {id} not found.");
        
        if (order.Status != OrderStatus.Pending)
        {
            throw new BadRequestException($"Cannot cancel order with status {order.Status}. Only Pending orders can be cancelled.");
        }
        
        foreach (var item in order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity += item.Quantity;
            }
        }
        
        var oldStatus = order.Status;
        order.Status = OrderStatus.Cancelled;
        await _orderRepository.SaveChangesAsync();
        
        await _historyService.RecordStatusChangeAsync(order.Id, oldStatus, order.Status);
    }
}
