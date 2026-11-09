using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api/events/{eventId}/categories
        [HttpPost("events/{eventId}/categories")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> AddCategory(int eventId, [FromBody] Category categoryDto)
        {
            var eventExists = await _context.Events.AnyAsync(e => e.EventId == eventId);
            if (!eventExists) return NotFound(new { Message = "Event does not exist" });

            categoryDto.EventId = eventId;
            _context.Categories.Add(categoryDto);
            await _context.SaveChangesAsync();

            return StatusCode(201, categoryDto);
        }

        // PUT: /api/categories/{id}
        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.CategoryName = categoryDto.CategoryName;
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // DELETE: /api/categories/{id}
        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Category deleted successfully" });
        }

        // POST: /api/categories/{categoryId}/enroll
        [HttpPost("categories/{categoryId}/enroll")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> EnrollInCategory(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return NotFound(new { Message = "Category does not exist" });

            // Get UserId from the JWT Token (We will set this up in Step 1.3)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int participantId = int.Parse(userIdClaim);

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CategoryId == categoryId && e.ParticipantId == participantId);

            if (existingEnrollment != null)
                return Conflict(new { Message = "Already enrolled in this category" });

            var enrollment = new Enrollment
            {
                CategoryId = categoryId,
                ParticipantId = participantId,
                Status = "Pending",
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return StatusCode(201, enrollment);
        }
    }
}