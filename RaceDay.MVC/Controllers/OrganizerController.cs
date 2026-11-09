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
            return View(allEvents);
        }

        // GET: /Organizer/CreateEvent
        public IActionResult CreateEvent()
        {
            return View();
        }

        // POST: /Organizer/CreateEvent
        [HttpPost]
        public async Task<IActionResult> CreateEvent(string eventName, string description, string eventDate, string location, string distance, string eventType, IFormFile? bannerFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

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
                // Read the newly created event ID from the API response
                var content = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonDocument.Parse(content).RootElement;

                // ASP.NET Core API returns camelCase by default (eventId)
                int newEventId = 0;
                if (data.TryGetProperty("eventId", out var idProp) || data.TryGetProperty("EventId", out idProp))
                {
                    newEventId = idProp.GetInt32();
                }

                // If a banner was uploaded, send it to the API
                if (bannerFile != null && bannerFile.Length > 0 && newEventId > 0)
                {
                    using var formContent = new MultipartFormDataContent();
                    var stream = bannerFile.OpenReadStream();
                    formContent.Add(new StreamContent(stream), "file", bannerFile.FileName);

                    var uploadResponse = await _api.PostFormAsync($"events/{newEventId}/banner", formContent);

                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "Event created, but banner upload failed. (Is Azurite running?)";
                        return RedirectToAction("MyEvents");
                    }
                }

                TempData["Success"] = "Event and banner created successfully!";
                return RedirectToAction("MyEvents");
            }

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