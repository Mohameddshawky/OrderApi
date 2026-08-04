using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(bool includeInactive = false)
    {
        var products = includeInactive ? await _repository.GetAllIncludingInactiveAsync() : await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> AddAsync(CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        await _repository.AddAsync(product);
        await _repository.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task UpdateAsync(int id, ProductDto productDto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Product with ID {id} not found.");

        _mapper.Map(productDto, existing);
        existing.Id = id; // Ensure ID is not overwritten by DTO if it was wrong
        
        await _repository.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product != null)
        {
            product.IsActive = false;
            await _repository.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold = 10)
    {
        var products = await _repository.GetLowStockProductsAsync(threshold);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
