using Core.Entities;

namespace Application.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);
    Task<IEnumerable<Product>> GetAllIncludingInactiveAsync();
    Task<Product?> GetByIdIncludingInactiveAsync(int id);
}
