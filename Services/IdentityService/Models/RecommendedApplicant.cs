// using System;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.ComponentModel.DataAnnotations;
// using System.Collections.Generic;
// namespace TalentHire.Models
// {
//     public class RecommendedApplicant
//     {
//         [Key]
//         public int Id { get; set; } // Primary Key

//         public int JobId { get; set; } // Foreign Key to Jobs.Id
//         [ForeignKey("JobId")]
//         public virtual Job Job { get; set; } // Navigation property for Job

//         public int RecommendedUserId { get; set; } // Foreign Key to Users.Id
//         [ForeignKey("RecommendedUserId")]
//         public virtual User RecommendedUser { get; set; } // Navigation property for User

//         public double Score { get; set; } // AI Matching Score
//         public DateTime CreatedAt { get; set; } // Timestamp of creation
//     }
// }