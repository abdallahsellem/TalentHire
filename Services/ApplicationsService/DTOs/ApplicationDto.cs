
using TalentHire.Services.ApplicationsService.Models;
namespace TalentHire.Services.ApplicationsService.DTOs
{

    public class ApplicationDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int UserId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantEmail { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? ReviewedDate { get; set; }
        public string? ReviewerNotes { get; set; }
    }

    public class CreateApplicationDto
    {
        public int JobId { get; set; }
        public int UserId { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string ApplicantEmail { get; set; } = string.Empty;
        public string ResumeUrl { get; set; } = string.Empty;
        public string CoverLetter { get; set; } = string.Empty;
    }

    public class UpdateApplicationStatusDto
    {
        public ApplicationStatus Status { get; set; }
        public string? ReviewerNotes { get; set; }
    }

};