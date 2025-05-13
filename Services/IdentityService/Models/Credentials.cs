using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalentHire.Services.IdentityService.Models
{
    public class Credentials 
    {
        [Key, ForeignKey("User")] // Primary Key and also Foreign Key
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }
        public DateTime Expiry { get; set; }
    }
}