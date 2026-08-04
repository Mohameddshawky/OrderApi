namespace Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal OriginalItemTotal { get; set; }
    public string? CouponCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public IEnumerable<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}
