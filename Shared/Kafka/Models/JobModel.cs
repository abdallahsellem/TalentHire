namespace TalentHire.Shared.Kafka.Models;

// Job Events
    public enum JobStatus
    {
        Open,
        Closed,
        Pending
    }
public class JobCreatedEvent : BaseEvent
{
    public int JobId { get; set; } = 0;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; }

    public int EmployerId { get; set; } = 0;
    public JobStatus Status { get; set; } = JobStatus.Open; // Default to "Open"
}