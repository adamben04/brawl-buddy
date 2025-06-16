// Meta Controller - Handles tier lists and meta analysis endpoints

using BrawlBuddy.Api.Models;
using BrawlBuddy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrawlBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetaController : ControllerBase
{
    private readonly BrawlApiService _brawlApiService;
    private readonly ILogger<MetaController> _logger;

    public MetaController(BrawlApiService brawlApiService, ILogger<MetaController> logger)
    {
        _brawlApiService = brawlApiService;
        _logger = logger;
    }

    /// <summary>
    /// Get tier list for a specific game mode
    /// </summary>
    /// <param name="mode">Game mode (gemgrab, heist, bounty, siege, brawlball, hotzone, knockout)</param>
    /// <returns>Tier list for the specified mode</returns>
    [HttpGet("tiers")]
    public async Task<IActionResult> GetTierList([FromQuery] string? mode = null)
    {
        try
        {
            _logger.LogInformation("Fetching tier list for mode: {Mode}", mode ?? "all");
            
            // Get all brawlers first
            var brawlers = await _brawlApiService.GetBrawlersAsync();
            
            if (brawlers == null || !brawlers.Any())
            {
                _logger.LogWarning("No brawlers data available for tier list");
                return NotFound(new { message = "Brawlers data not available" });
            }

            // Create a mock tier list (in real implementation, this would come from analytics)
            var tierList = CreateMockTierList(brawlers, mode);
            
            _logger.LogInformation("Successfully generated tier list for mode: {Mode}", mode ?? "all");
            return Ok(new { 
                mode = mode ?? "all", 
                tierList, 
                lastUpdated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                totalBrawlers = brawlers.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating tier list for mode: {Mode}", mode);
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get current meta statistics
    /// </summary>
    /// <returns>Meta statistics including pick rates and win rates</returns>
    [HttpGet("stats")]
    public async Task<IActionResult> GetMetaStats()
    {
        try
        {
            _logger.LogInformation("Fetching meta statistics");
            
            var brawlers = await _brawlApiService.GetBrawlersAsync();
            
            if (brawlers == null || !brawlers.Any())
            {
                _logger.LogWarning("No brawlers data available for meta stats");
                return NotFound(new { message = "Brawlers data not available" });
            }

            // Create mock meta statistics
            var metaStats = CreateMockMetaStats(brawlers);
            
            _logger.LogInformation("Successfully generated meta statistics");
            return Ok(metaStats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating meta statistics");
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    private object CreateMockTierList(List<Brawler> brawlers, string? mode)
    {
        // Simple mock tier assignment based on brawler names/types
        var random = new Random(42); // Fixed seed for consistent results
        
        var tierList = new
        {
            S = brawlers.Take(6).Select(b => new { 
                id = b.Id, 
                name = b.Name, 
                winRate = Math.Round(75 + random.NextDouble() * 15, 1),
                pickRate = Math.Round(8 + random.NextDouble() * 12, 1)
            }).ToList(),
            A = brawlers.Skip(6).Take(10).Select(b => new { 
                id = b.Id, 
                name = b.Name, 
                winRate = Math.Round(65 + random.NextDouble() * 10, 1),
                pickRate = Math.Round(5 + random.NextDouble() * 8, 1)
            }).ToList(),
            B = brawlers.Skip(16).Take(12).Select(b => new { 
                id = b.Id, 
                name = b.Name, 
                winRate = Math.Round(50 + random.NextDouble() * 15, 1),
                pickRate = Math.Round(2 + random.NextDouble() * 6, 1)
            }).ToList(),
            C = brawlers.Skip(28).Select(b => new { 
                id = b.Id, 
                name = b.Name, 
                winRate = Math.Round(35 + random.NextDouble() * 20, 1),
                pickRate = Math.Round(0.5 + random.NextDouble() * 3, 1)
            }).ToList()
        };

        return tierList;
    }

    private object CreateMockMetaStats(List<Brawler> brawlers)
    {
        var random = new Random(42);
        
        return new
        {
            totalMatches = 1250000,
            lastUpdated = DateTime.UtcNow.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            topBrawlers = brawlers.Take(10).Select(b => new {
                id = b.Id,
                name = b.Name,
                pickRate = Math.Round(5 + random.NextDouble() * 15, 1),
                winRate = Math.Round(45 + random.NextDouble() * 25, 1),
                banRate = Math.Round(random.NextDouble() * 10, 1)
            }).ToList(),
            gameModeMeta = new
            {
                gemGrab = new { avgMatchLength = "2:45", mostPicked = "Poco" },
                brawlBall = new { avgMatchLength = "1:30", mostPicked = "Mortis" },
                heist = new { avgMatchLength = "2:15", mostPicked = "Colt" },
                bounty = new { avgMatchLength = "3:20", mostPicked = "Piper" },
                siege = new { avgMatchLength = "4:10", mostPicked = "Jessie" },
                hotZone = new { avgMatchLength = "2:55", mostPicked = "Rosa" },
                knockout = new { avgMatchLength = "1:45", mostPicked = "Edgar" }
            }
        };
    }
}