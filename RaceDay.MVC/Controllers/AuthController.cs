using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiClient _api;

        public AuthController(ApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Map 'password' to 'PasswordHash' to match the API's expected payload
            var response = await _api.PostAsync("auth/login", new { Email = email, PasswordHash = password });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonDocument.Parse(content).RootElement;

                // Save to Session
                HttpContext.Session.SetString("JwtToken", data.GetProperty("token").GetString()!);
                HttpContext.Session.SetString("UserName", data.GetProperty("fullName").GetString()!);
                HttpContext.Session.SetString("UserRole", data.GetProperty("role").GetString()!);
                HttpContext.Session.SetInt32("UserId", data.GetProperty("userId").GetInt32());

                // Redirect based on role
                if (data.GetProperty("role").GetString() == "Organizer")
                    return RedirectToAction("Dashboard", "Organizer");
                else
                    return RedirectToAction("Index", "Participant");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string role)
        {
            var response = await _api.PostAsync("auth/register", new
            {
                FullName = fullName,
                Email = email,
                PasswordHash = password,
                Role = role
            });

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Account created! Please log in.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Registration failed. Email might already be in use.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}