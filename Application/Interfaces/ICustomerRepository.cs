using Core.Entities;

namespace Application.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<IEnumerable<Order>> GetOrderHistoryAsync(int customerId);
    Task<IEnumerable<Customer>> GetAllIncludingInactiveAsync();
    Task<Customer?> GetByIdIncludingInactiveAsync(int id);
}
