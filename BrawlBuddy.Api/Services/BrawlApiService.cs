// Service to interact with the official Brawl Stars API

using BrawlBuddy.Api.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrawlBuddy.Api.Services;

public class BrawlApiService : IBrawlApiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ICacheService _cacheService;
    private readonly ILogger<BrawlApiService> _logger;
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly int _rateLimitDelay;
    private readonly bool _useMockData;    public BrawlApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ICacheService cacheService,
        ILogger<BrawlApiService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _cacheService = cacheService;
        _logger = logger;
        _baseUrl = _configuration["BrawlStarsApi:BaseUrl"] ?? "https://api.brawlapi.com/";
        _apiKey = _configuration["BrawlStarsApi:ApiKey"] ?? "";
        _rateLimitDelay = int.Parse(_configuration["BrawlStarsApi:RateLimitDelay"] ?? "100");
        _useMockData = bool.Parse(_configuration["BrawlStarsApi:UseMockData"] ?? "false");

        _logger.LogInformation("=== BrawlApiService Constructor ===");
        _logger.LogInformation("Base URL: {BaseUrl}", _baseUrl);
        _logger.LogInformation("Use Mock Data: {UseMockData}", _useMockData);
        _logger.LogInformation("Rate Limit Delay: {RateLimitDelay}ms", _rateLimitDelay);

        // Configure HTTP client
        _httpClient.BaseAddress = new Uri(_baseUrl);
        
        // Only add authorization for official Brawl Stars API
        if (_baseUrl.Contains("api.brawlstars.com") && !string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            _logger.LogInformation("Added Authorization header for official Brawl Stars API");
        }
        else
        {
            // For BrawlAPI, we need User-Agent header
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BrawlBuddy/1.0");
            _logger.LogInformation("Added User-Agent header for BrawlAPI");
        }
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

            // Use mock data if configured
            if (_useMockData)
            {
                _logger.LogInformation("Using mock player data for development");
                var mockPlayer = GetMockPlayer(playerTag);
                _cacheService.Set(cacheKey, mockPlayer, TimeSpan.FromMinutes(1));
                return mockPlayer;
            }

            // Ensure player tag is properly formatted
            var formattedTag = FormatPlayerTag(playerTag);
            
            _logger.LogInformation("Fetching player data for {PlayerTag}", formattedTag);
            _logger.LogInformation("Using API base URL: {BaseUrl}", _baseUrl);
            _logger.LogInformation("Full request URL: {RequestUrl}", $"{_baseUrl}/players/{Uri.EscapeDataString(formattedTag)}");
            
            var response = await _httpClient.GetAsync($"/players/{Uri.EscapeDataString(formattedTag)}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch player {PlayerTag}. Status: {StatusCode}", formattedTag, response.StatusCode);
                _logger.LogWarning("Response content: {ResponseContent}", await response.Content.ReadAsStringAsync());
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

            // Use mock data if configured
            if (_useMockData)
            {
                _logger.LogInformation("Using mock battle log data for development");
                var mockBattleLog = GetMockBattleLog(playerTag);
                _cacheService.Set(cacheKey, mockBattleLog, TimeSpan.FromMinutes(2));
                return mockBattleLog;
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
    }    public async Task<List<Brawler>?> GetBrawlersAsync()
    {
        try
        {
            _logger.LogInformation("=== GetBrawlersAsync CALLED ===");
            
            // Check cache first
            const string cacheKey = "all_brawlers";
            var cachedBrawlers = _cacheService.Get<List<Brawler>>(cacheKey);
            if (cachedBrawlers != null)
            {
                _logger.LogInformation("Returning cached brawlers data - Count: {Count}", cachedBrawlers.Count);
                _logger.LogInformation("=== GetBrawlersAsync RETURNING CACHED: {Count} ===", cachedBrawlers.Count);
                return cachedBrawlers;
            }

            _logger.LogInformation("No cached data found, fetching from API");

            // Use mock data if configured
            if (_useMockData)
            {
                _logger.LogInformation("Using mock brawlers data for development");
                var mockBrawlers = GetMockBrawlers();
                _logger.LogInformation("Mock brawlers count: {Count}", mockBrawlers?.Count ?? 0);
                _cacheService.Set(cacheKey, mockBrawlers, TimeSpan.FromMinutes(30)); // Cache mock data
                _logger.LogInformation("=== GetBrawlersAsync RETURNING MOCK: {Count} ===", mockBrawlers?.Count ?? 0);
                return mockBrawlers;
            }            _logger.LogInformation("Fetching all brawlers data from BrawlAPI");
            _logger.LogInformation("Making request to: {BaseUrl}v1/brawlers", _baseUrl);

            // Use BrawlAPI endpoint
            var response = await _httpClient.GetAsync("v1/brawlers");

            _logger.LogInformation("BrawlAPI response status: {StatusCode}", response.StatusCode);
            _logger.LogInformation("BrawlAPI response headers: {Headers}", response.Headers.ToString());

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch brawlers from BrawlAPI. Status: {StatusCode}", response.StatusCode);
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error response content: {ErrorContent}", errorContent);

                // Return mock data as fallback
                var mockBrawlers = GetMockBrawlers();
                _logger.LogInformation("Returning mock brawlers data as fallback - Count: {Count}", mockBrawlers?.Count ?? 0);
                _cacheService.Set(cacheKey, mockBrawlers, TimeSpan.FromMinutes(5)); // Cache mock data fallback
                _logger.LogInformation("=== GetBrawlersAsync RETURNING MOCK FALLBACK: {Count} ===", mockBrawlers?.Count ?? 0);
                return mockBrawlers;
            }

            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Received JSON response length: {Length} characters", json.Length);
            _logger.LogInformation("JSON response preview: {JsonPreview}", json.Length > 200 ? json.Substring(0, 200) + "..." : json);

            var brawlersListResponse = JsonSerializer.Deserialize<BrawlersListApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _logger.LogInformation("Deserialization complete. Response is null: {IsNull}", brawlersListResponse == null);
            
            var brawlers = brawlersListResponse?.List; // Changed from Items to List
            _logger.LogInformation("Brawlers list is null: {IsNull}", brawlers == null);
            
            if (brawlers != null)
            {
                _logger.LogInformation("Brawlers list count: {Count}", brawlers.Count);
                
                // Cache brawlers data for longer (less dynamic)
                _cacheService.Set(cacheKey, brawlers, TimeSpan.FromHours(1));
                _logger.LogInformation("Successfully fetched and cached {Count} brawlers from BrawlAPI", brawlers.Count);
                _logger.LogInformation("=== GetBrawlersAsync RETURNING API DATA: {Count} ===", brawlers.Count);
            }
            else
            {
                _logger.LogWarning("Brawlers list was null after deserialization from BrawlAPI. JSON: {Json}", json);
                 // Return mock data as fallback if deserialization yields null list
                var mockBrawlers = GetMockBrawlers();
                _logger.LogInformation("Returning mock brawlers data as fallback due to null list from API - Count: {Count}", mockBrawlers?.Count ?? 0);
                _cacheService.Set(cacheKey, mockBrawlers, TimeSpan.FromMinutes(5));
                _logger.LogInformation("=== GetBrawlersAsync RETURNING MOCK DUE TO NULL: {Count} ===", mockBrawlers?.Count ?? 0);
                return mockBrawlers;
            }

            await Task.Delay(_rateLimitDelay);

            return brawlers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "=== ERROR IN GetBrawlersAsync ===");
            _logger.LogError("Exception type: {ExceptionType}", ex.GetType().Name);
            _logger.LogError("Exception message: {ExceptionMessage}", ex.Message);
            _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);

            // Return mock data as fallback
            var mockBrawlers = GetMockBrawlers();
            _logger.LogInformation("Returning mock brawlers data as fallback due to exception - Count: {Count}", mockBrawlers?.Count ?? 0);
            _cacheService.Set("all_brawlers", mockBrawlers, TimeSpan.FromMinutes(5)); // Cache mock data fallback
            _logger.LogInformation("=== GetBrawlersAsync RETURNING MOCK DUE TO EXCEPTION: {Count} ===", mockBrawlers?.Count ?? 0);
            return mockBrawlers;
        }
    }

    public async Task<EventRotation?> GetEventRotationAsync()
    {
        try
        {
            // Check cache first
            const string cacheKey = "event_rotation";
            var cachedEventRotation = _cacheService.Get<EventRotation>(cacheKey);
            if (cachedEventRotation != null)
            {
                _logger.LogInformation("Returning cached event rotation data");
                return cachedEventRotation;
            }

            // Use mock data if configured
            if (_useMockData)
            {
                _logger.LogInformation("Using mock event rotation data for development");
                var mockEventRotation = GetMockEventRotation();
                _cacheService.Set(cacheKey, mockEventRotation, TimeSpan.FromMinutes(15));
                return mockEventRotation;
            }

            _logger.LogInformation("Fetching event rotation data from BrawlAPI");

            var response = await _httpClient.GetAsync("/events/rotation");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch event rotation from BrawlAPI. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var eventRotation = JsonSerializer.Deserialize<EventRotation>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (eventRotation != null)
            {
                // Cache event rotation data
                _cacheService.Set(cacheKey, eventRotation, TimeSpan.FromMinutes(15));
                _logger.LogInformation("Successfully fetched and cached event rotation from BrawlAPI");
            }

            await Task.Delay(_rateLimitDelay);

            return eventRotation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching event rotation data from BrawlAPI");
            return null;
        }
    }

    public async Task<List<Strategy>?> GetMapStrategiesAsync(string mapId)
    {
        try
        {
            // Check cache first
            var cacheKey = $"map_strategies_{mapId}";
            var cachedMapStrategies = _cacheService.Get<List<Strategy>>(cacheKey);
            if (cachedMapStrategies != null)
            {
                _logger.LogInformation("Returning cached map strategies for map {MapId}", mapId);
                return cachedMapStrategies;
            }

            // Use mock data if configured
            if (_useMockData)
            {
                _logger.LogInformation("Using mock map strategies data for development");
                var mockMapStrategies = GetMockMapStrategies(mapId);
                _cacheService.Set(cacheKey, mockMapStrategies, TimeSpan.FromMinutes(30));
                return mockMapStrategies;
            }

            _logger.LogInformation("Fetching map strategies for map {MapId} from BrawlAPI", mapId);

            var response = await _httpClient.GetAsync($"/maps/{mapId}/strategies");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch map strategies for map {MapId} from BrawlAPI. Status: {StatusCode}", mapId, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            var mapStrategies = JsonSerializer.Deserialize<List<Strategy>>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (mapStrategies != null)
            {
                // Cache map strategies data
                _cacheService.Set(cacheKey, mapStrategies, TimeSpan.FromMinutes(30));
                _logger.LogInformation("Successfully fetched and cached map strategies for map {MapId} from BrawlAPI", mapId);
            }

            await Task.Delay(_rateLimitDelay);

            return mapStrategies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching map strategies for map {MapId} from BrawlAPI", mapId);
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
        // Don't convert to uppercase - Brawl Stars tags are case-sensitive
        return playerTag;
    }    private static List<Brawler> GetMockBrawlers()
    {
        return new List<Brawler>
        {
            new() {
                Id = 16000000,
                Name = "Shelly",
                Description = "Shelly's shotgun shreds enemies at close range.",
                ImageUrl = $"https://cdn.brawlify.com/brawlers/borderless/16000000.png",
                Rarity = new BrawlerRarity { Id = 1, Name = "Common", Color = "#b9eaff" },
                Class = new BrawlerClass { Id = 1, Name = "Damage Dealer" },
                StarPowers = new List<BrawlerStarPower> {
                    new() { Id = 23000077, Name = "Shell Shock", Description = "Super shells slow down enemies for 3.0 seconds!" },
                    new() { Id = 23000078, Name = "Band-Aid", Description = "When Shelly falls below 40% health, she instantly heals to 50% health. Band-Aid recharges in 20.0 seconds." }
                },
                Gadgets = new List<BrawlerGadget> {
                    new() { Id = 23000188, Name = "Fast Forward", Description = "Shelly dashes forward, skipping a few steps!" },
                    new() { Id = 23000189, Name = "Clay Pigeons", Description = "Shelly's next main attack deals +550 damage but has -33% range." }
                }
            },
            new() {
                Id = 16000001,
                Name = "Colt",
                Description = "Colt fires an accurate burst of bullets from his dual revolvers.",
                ImageUrl = $"https://cdn.brawlify.com/brawlers/borderless/16000001.png",
                Rarity = new BrawlerRarity { Id = 1, Name = "Common", Color = "#b9eaff" },
                Class = new BrawlerClass { Id = 1, Name = "Damage Dealer" },                StarPowers = new List<BrawlerStarPower> {
                    new() { Id = 23000079, Name = "Slick Boots", Description = "Colt's movement speed is increased by 10%." },
                    new() { Id = 23000080, Name = "Magnum Special", Description = "Colt's attack range and bullet speed are increased by 11%." }
                },
                Gadgets = new List<BrawlerGadget> {
                    new() { Id = 23000190, Name = "Speedloader", Description = "Colt instantly reloads 2 ammo." },
                    new() { Id = 23000191, Name = "Silver Bullet", Description = "Colt's next attack ignores walls and deals +1000 damage." }
                }
            },
            new() { 
                Id = 16000002, 
                Name = "Bull", 
                Description = "Bull deals massive damage up close with his shotgun.",
                ImageUrl = $"https://cdn.brawlify.com/brawlers/borderless/16000002.png",
                Rarity = new BrawlerRarity { Id = 1, Name = "Common", Color = "#b9eaff" },
                Class = new BrawlerClass { Id = 2, Name = "Tank" },
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000081, Name = "Berserker", Description = "When Bull falls below 60% health, his reload speed doubles." },
                    new() { Id = 23000082, Name = "Tough Guy", Description = "When Bull falls below 40% health, he gains a shield that reduces all damage he takes by 30%." }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000192, Name = "T-Bone Injector", Description = "Bull instantly heals 1500 health." },
                    new() { Id = 23000193, Name = "Stomper", Description = "Bull's next main attack has +1000 damage and destroys walls." }
                } 
            },
            new() { 
                Id = 16000003, 
                Name = "Brock", 
                Description = "Brock fires rockets that pack a punch!",
                ImageUrl = $"https://cdn.brawlify.com/brawlers/borderless/16000003.png",
                Rarity = new BrawlerRarity { Id = 2, Name = "Rare", Color = "#68fd58" },
                Class = new BrawlerClass { Id = 1, Name = "Damage Dealer" },                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000083, Name = "Incendiary", Description = "Brock's attack sets the ground on fire for 2 seconds." },
                    new() { Id = 23000084, Name = "Rocket No. 4", Description = "Brock's rocket splits into 4 smaller rockets when it hits a wall." }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000194, Name = "Rocket Laces", Description = "Brock blasts himself into the air and over obstacles." },
                    new() { Id = 23000195, Name = "Rocket Fuel", Description = "Brock's next attack has +1050 damage and +33% range." }
                } 
            },
            new() { 
                Id = 16000004, 
                Name = "Rico", 
                Description = "Rico's bullets bounce off walls!",
                ImageUrl = $"https://cdn.brawlify.com/brawlers/borderless/16000004.png",
                Rarity = new BrawlerRarity { Id = 3, Name = "Super Rare", Color = "#5ab7ff" },
                Class = new BrawlerClass { Id = 1, Name = "Damage Dealer" },
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000085, Name = "Super Bouncy", Description = "Rico's bullets gain +80 damage after bouncing off a wall." },
                    new() { Id = 23000086, Name = "Robo Retreat", Description = "When Rico falls below 40% health, his movement speed increases by 34%." }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000196, Name = "Multiball Launcher", Description = "Rico's next attack splits into 5 bullets in a cone pattern." },
                    new() { Id = 23000197, Name = "Bouncy Castle", Description = "Rico bounces around, becoming immune to damage for 0.6 seconds." }
                } 
            },            new() { 
                Id = 16000005, 
                Name = "Spike", 
                Description = "Spike throws cactus grenades that explode and shoot spikes in all directions!",
                ImageUrl = $"https://cdn.brawlify.com/brawlers/borderless/16000005.png",
                Rarity = new BrawlerRarity { Id = 5, Name = "Legendary", Color = "#fff063" },
                Class = new BrawlerClass { Id = 1, Name = "Damage Dealer" },                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000087, Name = "Fertilize", Description = "Spike's Super heals him and his teammates who are in its area of effect." },
                    new() { Id = 23000088, Name = "Curveball", Description = "Spike's attack spikes curve around walls and other obstacles." }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000198, Name = "Popping Pincushion", Description = "Spike instantly breaks apart and shoots spikes in all directions." },
                    new() { Id = 23000199, Name = "Life Plant", Description = "Spike throws a cactus that heals him and teammates." }                } 
            },
            new() { 
                Id = 16000094, 
                Name = "Kaze", 
                Description = "Kaze is a mysterious Ultra Legendary brawler with wind powers!",
                ImageUrl = $"https://cdn.brawlify.com/brawlers/borderless/16000094.png",
                Rarity = new BrawlerRarity { Id = 7, Name = "Ultra Legendary", Color = "#ff6b35" },
                Class = new BrawlerClass { Id = 3, Name = "Assassin" },
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000300, Name = "Wind Walker", Description = "Kaze gains increased movement speed after using his Super." },
                    new() { Id = 23000301, Name = "Storm Strike", Description = "Kaze's attacks create wind currents that deal extra damage." }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000400, Name = "Gust Shield", Description = "Kaze creates a protective wind barrier." },
                    new() { Id = 23000401, Name = "Whirlwind Dash", Description = "Kaze dashes through enemies, dealing damage." }
                } 
            }
        };
    }

    private static Player GetMockPlayer(string playerTag)
    {
        var random = new Random(playerTag.GetHashCode());
        var brawlers = GetMockBrawlers();
        
        // Select 15-25 random brawlers for the player
        var playerBrawlers = brawlers
            .OrderBy(x => random.Next())
            .Take(random.Next(15, 26))
            .Select(b => new PlayerBrawler
            {
                Id = b.Id,
                Name = b.Name,
                Power = random.Next(1, 12),
                Rank = random.Next(1, 36),
                Trophies = random.Next(0, 1000),
                HighestTrophies = random.Next(500, 1500),
                Gadgets = b.Gadgets.Take(random.Next(0, 3)).Select(g => new PlayerGadget
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList(),
                StarPowers = b.StarPowers.Take(random.Next(0, 3)).Select(sp => new PlayerStarPower
                {
                    Id = sp.Id,
                    Name = sp.Name
                }).ToList(),
                Gears = new List<PlayerGear>()
            }).ToList();

        return new Player
        {
            Tag = playerTag,
            Name = $"MockPlayer_{playerTag.Replace("#", "")}",
            NameColor = "0xffffffff",
            Icon = new PlayerIcon { Id = random.Next(28000000, 28000100) },
            Trophies = playerBrawlers.Sum(b => b.Trophies),
            HighestTrophies = playerBrawlers.Sum(b => b.HighestTrophies),
            ExpLevel = random.Next(1, 200),
            ExpPoints = random.Next(0, 10000),
            IsQualifiedFromChampionshipChallenge = random.NextDouble() > 0.8,
            SoloVictories = random.Next(0, 500),
            DuoVictories = random.Next(0, 300),
            BestRoboRumbleTime = random.Next(60, 600),
            BestTimeAsBigBrawler = random.Next(30, 180),
            Club = random.NextDouble() > 0.3 ? new PlayerClub
            {
                Tag = "#" + random.Next(1000000, 9999999).ToString("X"),
                Name = "Mock Club " + random.Next(1, 100)
            } : null,
            Brawlers = playerBrawlers
        };
    }

    private static BattleLog GetMockBattleLog(string playerTag)
    {
        var random = new Random(playerTag.GetHashCode());
        var brawlers = GetMockBrawlers();
        var battles = new List<BattleLogItem>();

        var gameModes = new[] { "Gem Grab", "Brawl Ball", "Heist", "Bounty", "Siege", "Hot Zone", "Knockout", "Solo Showdown", "Duo Showdown" };
        var maps = new[] { "Shooting Star", "Hard Rock Mine", "Backyard Bowl", "Snake Prairie", "Feast or Famine", "Cavern Churn", "Scorched Stone" };

        for (int i = 0; i < 20; i++)
        {
            var mode = gameModes[random.Next(gameModes.Length)];
            var map = maps[random.Next(maps.Length)];
            var result = random.NextDouble() > 0.5 ? "victory" : "defeat";
            var isTeamMode = !mode.Contains("Showdown");

            var battle = new BattleLogItem
            {
                BattleTime = DateTime.UtcNow.AddMinutes(-random.Next(1, 10080)).ToString("yyyyMMddTHHmmss.fffZ"),
                Event = new BattleEvent
                {
                    Id = random.Next(15000000, 15000100),
                    Mode = mode,
                    Map = map
                },
                Battle = new Battle
                {
                    Mode = mode,
                    Type = random.NextDouble() > 0.8 ? "ranked" : "casual",
                    Result = result,
                    Duration = random.Next(60, 300),
                    TrophyChange = result == "victory" ? random.Next(1, 12) : -random.Next(1, 12),
                    StarTokensGained = random.Next(0, 3),
                    Teams = isTeamMode ? new List<List<BattlePlayer>>
                    {
                        GenerateMockTeam(brawlers, random, 3),
                        GenerateMockTeam(brawlers, random, 3)
                    } : new List<List<BattlePlayer>>()
                }
            };

            battles.Add(battle);
        }

        return new BattleLog { Items = battles };
    }

    private static EventRotation GetMockEventRotation()
    {
        return new EventRotation(); // Implement mock data for EventRotation
    }

    private static List<Strategy> GetMockMapStrategies(string mapId)
    {
        return new List<Strategy>(); // Implement mock data for MapStrategies
    }

    private static List<BattlePlayer> GenerateMockTeam(List<Brawler> brawlers, Random random, int teamSize)
    {
        return brawlers
            .OrderBy(x => random.Next())
            .Take(teamSize)
            .Select(b => new BattlePlayer
            {
                Tag = "#" + random.Next(1000000, 9999999).ToString("X"),
                Name = $"Player{random.Next(1, 1000)}",
                Brawler = new BattleBrawler
                {
                    Id = b.Id,
                    Name = b.Name,
                    Power = random.Next(1, 12),
                    Trophies = random.Next(0, 1000)
                }
            }).ToList();
    }
}

// Helper class for brawlers API response
public class BrawlersListApiResponse // Renamed from BrawlersResponse
{
    [JsonPropertyName("list")] // To match the expected JSON structure like {"list": [...]}
    public List<Brawler> List { get; set; } = new();
}