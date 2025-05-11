using System.Threading.Tasks;
using TalentHire.Models;
namespace TalentHire.Interfaces
{
     public interface IUserRepository
    {
        Task<User> GetUser(string username, string password);
        Task<User> CreateUser(string username, string password, string role);
        Task<User> GetUserByUsername(string username);

       
    }
}