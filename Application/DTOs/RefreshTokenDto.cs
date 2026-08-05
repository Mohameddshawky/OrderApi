using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
