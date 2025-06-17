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
    }

    private static List<Brawler> GetMockBrawlers()
    {
        // Adding ImageUrl, Rarity, and Class to mock brawlers for frontend development
        // This assumes your Brawler model has these properties.
        // You'll need to define these properties in your Models.Brawler class.
        return new List<Brawler>
        {
            new() {
                Id = 16000000,
                Name = "Shelly",
                // Assuming Brawler model has these:
                // ImageUrl = "https://cdn.brawlify.com/brawlers-bs/16000000.png", // Example URL
                // Rarity = new BrawlerRarity { Name = "Common", Color = "#b9eaff" },
                // Class = new BrawlerClass { Name = "Damage Dealer" },
                StarPowers = new List<BrawlerStarPower> {
                    new() { Id = 23000077, Name = "Shell Shock" },
                    new() { Id = 23000078, Name = "Band-Aid" }
                },
                Gadgets = new List<BrawlerGadget> {
                    new() { Id = 23000188, Name = "Fast Forward" },
                    new() { Id = 23000189, Name = "Clay Pigeons" }
                }
            },
            new() {
                Id = 16000001,
                Name = "Colt",
                // ImageUrl = "https://cdn.brawlify.com/brawlers-bs/16000001.png",
                // Rarity = new BrawlerRarity { Name = "Common", Color = "#b9eaff" },
                // Class = new BrawlerClass { Name = "Damage Dealer" },
                StarPowers = new List<BrawlerStarPower> {
                    new() { Id = 23000079, Name = "Slick Boots" },
                    new() { Id = 23000080, Name = "Magnum Special" }
                },
                Gadgets = new List<BrawlerGadget> {
                    new() { Id = 23000190, Name = "Speedloader" },
                    new() { Id = 23000191, Name = "Silver Bullet" }
                }
            },
            new() { 
                Id = 16000002, 
                Name = "Bull", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000081, Name = "Berserker" },
                    new() { Id = 23000082, Name = "Tough Guy" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000192, Name = "T-Bone Injector" },
                    new() { Id = 23000193, Name = "Stomper" }
                } 
            },
            new() { 
                Id = 16000003, 
                Name = "Brock", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000083, Name = "Incendiary" },
                    new() { Id = 23000084, Name = "Rocket No. 4" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000194, Name = "Rocket Laces" },
                    new() { Id = 23000195, Name = "Rocket Fuel" }
                } 
            },
            new() { 
                Id = 16000004, 
                Name = "Rico", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000085, Name = "Super Bouncy" },
                    new() { Id = 23000086, Name = "Robo Retreat" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000196, Name = "Multiball Launcher" },
                    new() { Id = 23000197, Name = "Bouncy Castle" }
                } 
            },
            new() { 
                Id = 16000005, 
                Name = "Spike", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000087, Name = "Fertilize" },
                    new() { Id = 23000088, Name = "Curveball" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000198, Name = "Popping Pincushion" },
                    new() { Id = 23000199, Name = "Life Plant" }
                } 
            },
            new() { 
                Id = 16000006, 
                Name = "Barley", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000089, Name = "Medical Use" },
                    new() { Id = 23000090, Name = "Extra Noxious" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000200, Name = "Sticky Syrup Mixer" },
                    new() { Id = 23000201, Name = "Herbal Tonic" }
                } 
            },
            new() { 
                Id = 16000007, 
                Name = "Jessie", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000091, Name = "Energize" },
                    new() { Id = 23000092, Name = "Shocky" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000202, Name = "Spark Plug" },
                    new() { Id = 23000203, Name = "Recoil Spring" }
                } 
            },
            new() { 
                Id = 16000008, 
                Name = "Nita", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000093, Name = "Bear With Me" },
                    new() { Id = 23000094, Name = "Hyper Bear" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000204, Name = "Bear Paws" },
                    new() { Id = 23000205, Name = "Faux Fur" }
                } 
            },
            new() { 
                Id = 16000009, 
                Name = "Dynamike", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000095, Name = "Dyna-Jump" },
                    new() { Id = 23000096, Name = "Demolition" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000206, Name = "Fidget Spinner" },
                    new() { Id = 23000207, Name = "Satchel Charge" }
                } 
            },
            new() { 
                Id = 16000010, 
                Name = "El Primo", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000097, Name = "El Fuego" },
                    new() { Id = 23000098, Name = "Meteor Rush" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000208, Name = "Suplex Supplement" },
                    new() { Id = 23000209, Name = "Asteroid Belt" }
                } 
            },
            new() { 
                Id = 16000011, 
                Name = "Mortis", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000099, Name = "Creepy Harvest" },
                    new() { Id = 23000100, Name = "Coiled Snake" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000210, Name = "Combo Spinner" },
                    new() { Id = 23000211, Name = "Survival Shovel" }
                } 
            },
            new() { 
                Id = 16000012, 
                Name = "Crow", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000101, Name = "Extra Toxic" },
                    new() { Id = 23000102, Name = "Carrion Crow" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000212, Name = "Defense Booster" },
                    new() { Id = 23000213, Name = "Slowing Toxin" }
                } 
            },
            new() { 
                Id = 16000013, 
                Name = "Poco", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000103, Name = "Da Capo!" },
                    new() { Id = 23000104, Name = "Screeching Solo" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000214, Name = "Tuning Fork" },
                    new() { Id = 23000215, Name = "Protective Tunes" }
                } 
            },
            new() { 
                Id = 16000014, 
                Name = "Bo", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000105, Name = "Circling Eagle" },
                    new() { Id = 23000106, Name = "Snare a Bear" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000216, Name = "Super Totem" },
                    new() { Id = 23000217, Name = "Tripwire" }
                } 
            },
            
            // Rare Brawlers
            new() { 
                Id = 16000015, 
                Name = "Piper", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000107, Name = "Ambush" },
                    new() { Id = 23000108, Name = "Snappy Sniping" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000218, Name = "Auto Aimer" },
                    new() { Id = 23000219, Name = "Homemade Recipe" }
                } 
            },
            new() { 
                Id = 16000016, 
                Name = "Pam", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000109, Name = "Mama's Hug" },
                    new() { Id = 23000110, Name = "Mama's Squeeze" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000220, Name = "Pulse Modulator" },
                    new() { Id = 23000221, Name = "Scrapsucker" }
                } 
            },
            new() { 
                Id = 16000017, 
                Name = "Frank", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000111, Name = "Power Grab" },
                    new() { Id = 23000112, Name = "Sponge" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000222, Name = "Active Noise Canceling" },
                    new() { Id = 23000223, Name = "Irresistible Attraction" }
                } 
            },
            new() { 
                Id = 16000018, 
                Name = "Bibi", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000113, Name = "Home Run" },
                    new() { Id = 23000114, Name = "Batting Stance" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000224, Name = "Vitamin Booster" },
                    new() { Id = 23000225, Name = "Extra Sticky" }
                } 
            },
            new() { 
                Id = 16000019, 
                Name = "Bea", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000115, Name = "Insta Beeload" },
                    new() { Id = 23000116, Name = "Honey Coat" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000226, Name = "Honey Molasses" },
                    new() { Id = 23000227, Name = "Rattled Hive" }
                } 
            },
            
            // Super Rare Brawlers  
            new() { 
                Id = 16000020, 
                Name = "Darryl", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000117, Name = "Steel Hoops" },
                    new() { Id = 23000118, Name = "Rolling Reload" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000228, Name = "Recoiling Rotator" },
                    new() { Id = 23000229, Name = "Tar Barrel" }
                } 
            },
            new() { 
                Id = 16000021, 
                Name = "Penny", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000119, Name = "Last Blast" },
                    new() { Id = 23000120, Name = "Balls of Fire" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000230, Name = "Salty Barrel" },
                    new() { Id = 23000231, Name = "Captain's Compass" }
                } 
            },
            new() { 
                Id = 16000022, 
                Name = "Carl", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000121, Name = "Power Throw" },
                    new() { Id = 23000122, Name = "Protective Pirouette" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000232, Name = "Heat Ejector" },
                    new() { Id = 23000233, Name = "Flying Hook" }
                } 
            },
            new() { 
                Id = 16000023, 
                Name = "Jacky", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000123, Name = "Counter Crush" },
                    new() { Id = 23000124, Name = "Hardy Hard Hat" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000234, Name = "Pneumatic Booster" },
                    new() { Id = 23000235, Name = "Rebuild" }
                } 
            },
            new() { 
                Id = 16000024, 
                Name = "Gus", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000125, Name = "Spirit Animal" },
                    new() { Id = 23000126, Name = "Health Bonanza" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000236, Name = "Kooky Popper" },
                    new() { Id = 23000237, Name = "Soul Switcher" }
                } 
            },
            
            // Epic Brawlers
            new() { 
                Id = 16000025, 
                Name = "Rosa", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000127, Name = "Plant Life" },
                    new() { Id = 23000128, Name = "Thorny Gloves" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000238, Name = "Grow Light" },
                    new() { Id = 23000239, Name = "Unfriendly Bushes" }
                } 
            },
            new() { 
                Id = 16000026, 
                Name = "Griff", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000129, Name = "Keep the Change" },
                    new() { Id = 23000130, Name = "Business Resilience" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000240, Name = "Piggy Bank" },
                    new() { Id = 23000241, Name = "Coin Shower" }
                } 
            },
            new() { 
                Id = 16000027, 
                Name = "Bonnie", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000131, Name = "Black Powder" },
                    new() { Id = 23000132, Name = "Wisdom Tooth" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000242, Name = "Sugar Rush" },
                    new() { Id = 23000243, Name = "Crash Test" }
                } 
            },
            new() { 
                Id = 16000028, 
                Name = "Grom", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000133, Name = "Foot Patrol" },
                    new() { Id = 23000134, Name = "X-Factor" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000244, Name = "Watchtower" },
                    new() { Id = 23000245, Name = "Radio Check" }
                } 
            },
            new() { 
                Id = 16000029, 
                Name = "Ash", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000135, Name = "First Bash" },
                    new() { Id = 23000136, Name = "Mad as Heck" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000246, Name = "Chill Pill" },
                    new() { Id = 23000247, Name = "Rotten Banana" }
                } 
            },

            // Mythic Brawlers
            new() { 
                Id = 16000030, 
                Name = "Tara", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000137, Name = "Black Portal" },
                    new() { Id = 23000138, Name = "Healing Shade" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000248, Name = "Psychic Enhancer" },
                    new() { Id = 23000249, Name = "Support from Beyond" }
                } 
            },
            new() { 
                Id = 16000031, 
                Name = "Gene", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000139, Name = "Magic Puffs" },
                    new() { Id = 23000140, Name = "Spirit Slap" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000250, Name = "Lamp Blowout" },
                    new() { Id = 23000251, Name = "Vengeful Spirits" }
                } 
            },
            new() { 
                Id = 16000032, 
                Name = "Max", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000141, Name = "Super Charged" },
                    new() { Id = 23000142, Name = "Run n Gun" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000252, Name = "Phase Shifter" },
                    new() { Id = 23000253, Name = "Sneaky Sneakers" }
                } 
            },
            new() { 
                Id = 16000033, 
                Name = "Mr. P", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000143, Name = "Handle With Care" },
                    new() { Id = 23000144, Name = "Revolving Door" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000254, Name = "Service Bell" },
                    new() { Id = 23000255, Name = "Porter Reinforcements" }
                } 
            },
            new() { 
                Id = 16000034, 
                Name = "Sprout", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000145, Name = "Overgrowth" },
                    new() { Id = 23000146, Name = "Photosynthesis" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000256, Name = "Garden Mulcher" },
                    new() { Id = 23000257, Name = "Transplant" }
                } 
            },

            // Legendary Brawlers
            new() { 
                Id = 16000035, 
                Name = "Leon", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000147, Name = "Smoke Trails" },
                    new() { Id = 23000148, Name = "Invisiheal" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000258, Name = "Clone Projector" },
                    new() { Id = 23000259, Name = "Lollipop Drop" }
                } 
            },
            new() { 
                Id = 16000036, 
                Name = "Sandy", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000149, Name = "Rude Sands" },
                    new() { Id = 23000150, Name = "Healing Winds" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000260, Name = "Sleep Stimulator" },
                    new() { Id = 23000261, Name = "Sweet Dreams" }
                } 
            },
            new() { 
                Id = 16000037, 
                Name = "Amber", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000151, Name = "Wild Flames" },
                    new() { Id = 23000152, Name = "Scorchin' Siphon" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000262, Name = "Fire Starters" },
                    new() { Id = 23000263, Name = "Dancing Flames" }
                } 
            },
            new() { 
                Id = 16000038, 
                Name = "Meg", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000153, Name = "Force Field" },
                    new() { Id = 23000154, Name = "Self Destruction" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000264, Name = "Jolting Volts" },
                    new() { Id = 23000265, Name = "Toolbox" }
                } 
            },
            new() { 
                Id = 16000039, 
                Name = "Chester", 
                StarPowers = new List<BrawlerStarPower> { 
                    new() { Id = 23000155, Name = "Bell O'Mania" },
                    new() { Id = 23000156, Name = "Spicy Dice" }
                }, 
                Gadgets = new List<BrawlerGadget> { 
                    new() { Id = 23000266, Name = "Candy Beans" },
                    new() { Id = 23000267, Name = "Pop Rocks" }
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