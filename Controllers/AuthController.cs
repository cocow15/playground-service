using Microsoft.AspNetCore.Mvc;
using AuthService.Services;
using AuthService.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Validation failed", details = ModelState });
        }

        var result = await _authService.RegisterAsync(registerDto);

        if (result == null)
        {
            _logger.LogWarning("Registration failed for username: {Username}", registerDto.Username);
            return BadRequest(new { error = "Username or email already exists" });
        }

        _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);
        return CreatedAtAction(nameof(Register), new { username = registerDto.Username }, result);
    }

    /// <summary>
    /// Login and receive JWT tokens
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Validation failed", details = ModelState });
        }

        var result = await _authService.LoginAsync(loginDto);

        if (result == null)
        {
            _logger.LogWarning("Login failed for user: {UsernameOrEmail}", loginDto.UsernameOrEmail);
            return Unauthorized(new { error = "Invalid credentials or account is locked" });
        }

        _logger.LogInformation("User logged in successfully: {Username}", result.Username);
        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Validation failed", details = ModelState });
        }

        var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

        if (result == null)
        {
            _logger.LogWarning("Token refresh failed with invalid refresh token");
            return Unauthorized(new { error = "Invalid or expired refresh token" });
        }

        _logger.LogInformation("Token refreshed successfully for user: {Username}", result.Username);
        return Ok(result);
    }

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Validation failed", details = ModelState });
        }

        var result = await _authService.RevokeTokenAsync(refreshTokenDto.RefreshToken);

        if (!result)
        {
            return BadRequest(new { error = "Token revocation failed" });
        }

        var username = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("Token revoked successfully for user: {Username}", username);
        return Ok(new { message = "Token revoked successfully" });
    }
}
