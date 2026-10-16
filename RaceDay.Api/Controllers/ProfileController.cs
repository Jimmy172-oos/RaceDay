using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/profile (Pass userId via query string for testing)
        [HttpGet("api/profile")]
        public async Task<ActionResult<User>> GetProfile([FromQuery] int userId)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound();

            return user;
        }

        // PUT: /api/profile
        [HttpPut("api/profile")]
        public async Task<IActionResult> UpdateProfile([FromQuery] int userId, [FromBody] User updatedUserData)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.FullName = updatedUserData.FullName;
            user.Email = updatedUserData.Email;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // GET: /api/profile/count
        [HttpGet("api/profile/count")]
        public async Task<ActionResult<int>> GetUserCount()
        {
            int count = await _context.Users.CountAsync();
            return Ok(new { TotalUsers = count });
        }
    }
}