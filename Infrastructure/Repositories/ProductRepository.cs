using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _dbSet
            .Where(p => p.StockQuantity < threshold)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllIncludingInactiveAsync()
    {
        return await _dbSet.IgnoreQueryFilters().ToListAsync();
    }

    public async Task<Product?> GetByIdIncludingInactiveAsync(int id)
    {
        return await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
    }
}
