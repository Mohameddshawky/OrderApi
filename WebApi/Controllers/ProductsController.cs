using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var products = await _productService.GetAllAsync(includeInactive);
        return Ok(products);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock()
    {
        var products = await _productService.GetLowStockProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();
            
        return Ok(product);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto createProductDto)
    {
        var created = await _productService.AddAsync(createProductDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductDto productDto)
    {
        try
        {
            await _productService.UpdateAsync(id, productDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.RemoveAsync(id);
        return NoContent();
    }
}
