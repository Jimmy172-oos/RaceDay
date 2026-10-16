using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/enrolments/my (Pass participantId via query string temporarily for testing)
        [HttpGet("api/enrolments/my")]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetMyEnrolments([FromQuery] int participantId)
        {
            return await _context.Enrollments
                .Include(e => e.Category)
                .ThenInclude(c => c!.Event)
                .Where(e => e.ParticipantId == participantId)
                .ToListAsync();
        }

        // DELETE: /api/enrolments/{id}
        [HttpDelete("api/enrolments/{id}")]
        public async Task<IActionResult> CancelEnrolment(int id)
        {
            var enrolment = await _context.Enrollments.FindAsync(id);
            if (enrolment == null) return NotFound();

            _context.Enrollments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Enrolment cancelled successfully" });
        }

        // POST: /api/enrolments/{enrolmentId}/result
        [HttpPost("api/enrolments/{enrolmentId}/result")]
        public async Task<ActionResult<Result>> CaptureResult(int enrolmentId, [FromBody] Result resultDto)
        {
            var enrolment = await _context.Enrollments.FindAsync(enrolmentId);
            if (enrolment == null) return NotFound(new { Message = "Enrolment does not exist" });

            resultDto.EnrollmentId = enrolmentId;
            _context.Results.Add(resultDto);
            await _context.SaveChangesAsync();

            return Created("", resultDto);
        }
    }
}