using System.IdentityModel.Tokens.Jwt; // JWT işlemleri için şart
using System.Security.Claims;          // Kimlik bilgileri (Claim) için şart
using System.Text;                     // Encoding için şart
using CtiHub.Application.Common.Interfaces;
using CtiHub.Application.DTOs;
using CtiHub.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;  // Şifreleme algoritmaları için şart

namespace CtiHub.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IGenericRepository<User> userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        // 1. Kullanıcıyı bul
        var users = await _userRepository.FindAsync(u => u.Email == request.Email);
        var user = users.FirstOrDefault();

        // 2. Kontrol et
        if (user == null)
        {
            return Unauthorized(new { message = "Email veya şifre hatalı!" });
        }

        if (user.PasswordHash != request.Password)
        {
            return Unauthorized(new { message = "Email veya şifre hatalı!" });
        }

        // 3. Token oluştur
        var token = GenerateJwtToken(user);

        return Ok(new { token = token, message = "Giriş başarılı!" });
    }

    // Token Üretme Motoru
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        // Ünlem işareti (!) null olmayacağını garanti eder
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

        // BURADAKİ HATA DÜZELTİLDİ: "newList" -> "new List"
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            // İsim null gelirse boş string ("") kullan
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }
}