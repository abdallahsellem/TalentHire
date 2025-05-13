using TalentHire.Services.IdentityService.Models;

namespace TalentHire.Services.IdentityService.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task SaveRefreshTokenAsync(string username, string refreshToken);
        Task<User> GetUserByRefreshToken(string refreshToken);
        public  Task DeleteRefreshTokenAsync(string username);
    }
}