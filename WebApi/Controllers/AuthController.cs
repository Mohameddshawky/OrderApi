using System;
using Application.DTOs;
using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthController(IUserRepository userRepository, IConfiguration configuration, AppDbContext context)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { Message = "Email already registered." });
        }

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User"
        };

        await _userRepository.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid credentials." });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        user.RefreshToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new { Token = tokenHandler.WriteToken(token), RefreshToken = user.RefreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Unauthorized(new { Message = "Invalid or expired refresh token." });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        user.RefreshToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new { Token = tokenHandler.WriteToken(token), RefreshToken = user.RefreshToken });
    }
}
