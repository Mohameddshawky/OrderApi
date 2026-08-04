using Application.DTOs;

namespace Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync(bool includeInactive = false);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> AddAsync(CustomerDto customerDto);
    Task RemoveAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(int customerId);
}
