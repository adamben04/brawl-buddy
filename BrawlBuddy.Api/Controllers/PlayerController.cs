// TODO: Implement PlayerController endpoints

using BrawlBuddy.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BrawlBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    // Test endpoint
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "Backend is working!", timestamp = DateTime.Now });
    }
    
    // TODO: Get player profile data
    // GET /api/player/{tag}
    
    // TODO: Get player battle logs
    // GET /api/player/{tag}/battles
}