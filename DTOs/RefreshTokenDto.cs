using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
