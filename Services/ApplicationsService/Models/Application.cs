using System;
namespace TalentHire.Services.ApplicationsService.Models
{
    
public class Application
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int UserId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    public string ResumeUrl { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedDate { get; set; }
    public string? ReviewerNotes { get; set; }
}

public enum ApplicationStatus
{
    Pending,
    UnderReview,
    Accepted,
    Rejected,
    Withdrawn
}

};