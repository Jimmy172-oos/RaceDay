using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
            {
                return BadRequest(new { Message = "Email already in use" });
            }

            // Simple hash placeholder (In production, use BCrypt or ASP.NET Identity)
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { Message = "User registered successfully", newUser.UserId });
        }

        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.PasswordHash, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Invalid credentials" });
            }

            // Normally you would generate and return a JWT Token here.
            return Ok(new { Message = "Login successful", Token = "mock-jwt-token", user.UserId, user.Role });
        }
    }
}