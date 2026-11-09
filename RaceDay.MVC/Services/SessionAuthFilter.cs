using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Services
{
    public class SessionAuthFilter : ActionFilterAttribute
    {
        public string? RequiredRole { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var token = session.GetString("JwtToken");

            // Not logged in -> go to login
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Logged in, but wrong role (e.g. Organizer trying to view Participant pages)
            if (!string.IsNullOrEmpty(RequiredRole))
            {
                var userRole = session.GetString("UserRole");
                if (userRole != RequiredRole)
                {
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                }
            }
        }
    }
}