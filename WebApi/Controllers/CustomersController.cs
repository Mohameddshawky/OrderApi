using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var customers = await _customerService.GetAllAsync(includeInactive);
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
            return NotFound();
            
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CustomerDto customerDto)
    {
        var created = await _customerService.AddAsync(customerDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerService.RemoveAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/order-history")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderHistory(int id)
    {
        var orders = await _customerService.GetOrderHistoryAsync(id);
        return Ok(orders);
    }
}
