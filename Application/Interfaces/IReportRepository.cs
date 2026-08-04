using Application.DTOs;

namespace Application.Interfaces;

public interface IReportRepository
{
    Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(int count);
    Task<IEnumerable<BestSellingProductDto>> GetBestSellingProductsAsync();
}
