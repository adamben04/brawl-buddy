// Player Controller - Handles player data endpoints

using BrawlBuddy.Api.Models;
using BrawlBuddy.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrawlBuddy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly BrawlApiService _brawlApiService;
    private readonly ILogger<PlayerController> _logger;

    public PlayerController(BrawlApiService brawlApiService, ILogger<PlayerController> logger)
    {
        _brawlApiService = brawlApiService;
        _logger = logger;
    }

    // Test endpoint
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new { message = "Backend is working!", timestamp = DateTime.Now });
    }
    
    /// <summary>
    /// Get player profile data by player tag
    /// </summary>
    /// <param name="tag">Player tag (with or without #)</param>
    /// <returns>Player profile data</returns>
    [HttpGet("{tag}")]
    public async Task<IActionResult> GetPlayer(string tag)
    {
        try
        {
            _logger.LogInformation("Fetching player data for tag: {Tag}", tag);
            
            var player = await _brawlApiService.GetPlayerAsync(tag);
            
            if (player == null)
            {
                _logger.LogWarning("Player not found for tag: {Tag}", tag);
                return NotFound(new { message = "Player not found", tag });
            }

            _logger.LogInformation("Successfully retrieved player data for tag: {Tag}", tag);
            return Ok(player);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving player data for tag: {Tag}", tag);
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
    
    /// <summary>
    /// Get player battle log by player tag
    /// </summary>
    /// <param name="tag">Player tag (with or without #)</param>
    /// <returns>Player battle log data</returns>
    [HttpGet("{tag}/battles")]
    public async Task<IActionResult> GetPlayerBattles(string tag)
    {
        try
        {
            _logger.LogInformation("Fetching battle log for tag: {Tag}", tag);
            
            var battleLog = await _brawlApiService.GetPlayerBattleLogAsync(tag);
            
            if (battleLog == null)
            {
                _logger.LogWarning("Battle log not found for tag: {Tag}", tag);
                return NotFound(new { message = "Battle log not found", tag });
            }

            _logger.LogInformation("Successfully retrieved battle log for tag: {Tag}", tag);
            return Ok(battleLog);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving battle log for tag: {Tag}", tag);
            return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        }
    }
}