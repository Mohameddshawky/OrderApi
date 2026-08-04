using Application.DTOs;
using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(int count)
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .GroupBy(o => new { o.CustomerId, o.Customer.Name })
            .Select(g => new TopCustomerDto {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.Name,
                TotalSpending = g.Sum(o => o.TotalAmount)
            })
            .OrderByDescending(c => c.TotalSpending)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<BestSellingProductDto>> GetBestSellingProductsAsync()
    {
        return await _context.OrderItems
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new BestSellingProductDto {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                TotalQuantitySold = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(p => p.TotalQuantitySold)
            .ToListAsync();
    }
}
