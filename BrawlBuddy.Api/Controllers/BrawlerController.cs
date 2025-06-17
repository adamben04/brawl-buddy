// Brawler Controller - Handles brawler data endpoints

using BrawlBuddy.Api.Models;
using BrawlBuddy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrawlBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrawlerController : ControllerBase
{    private readonly IBrawlApiService _brawlApiService;
    private readonly ILogger<BrawlerController> _logger;

    public BrawlerController(IBrawlApiService brawlApiService, ILogger<BrawlerController> logger)
    {
        _brawlApiService = brawlApiService;
        _logger = logger;
    }

    /// <summary>
    /// Get all available brawlers from the API
    /// </summary>
    /// <returns>List of all brawlers</returns>
    [HttpGet]
    public async Task<IActionResult> GetBrawlers()
    {
        try
        {
            _logger.LogInformation("Fetching all brawlers data");
            
            var brawlers = await _brawlApiService.GetBrawlersAsync();
            
            if (brawlers == null || !brawlers.Any())
            {
                _logger.LogWarning("No brawlers data found");
                return NotFound(new { message = "No brawlers data available" });
            }

            _logger.LogInformation("Successfully retrieved {Count} brawlers", brawlers.Count);
            return Ok(new { brawlers, count = brawlers.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving brawlers data");
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    /// <summary>
    /// Get specific brawler by ID
    /// </summary>
    /// <param name="id">Brawler ID</param>
    /// <returns>Specific brawler data</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBrawler(int id)
    {
        try
        {
            _logger.LogInformation("Fetching brawler data for ID: {Id}", id);
            
            var brawlers = await _brawlApiService.GetBrawlersAsync();
            
            if (brawlers == null)
            {
                _logger.LogWarning("No brawlers data available");
                return NotFound(new { message = "Brawlers data not available" });
            }

            var brawler = brawlers.FirstOrDefault(b => b.Id == id);
            
            if (brawler == null)
            {
                _logger.LogWarning("Brawler not found for ID: {Id}", id);
                return NotFound(new { message = "Brawler not found", id });
            }

            _logger.LogInformation("Successfully retrieved brawler data for ID: {Id}", id);
            return Ok(brawler);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving brawler data for ID: {Id}", id);
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}