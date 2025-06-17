// TODO: Implement Strategy model properties

namespace BrawlBuddy.Api.Models;

public class Strategy
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
}