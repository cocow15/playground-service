using AuthService.DTOs;
using AuthService.Models;

namespace AuthService.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(int userId);
    int? ValidateAccessToken(string token);
}
