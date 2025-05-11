using System.ComponentModel.DataAnnotations;

namespace TalentHire.DTOs
{
    public class UserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string ?Role { get; set; }
    }
}