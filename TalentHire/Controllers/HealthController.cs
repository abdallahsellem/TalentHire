using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TalentHire.Interfaces;
using TalentHire.Models;
namespace TalentHire.Controllers
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