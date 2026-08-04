using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;


namespace Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(bool includeInactive = false)
    {
        var customers = includeInactive ? await _repository.GetAllIncludingInactiveAsync() : await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        return customer == null ? null : _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto> AddAsync(CustomerDto customerDto)
    {
        var customer = _mapper.Map<Customer>(customerDto);
        await _repository.AddAsync(customer);
        await _repository.SaveChangesAsync();
        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task RemoveAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer != null)
        {
            customer.IsActive = false;
            await _repository.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(int customerId)
    {
        var orders = await _repository.GetOrderHistoryAsync(customerId);
        return _mapper.Map<IEnumerable<OrderDto>>(orders);
    }
}
