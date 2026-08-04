namespace Application.DTOs;

public class TopCustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalSpending { get; set; }
}
