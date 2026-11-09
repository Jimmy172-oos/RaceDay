using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    [SessionAuthFilter(RequiredRole = "Participant")]
    public class ParticipantController : Controller
    {
        private readonly ApiClient _api;

        public ParticipantController(ApiClient api)
        {
            _api = api;
        }

        // GET: /Participant
        public async Task<IActionResult> Index()
        {
            var events = await _api.GetAsync<List<EventDto>>("events");
            return View(events ?? new List<EventDto>());
        }

        // GET: /Participant/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _api.GetAsync<EventDto>($"events/{id}");
            if (ev == null) return NotFound();

            var myEnrollments = await _api.GetAsync<List<EnrollmentDto>>("enrollments/my");
            ViewBag.AlreadyEnrolled = myEnrollments?.Any(e => e.CategoryId == id) ?? false;

            return View(ev);
        }

        // POST: /Participant/Enroll
        [HttpPost]
        public async Task<IActionResult> Enroll(int categoryId)
        {
            var response = await _api.PostAsync($"categories/{categoryId}/enroll", new { });

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Successfully enrolled in this category!";
            else
                TempData["Error"] = "Could not enroll. You may already be enrolled.";

            return RedirectToAction("MyEnrollments");
        }

        // GET: /Participant/MyEnrollments
        public async Task<IActionResult> MyEnrollments()
        {
            var enrollments = await _api.GetAsync<List<MyEnrollmentDto>>("enrollments/my");
            return View(enrollments ?? new List<MyEnrollmentDto>());
        }

        // GET: /Participant/MyResults
        public async Task<IActionResult> MyResults()
        {
            var results = await _api.GetAsync<List<MyResultDto>>("results/my");
            return View(results ?? new List<MyResultDto>());
        }

        // GET: /Participant/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var profile = await _api.GetAsync<UserProfileDto>($"profile?userId={userId}");
            return View(profile);
        }
    }

    // Data Transfer Objects (DTOs) to match the API response
    public class EventDto
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string? BannerUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MyEnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public CategoryDto? Category { get; set; }
    }

    public class MyResultDto
    {
        public int ResultId { get; set; }
        public string? FinishTime { get; set; }
        public int? Position { get; set; }
        public string Status { get; set; } = string.Empty;
        public EnrollmentDetailDto? Enrollment { get; set; }
    }

    public class EnrollmentDetailDto
    {
        public CategoryDto? Category { get; set; }
    }

    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ProfileDetailDto? Profile { get; set; }
    }

    public class ProfileDetailDto
    {
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}