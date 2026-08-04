using Core.Entities;
using Application.DTOs;

namespace Application.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetOrderWithDetailsAsync(int id);
    Task<(IEnumerable<Order> Orders, int TotalCount)> GetFilteredAndPagedOrdersAsync(OrderQueryParameters parameters);
}
