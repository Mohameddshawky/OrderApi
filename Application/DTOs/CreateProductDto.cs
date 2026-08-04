using System.ComponentModel.DataAnnotations;
using Application.ValidationAttributes;

namespace Application.DTOs;

public class CreateProductDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [EvenNumber]
    public int TestNumber { get; set; }
}
