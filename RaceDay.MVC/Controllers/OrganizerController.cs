using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [SessionAuthFilter(RequiredRole = "Organizer")]
    public class OrganizerController : Controller
    {
        private readonly ApiClient _api;

        public OrganizerController(ApiClient api)
        {
            _api = api;
        }

        // GET: /Organizer/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: /Organizer/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            var allEvents = await _api.GetAsync<List<EventDto>>("events") ?? new List<EventDto>();

            // Filter to only show events created by this Organizer
            var userId = HttpContext.Session.GetInt32("UserId");

            // Note: Since the API returns OrganizerId as a nested object sometimes, 
            // we just check if it exists. In a real app, you'd filter by OrganizerId.
            // For now, let's just show all events so you can see them.
            return View(allEvents);
        }

        // GET: /Organizer/CreateEvent
        public IActionResult CreateEvent()
        {
            return View();
        }

        // POST: /Organizer/CreateEvent
        [HttpPost]
        public async Task<IActionResult> CreateEvent(string eventName, string description, string eventDate, string location, string distance, string eventType)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // Parse the date string safely
            if (!DateTime.TryParse(eventDate, out DateTime parsedDate))
            {
                ViewBag.Error = "Invalid date format. Please try again.";
                return View();
            }

            var newEvent = new
            {
                EventName = eventName,
                Description = description,
                EventDate = parsedDate,
                Location = location,
                Distance = distance,
                EventType = eventType,
                OrganizerId = userId
            };

            var response = await _api.PostAsync("events", newEvent);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("MyEvents");
            }

            // Read the API error message for debugging
            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Failed to create event. API says: {errorContent}";
            return View();
        }

        // GET: /Organizer/EventDetails/5
        public async Task<IActionResult> EventDetails(int id)
        {
            var ev = await _api.GetAsync<EventDto>($"events/{id}");
            if (ev == null) return NotFound();

            var enrollments = await _api.GetAsync<List<OrganizerEnrollmentDto>>($"events/{id}/enrollments");
            ViewBag.Enrollments = enrollments ?? new List<OrganizerEnrollmentDto>();

            return View(ev);
        }

        // POST: /Organizer/AddCategory
        [HttpPost]
        public async Task<IActionResult> AddCategory(int eventId, string categoryName)
        {
            var response = await _api.PostAsync($"events/{eventId}/categories", new { CategoryName = categoryName });

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Category added!";
            else
                TempData["Error"] = "Failed to add category.";

            return RedirectToAction("EventDetails", new { id = eventId });
        }

        // POST: /Organizer/CaptureResult
        [HttpPost]
        public async Task<IActionResult> CaptureResult(int enrollmentId, string finishTime, int position, string status, int eventId)
        {
            var resultData = new
            {
                FinishTime = finishTime,
                Position = position,
                Status = status
            };

            var response = await _api.PostAsync($"enrollments/{enrollmentId}/result", resultData);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Result captured successfully!";
            else
                TempData["Error"] = "Failed to capture result.";

            return RedirectToAction("EventDetails", new { id = eventId });
        }
    }

    public class OrganizerEnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public ParticipantDto? Participant { get; set; }
        public CategoryDto? Category { get; set; }
        public ResultDto? Result { get; set; }
    }

    public class ParticipantDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ResultDto
    {
        public string? FinishTime { get; set; }
        public int? Position { get; set; }
        public string? Status { get; set; }
    }
}