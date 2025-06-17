// TODO: Implement EventsController endpoints

using BrawlBuddy.Api.Models;
using BrawlBuddy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrawlBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IBrawlApiService _brawlApiService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IBrawlApiService brawlApiService, ILogger<EventsController> logger)
    {
        _brawlApiService = brawlApiService;
        _logger = logger;
    }    /// <summary>
    /// Get map and event rotations
    /// </summary>
    /// <returns>Event rotation data</returns>
    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        try
        {
            _logger.LogInformation("Fetching event rotation data");

            var eventRotation = await _brawlApiService.GetEventRotationAsync();

            if (eventRotation == null)
            {
                _logger.LogWarning("Event rotation not found, returning mock data");
                
                // Return mock event data similar to Brawlify
                var mockEvents = new
                {
                    events = new[]
                    {
                        new
                        {
                            eventName = "Solo Showdown",
                            mapName = "Acid Lakes",
                            gameMode = "Solo Showdown",
                            timeLeft = "10h 17m",
                            topBrawlers = new[]
                            {
                                new { brawlerId = 16000011, name = "Edgar", winRate = 55.0, useRate = 0.7 },
                                new { brawlerId = 16000000, name = "Shelly", winRate = 55.0, useRate = 1.0 },
                                new { brawlerId = 16000007, name = "Leon", winRate = 55.0, useRate = 0.4 },
                                new { brawlerId = 16000001, name = "Bull", winRate = 52.0, useRate = 0.5 },
                                new { brawlerId = 16000018, name = "Rosa", winRate = 50.0, useRate = 6.7 }
                            }
                        },
                        new
                        {
                            eventName = "Brawl Ball",
                            mapName = "Spiraling Out",
                            gameMode = "Brawl Ball",
                            timeLeft = "22h 17m",
                            topBrawlers = new[]
                            {
                                new { brawlerId = 16000008, name = "Mortis", winRate = 73.0, useRate = 0.1 },
                                new { brawlerId = 16000004, name = "Frank", winRate = 69.0, useRate = 0.1 },
                                new { brawlerId = 16000003, name = "El Primo", winRate = 63.0, useRate = 0.1 },
                                new { brawlerId = 16000020, name = "Darryl", winRate = 61.0, useRate = 0.1 },
                                new { brawlerId = 16000016, name = "Bibi", winRate = 61.0, useRate = 0.6 }
                            }
                        },
                        new
                        {
                            eventName = "Gem Grab",
                            mapName = "On A Roll",
                            gameMode = "Gem Grab",
                            timeLeft = "16h 17m",
                            topBrawlers = new[]
                            {
                                new { brawlerId = 16000009, name = "Tara", winRate = 58.0, useRate = 0.3 },
                                new { brawlerId = 16000010, name = "Gene", winRate = 57.0, useRate = 1.9 },
                                new { brawlerId = 16000006, name = "Poco", winRate = 56.0, useRate = 3.1 },
                                new { brawlerId = 16000013, name = "Pam", winRate = 55.0, useRate = 1.1 },
                                new { brawlerId = 16000017, name = "Byron", winRate = 54.0, useRate = 1.2 }
                            }
                        }
                    },
                    dataSource = "Mock Data",
                    timestamp = DateTime.UtcNow
                };
                
                return Ok(mockEvents);
            }

            _logger.LogInformation("Successfully retrieved event rotation data");
            return Ok(eventRotation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event rotation data");
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}