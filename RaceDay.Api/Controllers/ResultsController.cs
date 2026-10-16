using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/results/my
        [HttpGet("api/results/my")]
        public async Task<ActionResult<IEnumerable<Result>>> GetMyResults([FromQuery] int participantId)
        {
            return await _context.Results
                .Include(r => r.Enrollment)
                .ThenInclude(e => e!.Category)
                .ThenInclude(c => c!.Event)
                .Where(r => r.Enrollment != null && r.Enrollment.ParticipantId == participantId)
                .ToListAsync();
        }

        // PUT: /api/results/{id}
        [HttpPut("api/results/{id}")]
        public async Task<IActionResult> UpdateResult(int id, [FromBody] Result resultDto)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null) return NotFound();

            result.FinishTime = resultDto.FinishTime;
            result.Position = resultDto.Position;
            result.Status = resultDto.Status;
            result.Notes = resultDto.Notes;

            await _context.SaveChangesAsync();

            return Ok(result);
        }
    }
}