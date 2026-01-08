using AuthService.Data;
using AuthService.DTOs;
using AuthService.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    public AuthService(ApplicationDbContext context, ITokenService tokenService, IConfiguration configuration)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
        {
            return null; // Username already taken
        }

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            return null; // Email already registered
        }

        // Hash password using BCrypt with cost factor 12
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, workFactor: 12);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15") * 60,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        // Find user by username or email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == loginDto.UsernameOrEmail || u.Email == loginDto.UsernameOrEmail);

        if (user == null)
        {
            return null; // User not found
        }

        // Check if account is locked
        if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
        {
            return null; // Account is locked
        }

        // Reset lockout if time has passed
        if (user.LockedUntil.HasValue && user.LockedUntil.Value <= DateTime.UtcNow)
        {
            user.LockedUntil = null;
            user.FailedLoginAttempts = 0;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;
            user.UpdatedAt = DateTime.UtcNow;

            // Lock account if max attempts reached
            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
            }

            await _context.SaveChangesAsync();
            return null; // Invalid password
        }

        // Reset failed attempts on successful login
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15") * 60,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshTokenString)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenString);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return null; // Invalid or expired refresh token
        }

        // Generate new tokens
        var accessToken = _tokenService.GenerateAccessToken(refreshToken.User);
        var newRefreshToken = _tokenService.GenerateRefreshToken(refreshToken.User.Id);

        // Revoke old refresh token
        refreshToken.RevokedAt = DateTime.UtcNow;

        // Add new refresh token
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresIn = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15") * 60,
            Username = refreshToken.User.Username,
            Email = refreshToken.User.Email
        };
    }

    public async Task<bool> RevokeTokenAsync(string refreshTokenString)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenString);

        if (refreshToken == null || refreshToken.IsRevoked)
        {
            return false;
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}
