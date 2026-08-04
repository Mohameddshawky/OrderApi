using Application.DTOs;

namespace Application.Interfaces;

public interface IReportService
{
    Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(int count = 5);
    Task<IEnumerable<BestSellingProductDto>> GetBestSellingProductsAsync();
}
