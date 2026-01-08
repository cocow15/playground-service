# Auth Service - .NET 8 Authentication API

Production-ready authentication service built with ASP.NET Core 8 and PostgreSQL, featuring comprehensive security hardening for pen-testing readiness.

## 🔒 Security Features

- **JWT Authentication**: Secure token-based authentication with HS256 algorithm
- **BCrypt Password Hashing**: Cost factor 12 for strong password protection
- **Rate Limiting**: Prevents brute-force attacks on authentication endpoints
- **Account Lockout**: Temporary lockout after 5 failed login attempts (15 minutes)
- **Refresh Tokens**: Secure token refresh mechanism with 7-day expiration
- **Security Headers**: HSTS, X-Frame-Options, CSP, X-Content-Type-Options
- **Request Logging**: Comprehensive logging for security monitoring
- **Input Validation**: FluentValidation with strong password requirements
- **SQL Injection Prevention**: EF Core parameterized queries
- **CORS Configuration**: Properly configured for production use

## 📋 Prerequisites

- .NET 8 SDK
- PostgreSQL 12 or higher
- Database: `production` with schema `cep`

## 🚀 Quick Start

### 1. Configure Database

Update the connection string in `appsettings.json` if needed:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=production;Username=postgres;Password=Hac68488;Search Path=cep"
}
```

### 2. Run Migrations

```powershell
dotnet ef database update
```

### 3. Start the Application

```powershell
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7000`
- HTTP: `http://localhost:5000`
- Swagger: `https://localhost:7000/swagger`

## 📚 API Endpoints

### Authentication Endpoints

#### Register New User

```powershell
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "SecurePass123!"
}
```

**Response (201 Created):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresIn": 900,
  "username": "testuser",
  "email": "test@example.com"
}
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character (@$!%*?&)

#### Login

```powershell
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "testuser",
  "password": "SecurePass123!"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresIn": 900,
  "username": "testuser",
  "email": "test@example.com"
}
```

**Note:** Account will be locked for 15 minutes after 5 failed login attempts.

#### Refresh Token

```powershell
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "your-refresh-token-here"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "new-access-token",
  "refreshToken": "new-refresh-token",
  "expiresIn": 900,
  "username": "testuser",
  "email": "test@example.com"
}
```

#### Revoke Token

```powershell
POST /api/auth/revoke
Authorization: Bearer {access-token}
Content-Type: application/json

{
  "refreshToken": "refresh-token-to-revoke"
}
```

### Protected Endpoints (Require Authorization)

#### Get User Profile

```powershell
GET /api/user/profile
Authorization: Bearer {access-token}
```

**Response (200 OK):**
```json
{
  "userId": "1",
  "username": "testuser",
  "email": "test@example.com",
  "message": "This is a protected endpoint - you are authenticated!"
}
```

#### Test Endpoint

```powershell
GET /api/user/test
Authorization: Bearer {access-token}
```

#### Get All Claims

```powershell
GET /api/user/claims
Authorization: Bearer {access-token}
```

## 💡 Usage Examples (PowerShell)

### Complete Authentication Flow

```powershell
# 1. Register a new user
$registerBody = @{
    username = "demouser"
    email = "demo@example.com"
    password = "DemoPass123!"
} | ConvertTo-Json

$registerResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/register" `
    -Method Post `
    -ContentType "application/json" `
    -Body $registerBody

Write-Host "Registration successful!" -ForegroundColor Green
Write-Host "Access Token: $($registerResponse.accessToken)"

# 2. Login
$loginBody = @{
    usernameOrEmail = "demouser"
    password = "DemoPass123!"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" `
    -Method Post `
    -ContentType "application/json" `
    -Body $loginBody

$accessToken = $loginResponse.accessToken
$refreshToken = $loginResponse.refreshToken

Write-Host "Login successful!" -ForegroundColor Green

# 3. Access protected endpoint
$profileResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/user/profile" `
    -Method Get `
    -Headers @{ "Authorization" = "Bearer $accessToken" }

Write-Host "Profile data:" -ForegroundColor Green
$profileResponse | ConvertTo-Json

# 4. Refresh token (after 15 minutes or when needed)
$refreshBody = @{
    refreshToken = $refreshToken
} | ConvertTo-Json

$newTokens = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/refresh" `
    -Method Post `
    -ContentType "application/json" `
    -Body $refreshBody

Write-Host "Token refreshed successfully!" -ForegroundColor Green
$newAccessToken = $newTokens.accessToken
```

### Using in Your API

To use this auth service in your own API, simply add the `[Authorize]` attribute to your controllers or actions:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YourApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires valid JWT token
public class YourController : ControllerBase
{
    [HttpGet("data")]
    public IActionResult GetData()
    {
        // Extract user information from JWT claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        // Your business logic here
        return Ok(new { userId, username, email });
    }

    [HttpPost("create")]
    public IActionResult CreateData([FromBody] YourDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Create data for the authenticated user
        // ...
        
        return CreatedAtAction(nameof(GetData), new { id = 1 }, dto);
    }
}
```

## 🔧 Configuration

### JWT Settings

Edit `appsettings.json`:

```json
"Jwt": {
  "SecretKey": "Your-Secure-256-Bit-Secret-Key-Here",
  "Issuer": "AuthService",
  "Audience": "AuthServiceClients",
  "AccessTokenExpirationMinutes": "15",
  "RefreshTokenExpirationDays": "7"
}
```

**Important:** Change the `SecretKey` to a strong, random 256-bit key in production!

### Rate Limiting

Edit rate limits in `appsettings.json`:

```json
"IpRateLimiting": {
  "GeneralPolicyRules": [
    {
      "Endpoint": "POST:/api/auth/login",
      "Period": "5m",
      "Limit": 5
    }
  ]
}
```

### CORS

Edit allowed origins in `appsettings.json`:

```json
"Cors": {
  "AllowedOrigins": [
    "http://localhost:3000",
    "https://yourapp.com"
  ]
}
```

## 🧪 Testing

### Test Account Lockout

```powershell
# Attempt login 6 times with wrong password
for ($i = 1; $i -le 6; $i++) {
    Write-Host "Attempt $i"
    $body = @{
        usernameOrEmail = "testuser"
        password = "WrongPassword123!"
    } | ConvertTo-Json
    
    try {
        Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" `
            -Method Post `
            -ContentType "application/json" `
            -Body $body
    } catch {
        Write-Host "Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
    }
}
```

### Test Rate Limiting

```powershell
# Quickly send 10 requests to test rate limiting
for ($i = 1; $i -le 10; $i++) {
    try {
        Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" `
            -Method Post `
            -ContentType "application/json" `
            -Body $loginBody
        Write-Host "Request $i succeeded" -ForegroundColor Green
    } catch {
        Write-Host "Request $i blocked - Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Yellow
    }
}
```

## 🛡️ Security Checklist

- [x] JWT tokens with HS256 algorithm
- [x] Access token expiration: 15 minutes
- [x] Refresh token expiration: 7 days
- [x] BCrypt password hashing (cost factor 12)
- [x] Strong password requirements enforced
- [x] Rate limiting on auth endpoints
- [x] Account lockout after 5 failed attempts
- [x] Security headers (HSTS, CSP, X-Frame-Options, etc.)
- [x] Request logging for monitoring
- [x] Input validation with FluentValidation
- [x] SQL injection prevention (EF Core)
- [x] CORS properly configured
- [x] HTTPS enforcement

## 📝 Database Schema

### Users Table (cep.users)

| Column | Type | Description |
|--------|------|-------------|
| id | INTEGER | Primary key |
| username | VARCHAR(50) | Unique username |
| email | VARCHAR(255) |Unique email |
| password_hash | TEXT | BCrypt hashed password |
| failed_login_attempts | INTEGER | Failed login counter |
| locked_until | TIMESTAMP | Account lock expiration |
| created_at | TIMESTAMP | Record creation time |
| updated_at | TIMESTAMP | Last update time |

### Refresh Tokens Table (cep.refresh_tokens)

| Column | Type | Description |
|--------|------|-------------|
| id | INTEGER | Primary key |
| token | TEXT | Unique refresh token |
| user_id | INTEGER | Foreign key to users |
| expires_at | TIMESTAMP | Token expiration |
| created_at | TIMESTAMP | Token creation time |
| revoked_at | TIMESTAMP | Token revocation time (nullable) |

## 📖 Project Structure

```
auth-service/
├── Controllers/
│   ├── AuthController.cs      # Authentication endpoints
│   └── UserController.cs      # Protected user endpoints
├── Data/
│   └── ApplicationDbContext.cs # EF Core database context
├── DTOs/
│   ├── RegisterDto.cs          # Registration request
│   ├── LoginDto.cs             # Login request
│   ├── AuthResponseDto.cs      # Auth response with tokens
│   └── RefreshTokenDto.cs      # Refresh token request
├── Middleware/
│   ├── SecurityHeadersMiddleware.cs   # Security headers
│   └── RequestLoggingMiddleware.cs    # Request logging
├── Migrations/                 # EF Core migrations
├── Models/
│   ├── User.cs                 # User entity
│   └── RefreshToken.cs         # Refresh token entity
├── Services/
│   ├── IAuthService.cs         # Auth service interface
│   ├── AuthService.cs          # Auth implementation
│   ├── ITokenService.cs        # Token service interface
│   └── TokenService.cs         # JWT token generation
├── Program.cs                  # Application entry point
├── appsettings.json            # Configuration
└── README.md                   # This file
```

## 🚨 Important Security Notes

1. **Change JWT SecretKey**: Always use a strong, random 256-bit key in production
2. **Use HTTPS**: Never transmit tokens over HTTP in production
3. **Secure Database**: Use strong database credentials and limit access
4. **Monitor Logs**: Regularly check logs for suspicious activity
5. **Update Dependencies**: Keep all NuGet packages up to date
6. **Environment Variables**: Store sensitive configuration in environment variables, not in code

## 📄 License

MIT License - feel free to use this in your projects!
