using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // PUT: /api/categories/{id}
        [HttpPut("api/categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.CategoryName = categoryDto.CategoryName;
            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // DELETE: /api/categories/{id}
        [HttpDelete("api/categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Category deleted successfully" });
        }

        // POST: /api/categories/{categoryId}/enrol
        [HttpPost("api/categories/{categoryId}/enrol")]
        public async Task<IActionResult> EnrolInCategory(int categoryId, [FromQuery] int participantId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return NotFound(new { Message = "Category does not exist" });

            // Check if already enrolled
            var existingEnrolment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CategoryId == categoryId && e.ParticipantId == participantId);

            if (existingEnrolment != null)
                return Conflict(new { Message = "Already enrolled in this category" });

            var enrolment = new Enrollment
            {
                CategoryId = categoryId,
                ParticipantId = participantId,
                Status = "Pending",
                EnrollmentDate = DateTime.Now
            };

            _context.Enrollments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Created("", enrolment);
        }
    }
}