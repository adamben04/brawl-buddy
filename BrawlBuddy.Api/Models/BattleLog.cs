// BattleLog model based on Brawl Stars API structure

namespace BrawlBuddy.Api.Models;

public class BattleLog
{
    public List<BattleLogItem> Items { get; set; } = new();
}

public class BattleLogItem
{
    public string BattleTime { get; set; } = string.Empty;
    public BattleEvent Event { get; set; } = new();
    public Battle Battle { get; set; } = new();
}

public class BattleEvent
{
    public int Id { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string Map { get; set; } = string.Empty;
}

public class Battle
{
    public string Mode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public int Duration { get; set; }
    public int TrophyChange { get; set; }
    public int StarTokensGained { get; set; }
    public List<List<BattlePlayer>> Teams { get; set; } = new();
}

public class BattlePlayer
{
    public string Tag { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public BattleBrawler Brawler { get; set; } = new();
}

public class BattleBrawler
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Power { get; set; }
    public int Trophies { get; set; }
}