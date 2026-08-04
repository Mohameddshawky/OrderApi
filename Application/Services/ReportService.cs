using Application.DTOs;
using Application.Interfaces;

namespace Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<TopCustomerDto>> GetTopCustomersAsync(int count = 5)
    {
        return await _reportRepository.GetTopCustomersAsync(count);
    }

    public async Task<IEnumerable<BestSellingProductDto>> GetBestSellingProductsAsync()
    {
        return await _reportRepository.GetBestSellingProductsAsync();
    }
}
