# Panduan Menggunakan Access Token di Service Lain

Dokumen ini menjelaskan langkah-langkah untuk mengonfigurasi service backend lain (khususnya .NET) agar dapat memvalidasi access token yang diterbitkan oleh `auth-service`.

## 1. Persiapan Konfigurasi

Pastikan Anda memiliki informasi berikut dari `auth-service` (lihat `appsettings.json` di `auth-service`):
- **SecretKey**: Kunci rahasia yang sama persis dengan yang digunakan di `auth-service`.
- **Issuer**: Penerbit token (misal: `AuthService`).
- **Audience**: Penerima token (misal: `AuthServiceUser`).

## 2. Konfigurasi `appsettings.json`

Tambahkan konfigurasi JWT yang sama di `appsettings.json` pada service yang ingin menggunakan token ini.

```json
{
  "Jwt": {
    "SecretKey": "PASTIKAN_KEY_INI_SAMA_PERSIS_DENGAN_AUTH_SERVICE",
    "Issuer": "AuthService",
    "Audience": "AuthServiceUser"
  }
}
```

## 3. Konfigurasi `Program.cs`

Install package NuGet yang diperlukan:
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

Tambahkan kode berikut di `Program.cs` service Anda:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ... konfigurasi lainnya ...

// 1. Ambil konfigurasi JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];
var key = Encoding.UTF8.GetBytes(secretKey);

// 2. Tambahkan Authentication Service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set true untuk production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Sinkronkan toleransi waktu dengan auth-service
    };
});

var app = builder.Build();

// ... middleware lainnya ...

// 3. Aktifkan Middleware Authentication & Authorization
// PENTING: Urutan ini harus benar (sebelum MapControllers)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 4. Cara Penggunaan di Controller

Gunakan attribute `[Authorize]` pada controller atau action yang ingin dilindungi.

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Lindungi seluruh controller
public class ProductController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProducts()
    {
        // Bisa mengambil user ID dari claims jika perlu
        var userId = User.FindFirst("id")?.Value;
        var username = User.Identity?.Name;
        
        return Ok(new { message = $"Authenticated as {username}" });
    }
    
    [HttpGet("public")]
    [AllowAnonymous] // Buka akses untuk endpoint tertentu
    public IActionResult GetPublicInfo()
    {
        return Ok(new { message = "Public data" });
    }
}
```

## Ringkasan
1.  Copy `SecretKey`, `Issuer`, dan `Audience` dari `auth-service`.
2.  Pasang di `appsettings.json` service tujuan.
3.  Setup `AddAuthentication` dan `AddJwtBearer` di `Program.cs`.
4.  Gunakan `app.UseAuthentication()` dan `app.UseAuthorization()`.
5.  Lindungi endpoint dengan `[Authorize]`.
