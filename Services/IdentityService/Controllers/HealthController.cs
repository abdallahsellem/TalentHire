using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentHire.Services.IdentityService.Interfaces;
using TalentHire.Services.IdentityService.DTOs;
namespace TalentHire.Services.IdentityService.Controllers
{

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }
    [HttpPost]
    [Authorize]
    public IActionResult Post()
    {
        return Ok("Healthy");
    }
}
} ;