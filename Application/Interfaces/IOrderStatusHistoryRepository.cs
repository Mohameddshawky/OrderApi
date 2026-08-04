using Core.Entities;

namespace Application.Interfaces;

public interface IOrderStatusHistoryRepository : IGenericRepository<OrderStatusHistory>
{
    Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(int orderId);
}
