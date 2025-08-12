using TalentHire.Services.IdentityService.DTOs;
namespace TalentHire.Services.IdentityService.Services
{

public interface ITokenService
{
    public Task<string> CreateAccessTokenAsync(UserDto user);
    public Task<string> CreateRefreshTokenAsync(UserDto user);
}

};