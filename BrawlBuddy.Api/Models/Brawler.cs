// Brawler model based on Brawl Stars API structure

namespace BrawlBuddy.Api.Models;

public class Brawler
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<BrawlerStarPower> StarPowers { get; set; } = new();
    public List<BrawlerGadget> Gadgets { get; set; } = new();
}

public class BrawlerStarPower
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class BrawlerGadget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}