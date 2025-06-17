// Player model based on Brawl Stars API structure

namespace BrawlBuddy.Api.Models;

public class Player
{
    public string Tag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NameColor { get; set; } = string.Empty;
    public PlayerIcon Icon { get; set; } = new();
    public int Trophies { get; set; }
    public int HighestTrophies { get; set; }
    public int ExpLevel { get; set; }
    public int ExpPoints { get; set; }
    public bool IsQualifiedFromChampionshipChallenge { get; set; }
    public int SoloVictories { get; set; }
    public int DuoVictories { get; set; }
    public int BestRoboRumbleTime { get; set; }
    public int BestTimeAsBigBrawler { get; set; }
    public PlayerClub? Club { get; set; }
    public List<PlayerBrawler> Brawlers { get; set; } = new();
    
    // Alternative property name mapping for the API response
    [System.Text.Json.Serialization.JsonPropertyName("3vs3Victories")]
    public int ThreeVsThreeVictories { get; set; }
}

public class PlayerIcon
{
    public int Id { get; set; }
}

public class PlayerClub
{
    public string Tag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class PlayerBrawler
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Power { get; set; }
    public int Rank { get; set; }
    public int Trophies { get; set; }
    public int HighestTrophies { get; set; }
    public List<PlayerGadget> Gadgets { get; set; } = new();
    public List<PlayerStarPower> StarPowers { get; set; } = new();
    public List<PlayerGear> Gears { get; set; } = new();
}

public class PlayerGadget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PlayerStarPower
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PlayerGear
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
}