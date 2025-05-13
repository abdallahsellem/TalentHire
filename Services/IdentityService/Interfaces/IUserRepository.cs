using System.Threading.Tasks;
using TalentHire.Services.IdentityService.Models;
namespace TalentHire.Services.IdentityService.Interfaces
{
     public interface IUserRepository
    {
        Task<User> GetUser(string username, string password);
        Task<User> CreateUser(string username, string password, string role);
        Task<User> GetUserByUsername(string username);

       
    }
}