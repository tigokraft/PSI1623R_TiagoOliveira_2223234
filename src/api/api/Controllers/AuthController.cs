using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinSync.Data;
using FinSync.DTOs;
using FinSync.Models;
using FinSync.Utils;

namespace FinSync.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FinSyncContext _context;
    private readonly IConfiguration _config;

    public AuthController(FinSyncContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null || !PasswordHelper.VerifyPassword(user.PasswordHash, request.Password))
            return Unauthorized("Invalid credentials.");

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (_context.Users.Any(u => u.Username == request.Username))
            return Conflict("Username already exists.");
        
        var newUser = new User
        {
            Username = request.Username,
            PasswordHash = PasswordHelper.HashPassword(request.Password),
            Role = "user"
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        // Create default "goal" category for the new user
        var goalCategory = new Category
        {
            UserId = newUser.UserId,
            CategoryName = "goal",
            Color = "#25be4f"
        };

        _context.Categories.Add(goalCategory);
        _context.SaveChanges();

        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        var token = GenerateJwtToken(user);
        return Ok(new
        {
            token
        });
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null)
            return NotFound("User not found.");

        user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
        _context.SaveChanges();

        return NoContent();
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("userId", user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
