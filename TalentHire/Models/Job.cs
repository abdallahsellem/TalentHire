using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace TalentHire.Models
{
    public enum JobStatus
    {
        Open,
        Closed,
        Pending
    }
    public class Job
    {
        [Key]
        public int Id { get; set; } // Primary Key

       
        public string Title { get; set; }

        
        public string Description { get; set; }

       
        public string Category { get; set; } // e.g., IT, Healthcare

        
        public string Location { get; set; }

        
        public string SalaryRange { get; set; } // e.g., "$3000 - $5000"

        // 💡 JSON array of skills → switch to List<string> for simplicity & auto-binding
        public List<string> RequiredSkills { get; set; } = new();

         // ✅ Foreign Key property
        public int EmployerId { get; set; }

        // ✅ Navigation property to User
        [ForeignKey(nameof(EmployerId))]
        public User Employer { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Open; // Default to "Open"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Set default to current time
    }
}
