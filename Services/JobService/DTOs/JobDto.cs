using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace TalentHire.Services.JobService.DTOs
{
    public class JobDto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } // e.g., IT, Healthcare

        [MaxLength(100)]
        public string Location { get; set; }

        [MaxLength(50)]
        public string SalaryRange { get; set; } // e.g., "$3000 - $5000"

        // 💡 JSON array of skills → switch to List<string> for simplicity & auto-binding
        public List<string> RequiredSkills { get; set; } = new();
        
        // ✅ Foreign Key property
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "EmployerId must be a positive integer.")]
        public int EmployerId { get; set; } // FK to Users table

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Open"; // Default to "Open"
    }
}