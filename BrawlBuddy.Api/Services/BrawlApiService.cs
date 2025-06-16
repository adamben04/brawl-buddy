// Service to interact with the official Brawl Stars API

using BrawlBuddy.Api.Models;
using System.Text.Json;

namespace BrawlBuddy.Api.Services;

public class BrawlApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;
    private readonly ILogger<BrawlApiService> _logger;
    
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly int _rateLimitDelay;

    public BrawlApiService(
        HttpClient httpClient, 
        IConfiguration configuration,
        ICacheService cacheService,
        ILogger<BrawlApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _cacheService = cacheService;
        _logger = logger;
        
        _baseUrl = _configuration["BrawlStarsApi:BaseUrl"] ?? "https://api.brawlstars.com/v1";
        _apiKey = _configuration["BrawlStarsApi:ApiKey"] ?? throw new InvalidOperationException("Brawl Stars API key is required");
        _rateLimitDelay = int.Parse(_configuration["BrawlStarsApi:RateLimitDelay"] ?? "100");
        
        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<Player?> GetPlayerAsync(string playerTag)
    {
        try
        {
            // Check cache first
            var cacheKey = $"player_{playerTag}";
            var cachedPlayer = _cacheService.Get<Player>(cacheKey);
            if (cachedPlayer != null)
            {
                _logger.LogInformation("Returning cached player data for {PlayerTag}", playerTag);
                return cachedPlayer;
            }

            // Ensure player tag is properly formatted
            var formattedTag = FormatPlayerTag(playerTag);
            
            _logger.LogInformation("Fetching player data for {PlayerTag}", formattedTag);
            
            var response = await _httpClient.GetAsync($"/players/{Uri.EscapeDataString(formattedTag)}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch player {PlayerTag}. Status: {StatusCode}", formattedTag, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var player = JsonSerializer.Deserialize<Player>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (player != null)
            {
                // Cache the result
                _cacheService.Set(cacheKey, player);
                _logger.LogInformation("Successfully fetched and cached player data for {PlayerTag}", formattedTag);
            }

            // Add rate limiting delay
            await Task.Delay(_rateLimitDelay);
            
            return player;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching player data for {PlayerTag}", playerTag);
            return null;
        }
    }

    public async Task<BattleLog?> GetPlayerBattleLogAsync(string playerTag)
    {
        try
        {
            // Check cache first
            var cacheKey = $"battlelog_{playerTag}";
            var cachedBattleLog = _cacheService.Get<BattleLog>(cacheKey);
            if (cachedBattleLog != null)
            {
                _logger.LogInformation("Returning cached battle log for {PlayerTag}", playerTag);
                return cachedBattleLog;
            }

            var formattedTag = FormatPlayerTag(playerTag);
            
            _logger.LogInformation("Fetching battle log for {PlayerTag}", formattedTag);
            
            var response = await _httpClient.GetAsync($"/players/{Uri.EscapeDataString(formattedTag)}/battlelog");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch battle log for {PlayerTag}. Status: {StatusCode}", formattedTag, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var battleLog = JsonSerializer.Deserialize<BattleLog>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (battleLog != null)
            {
                // Cache with shorter expiry for battle logs (more dynamic data)
                _cacheService.Set(cacheKey, battleLog, TimeSpan.FromMinutes(2));
                _logger.LogInformation("Successfully fetched and cached battle log for {PlayerTag}", formattedTag);
            }

            await Task.Delay(_rateLimitDelay);
            
            return battleLog;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching battle log for {PlayerTag}", playerTag);
            return null;
        }
    }

    public async Task<List<Brawler>?> GetBrawlersAsync()
    {
        try
        {
            // Check cache first
            const string cacheKey = "all_brawlers";
            var cachedBrawlers = _cacheService.Get<List<Brawler>>(cacheKey);
            if (cachedBrawlers != null)
            {
                _logger.LogInformation("Returning cached brawlers data");
                return cachedBrawlers;
            }

            _logger.LogInformation("Fetching all brawlers data");
            
            var response = await _httpClient.GetAsync("/brawlers");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch brawlers. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var brawlersResponse = JsonSerializer.Deserialize<BrawlersResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var brawlers = brawlersResponse?.Items;
            if (brawlers != null)
            {
                // Cache brawlers data for longer (less dynamic)
                _cacheService.Set(cacheKey, brawlers, TimeSpan.FromHours(1));
                _logger.LogInformation("Successfully fetched and cached {Count} brawlers", brawlers.Count);
            }

            await Task.Delay(_rateLimitDelay);
            
            return brawlers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching brawlers data");
            return null;
        }
    }

    private static string FormatPlayerTag(string playerTag)
    {
        // Ensure the tag starts with # and is properly formatted
        if (!playerTag.StartsWith('#'))
        {
            playerTag = "#" + playerTag;
        }
        return playerTag.ToUpper();
    }
}

// Helper class for brawlers API response
public class BrawlersResponse
{
    public List<Brawler> Items { get; set; } = new();
}