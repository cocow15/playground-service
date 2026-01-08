namespace AuthService.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } // in seconds
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
