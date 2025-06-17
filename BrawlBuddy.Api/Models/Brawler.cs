// Brawler model based on Brawl Stars API structure

namespace BrawlBuddy.Api.Models;

public class Brawler
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public BrawlerRarity? Rarity { get; set; }
    public BrawlerClass? Class { get; set; }
    public List<BrawlerStarPower> StarPowers { get; set; } = new();
    public List<BrawlerGadget> Gadgets { get; set; } = new();
}

public class BrawlerRarity
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class BrawlerClass
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class BrawlerStarPower
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

public class BrawlerGadget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}