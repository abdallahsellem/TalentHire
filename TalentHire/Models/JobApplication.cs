
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace TalentHire.Models
{
    public class JobApplication
    {
        [Key]
        public int Id { get; set; } // Primary Key

        public int JobId { get; set; } // Foreign Key to Jobs.Id
        [ForeignKey(nameof(JobId))]
        public Job Job { get; set; } // Navigation property for the related Job


        public int JobSeekerId { get; set; } // Foreign Key to Users.Id
        [ForeignKey(nameof(JobSeekerId))]
        public User JobSeeker { get; set; } // Navigation property for the related User

        public string ResumeURL { get; set; } // URL to the resume

        public ApplicationStatus Status { get; set; } // Status of the application

        public DateTime AppliedAt { get; set; } // Timestamp of when the application was submitted
    }

    public enum ApplicationStatus
    {
        Pending,
        Interview,
        Rejected,
        Hired
    }
}