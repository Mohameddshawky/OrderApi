using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderStatusHistoryRepository : GenericRepository<OrderStatusHistory>, IOrderStatusHistoryRepository
{
    public OrderStatusHistoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(int orderId)
    {
        return await _dbSet
            .Where(h => h.OrderId == orderId)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();
    }
}
