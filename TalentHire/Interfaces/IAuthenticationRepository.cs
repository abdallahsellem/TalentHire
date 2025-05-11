using TalentHire.Models;

namespace TalentHire.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task SaveRefreshTokenAsync(string username, string refreshToken);
        Task<User> GetUserByRefreshToken(string refreshToken);
        public  Task DeleteRefreshTokenAsync(string username);
    }
}