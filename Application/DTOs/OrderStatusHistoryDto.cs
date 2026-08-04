namespace Application.DTOs;

public class OrderStatusHistoryDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}
