using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IOrderStatusHistoryService _historyService;

    public OrdersController(IOrderService orderService, IOrderStatusHistoryService historyService)
    {
        _orderService = orderService;
        _historyService = historyService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetAll([FromQuery] OrderQueryParameters parameters)
    {
        var result = await _orderService.GetOrdersAsync(parameters);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderWithDetailsAsync(id);
        return Ok(order);
    }
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto createOrderDto)
    {
        var created = await _orderService.CreateOrderAsync(createOrderDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/ship")]
    public async Task<IActionResult> Ship(int id)
    {
        await _orderService.ShipOrderAsync(id);
        return NoContent();
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/deliver")]
    public async Task<IActionResult> Deliver(int id)
    {
        await _orderService.DeliverOrderAsync(id);
        return NoContent();
    }
    [Authorize]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        await _orderService.CancelOrderAsync(id);
        return NoContent();
    }

    [HttpGet("test-error")]
    public IActionResult TestError() => throw new Exception("This is a test error");

    [HttpGet("{id}/history")]
    public async Task<ActionResult<IEnumerable<OrderStatusHistoryDto>>> GetHistory(int id)
    {
        var history = await _historyService.GetOrderHistoryAsync(id);
        return Ok(history);
    }
}
