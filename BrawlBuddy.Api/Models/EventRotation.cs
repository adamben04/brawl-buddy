// TODO: Implement EventRotation model properties

namespace BrawlBuddy.Api.Models;

public class EventRotation
{
    public List<Event> Schedule { get; set; } = new();
}

public class Event
{
    public int Id { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string Map { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}