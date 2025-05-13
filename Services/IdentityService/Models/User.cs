using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using TalentHire.Services.JobService.Models;
namespace TalentHire.Services.IdentityService.Models
{
        public enum userRoles
    {
        User=0,
        Admin=1,
        Employer=2
    }
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }

        public string ?Email { get; set; } // Optional email field

        public userRoles Role { get; set; } = userRoles.User; // Default role is "User"

        public List<Job> Jobs { get; set; } = new(); // Navigation property for jobs posted by the user

        public List<Job> AppliedJobs { get; set; } = new(); // Navigation property for jobs applied by the user

        public List<Credentials> Credentials { get; set; } = new(); // Navigation property for credentials
    }
}