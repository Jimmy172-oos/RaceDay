
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Inject the database context into the controller
        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            // Includes related Categories and Organizer info using Entity Framework
            return await _context.Events
                .Include(e => e.Categories)
                .Include(e => e.Organizer)
                .ToListAsync();
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent([FromBody] Event newEvent)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvents), new { id = newEvent.EventId }, newEvent);
        }
    }
}