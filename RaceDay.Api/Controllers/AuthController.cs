using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenService _jwtService;

        public AuthController(ApplicationDbContext context, JwtTokenService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            if (await _context.Users.AnyAsync(u => u.Email == newUser.Email))
                return BadRequest(new { Message = "Email already in use" });

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { Message = "User registered successfully", newUser.UserId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.PasswordHash, user.PasswordHash))
                return Unauthorized(new { Message = "Invalid credentials" });

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Role = user.Role
            });
        }
    }
}