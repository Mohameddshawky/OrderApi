using Application.DTOs;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<PagedResult<OrderDto>> GetOrdersAsync(OrderQueryParameters parameters);
    Task<OrderDto> GetOrderWithDetailsAsync(int id);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task ShipOrderAsync(int id);
    Task DeliverOrderAsync(int id);
    Task CancelOrderAsync(int id);
}
