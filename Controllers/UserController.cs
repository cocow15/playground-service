using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints in this controller require authentication
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile - Demonstrates JWT authorization
    /// </summary>
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        // Extract user information from JWT claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = "Invalid token claims" });
        }

        _logger.LogInformation("User profile accessed by: {Username}", username);

        return Ok(new
        {
            userId = userId,
            username = username,
            email = email,
            message = "This is a protected endpoint - you are authenticated!"
        });
    }

    /// <summary>
    /// Test endpoint - Another example of protected endpoint
    /// </summary>
    [HttpGet("test")]
    public IActionResult TestEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        
        _logger.LogInformation("Test endpoint accessed by: {Username}", username);

        return Ok(new
        {
            message = "You have successfully accessed a protected endpoint!",
            username = username,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get all user claims - Shows all JWT token claims
    /// </summary>
    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new
        {
            type = c.Type,
            value = c.Value
        });

        var username = User.Identity?.Name ?? "Unknown";
        _logger.LogInformation("Claims accessed by: {Username}", username);

        return Ok(new
        {
            message = "All JWT claims from your token",
            claims = claims
        });
    }
}
