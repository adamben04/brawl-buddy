// TODO: Implement MapStrategiesController endpoints

using BrawlBuddy.Api.Models;
using BrawlBuddy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrawlBuddy.Api.Controllers;

[ApiController]
[Route("api/maps/{mapId}/strategies")]
public class MapStrategiesController : ControllerBase
{
    private readonly IBrawlApiService _brawlApiService;
    private readonly ILogger<MapStrategiesController> _logger;

    public MapStrategiesController(IBrawlApiService brawlApiService, ILogger<MapStrategiesController> logger)
    {
        _brawlApiService = brawlApiService;
        _logger = logger;
    }

    /// <summary>
    /// Get community map strategies
    /// </summary>
    /// <param name="mapId">Map ID</param>
    /// <returns>List of map strategies</returns>
    [HttpGet]
    public async Task<IActionResult> GetMapStrategies(string mapId)
    {
        try
        {
            _logger.LogInformation("Fetching map strategies for map {MapId}", mapId);

            var mapStrategies = await _brawlApiService.GetMapStrategiesAsync(mapId);

            if (mapStrategies == null)
            {
                _logger.LogWarning("Map strategies not found for map {MapId}", mapId);
                return NotFound(new { message = "Map strategies not found", mapId });
            }

            _logger.LogInformation("Successfully retrieved map strategies for map {MapId}", mapId);
            return Ok(mapStrategies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving map strategies for map {MapId}", mapId);
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }

    // TODO: Submit strategy tip (auth req.)
    // POST /api/maps/{mapId}/strategies
}