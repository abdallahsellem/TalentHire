using TalentHire.Services.IdentityService.Interfaces;
using TalentHire.Services.IdentityService.Models;
using TalentHire.Services.IdentityService.Data;
using Microsoft.EntityFrameworkCore;
namespace TalentHire.Services.IdentityService.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public AuthenticationRepository(ApplicationDbContext context, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _context = context;
        }

       public async Task SaveRefreshTokenAsync(string username, string refreshToken)
        {
            var user = await _userRepository.GetUserByUsername(username);
            var credentials = await _context.Credentials.FirstOrDefaultAsync(c => c.UserId == user.Id);
           
            if (credentials == null){
                credentials = new Credentials
                {
                    User = user,
                    RefreshToken = refreshToken,
                    Expiry = DateTime.UtcNow.AddDays(7)
                };
                await _context.Credentials.AddAsync(credentials);
            }
            else{
                credentials.RefreshToken = refreshToken;
                credentials.Expiry = DateTime.UtcNow.AddDays(7);
                _context.Credentials.Update(credentials);
            }
               
                await _context.SaveChangesAsync();
            
        }
         public async Task DeleteRefreshTokenAsync(string username)
         {
            var user = await _userRepository.GetUserByUsername(username);
            var credentials = await _context.Credentials.FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (credentials == null){
                return  ;
            }
            Console.WriteLine("LEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEh");
            Console.WriteLine(credentials.RefreshToken) ;
            Console.WriteLine(user.Id);
            _context.Credentials.Remove(credentials);
            await _context.SaveChangesAsync();
         }
        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {

            var credentials = await _context.Credentials.FirstOrDefaultAsync(c => c.RefreshToken == refreshToken);
            return credentials?.User;
        }
        
        
    }
}