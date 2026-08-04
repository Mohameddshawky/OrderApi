using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(bool includeInactive = false);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> AddAsync(CreateProductDto createProductDto);
    Task UpdateAsync(int id, ProductDto productDto);
    Task RemoveAsync(int id);
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10);
}
