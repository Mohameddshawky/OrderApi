namespace Application.DTOs;

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public string? CouponCode { get; set; }
    public IEnumerable<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
