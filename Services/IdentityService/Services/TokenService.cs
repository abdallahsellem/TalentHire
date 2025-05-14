using TalentHire.Services.IdentityService.DTOs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using TalentHire.Services.IdentityService.Interfaces;
using System.Text;
using System.Security.Cryptography;

namespace TalentHire.Services.IdentityService.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationRepository _authenticationRepository;

        private const string JwtSettingsSection = "JwtSettings";
        private const string SecretKey = "SecretKey";
        private const string Issuer = "Issuer";
        private const string Audience = "Audience";
        private const string ExpiryMinutes = "ExpiryMinutes";


        public TokenService(IConfiguration config, IUserRepository userRepository, IAuthenticationRepository authenticationRepository)
        {
            _config = config;
            _userRepository = userRepository;
            _authenticationRepository = authenticationRepository;
        }

        public async Task<string> CreateAccessTokenAsync(UserDto user)
        {
            var jwtSettings = _config.GetSection(JwtSettingsSection);
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings[SecretKey]);

            // Fetch user from repository asynchronously
            var currUser = await _userRepository.GetUser(user.Username, user.Password);
            if (currUser == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Define claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, currUser.Role.ToString()) // Add role claim
            };

            // Create signing credentials
            var key = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: jwtSettings[Issuer],
                audience: jwtSettings[Audience],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings[ExpiryMinutes])),
                signingCredentials: credentials);

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshTokenAsync(UserDto user)
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var refreshToken = Convert.ToBase64String(randomBytes);

            // Save the refresh token in DB with the user, expiration, etc.
            await _authenticationRepository.SaveRefreshTokenAsync(user.Username, refreshToken);

            return refreshToken;
        }

    }
}