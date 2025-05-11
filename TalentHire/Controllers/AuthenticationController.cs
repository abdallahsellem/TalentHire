using Microsoft.AspNetCore.Mvc;
using TalentHire.Interfaces;
using TalentHire.Models;
using TalentHire.DTOs;
using TalentHire.Services;
using TalentHire.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace TalentHire.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationController( IUserRepository userRepository, ITokenService tokenService,IAuthenticationRepository authenticationRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _authenticationRepository=authenticationRepository;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Credentials>> Login(UserDto user)
        {
            if(await _userRepository.GetUser(user.Username, user.Password) == null)
            {
                return Unauthorized();
            }

            string AccessToken = await _tokenService.CreateAccessTokenAsync(user);
            string RefreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            return Ok(new CredentialsDto
            {
                AccessToken = AccessToken,
                RefreshToken = RefreshToken,
            });
        }
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserDto user)
        {
            await _userRepository.CreateUser(user.Username, user.Password, user.Role);
            return Ok("User created successfully");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            Console.WriteLine("Ahmeeeeeeeeeeed " +username) ;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            await _authenticationRepository.DeleteRefreshTokenAsync(username);
            return Ok(new { message = "Logged out successfully" });
        }
    }
}