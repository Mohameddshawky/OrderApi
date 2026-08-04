using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces;

public interface IOrderStatusHistoryService
{
    Task RecordStatusChangeAsync(int orderId, OrderStatus? oldStatus, OrderStatus newStatus);
    Task<IEnumerable<OrderStatusHistoryDto>> GetOrderHistoryAsync(int orderId);
}
