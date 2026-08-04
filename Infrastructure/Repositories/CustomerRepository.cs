using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrderHistoryAsync(int customerId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllIncludingInactiveAsync()
    {
        return await _dbSet.IgnoreQueryFilters().ToListAsync();
    }

    public async Task<Customer?> GetByIdIncludingInactiveAsync(int id)
    {
        return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
    }
}
