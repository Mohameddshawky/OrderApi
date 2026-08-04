using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;

namespace Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetFilteredAndPagedOrdersAsync(OrderQueryParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        if (parameters.Status.HasValue)
        {
            query = query.Where(o => o.Status == parameters.Status.Value);
        }

        if (parameters.CustomerId.HasValue)
        {
            query = query.Where(o => o.CustomerId == parameters.CustomerId.Value);
        }

        if (parameters.FromDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= parameters.FromDate.Value);
        }

        if (parameters.ToDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= parameters.ToDate.Value);
        }

        var totalCount = await query.CountAsync();

        var orders = await query
            .Include(o => o.Customer)
            .Include(o => o.Coupon)
            .Include(o => o.OrderItems)

            .ThenInclude(oi => oi.Product)
            .IgnoreQueryFilters()
            .OrderByDescending(o => o.OrderDate)
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return (orders, totalCount);
    }
}
