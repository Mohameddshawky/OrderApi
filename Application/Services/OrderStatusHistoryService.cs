using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;

namespace Application.Services;

public class OrderStatusHistoryService : IOrderStatusHistoryService
{
    private readonly IOrderStatusHistoryRepository _repository;
    private readonly IMapper _mapper;

    public OrderStatusHistoryService(IOrderStatusHistoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task RecordStatusChangeAsync(int orderId, OrderStatus? oldStatus, OrderStatus newStatus)
    {
        var history = new OrderStatusHistory
        {
            OrderId = orderId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(history);
        await _repository.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderStatusHistoryDto>> GetOrderHistoryAsync(int orderId)
    {
        var history = await _repository.GetByOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<OrderStatusHistoryDto>>(history);
    }
}
