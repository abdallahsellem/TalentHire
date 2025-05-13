using System.ComponentModel.DataAnnotations;

namespace TalentHire.Services.IdentityService.DTOs
{
    public class CredentialsDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}