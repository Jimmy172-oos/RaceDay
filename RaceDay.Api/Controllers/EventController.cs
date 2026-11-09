using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events
                .Include(e => e.Categories)
                .Include(e => e.Organizer)
                .ToListAsync();
        }

        // GET: api/events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Categories)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (ev == null) return NotFound();
            return ev;
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }

        // PUT: api/events/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            ev.EventName = updatedEvent.EventName;
            ev.Description = updatedEvent.Description;
            ev.EventDate = updatedEvent.EventDate;
            ev.Location = updatedEvent.Location;
            ev.Distance = updatedEvent.Distance;
            ev.EventType = updatedEvent.EventType;

            await _context.SaveChangesAsync();
            return Ok(ev);
        }

        // DELETE: api/events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Event deleted" });
        }

        // POST: api/events/5/banner
        [HttpPost("{id}/banner")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> UploadBanner(int id, IFormFile file, [FromServices] BlobStorageService blobService)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var imageUrl = await blobService.UploadFileAsync(file);
            ev.BannerUrl = imageUrl;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Banner uploaded successfully", Url = imageUrl });
        }

        // GET: api/events/5/enrollments
        // This is the new endpoint the Organizer needs to see who enrolled!
        [HttpGet("{id}/enrollments")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> GetEventEnrollments(int id)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Include(e => e.Result)
                .Where(e => e.Category != null && e.Category.EventId == id)
                .ToListAsync();

            return Ok(enrollments);
        }
    }
}