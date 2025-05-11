using Microsoft.EntityFrameworkCore;
using TalentHire.Data;
using TalentHire.Models;
using TalentHire.Interfaces;
using BCrypt.Net;

namespace TalentHire.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(string username, string password, string role)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                throw new System.Exception("Username already exists.");
            }
            role = role == null ? "User" : role;
            
            // Hash the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            User user = new User
            {
                Username = username,
                Password = hashedPassword,
                Role = (userRoles)Enum.Parse(typeof(userRoles), role)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUser(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
                
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            
            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }



    }
}
