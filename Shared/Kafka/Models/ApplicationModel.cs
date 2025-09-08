namespace TalentHire.Shared.Kafka.Models;

// Base Event
public abstract class BaseEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = string.Empty;
}

// Order Events
public class ApplicationCreatedEvent : BaseEvent
{
    public int ApplicationId { get; set; } = 0;
    public int UserId { get; set; } = 0;
    public int JobId { get; set; } = 0;
    public string Status { get; set; } = string.Empty;
}