using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RaceController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRaces()
    {
        return Ok(new[] { "Race 1", "Race 2" });
    }
}