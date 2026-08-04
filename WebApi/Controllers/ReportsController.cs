using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("top-customers")]
    public async Task<ActionResult<IEnumerable<TopCustomerDto>>> GetTopCustomers()
    {
        var topCustomers = await _reportService.GetTopCustomersAsync();
        return Ok(topCustomers);
    }

    [HttpGet("best-selling-products")]
    public async Task<ActionResult<IEnumerable<BestSellingProductDto>>> GetBestSellingProducts()
    {
        var bestSellers = await _reportService.GetBestSellingProductsAsync();
        return Ok(bestSellers);
    }
}
