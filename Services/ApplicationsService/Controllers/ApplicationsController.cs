using TalentHire.Services.ApplicationsService.DTOs;
using TalentHire.Services.ApplicationsService.Models;
using TalentHire.Services.ApplicationsService.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using TalentHire.Services.ApplicationsService.Services;

namespace TalentHire.Services.ApplicationsService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ApplicationsController> _logger;
    private readonly IJobServiceClient _jobServiceClient;

    public ApplicationsController(
        IApplicationRepository applicationRepository,
        IMapper mapper,
        ILogger<ApplicationsController> logger, 
        IJobServiceClient jobServiceClient)
    {
        _applicationRepository = applicationRepository;
        _mapper = mapper;
        _logger = logger;
        _jobServiceClient = jobServiceClient;
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(JwtClaimTypes.SessionId);
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private bool IsAdmin()
    {
        return User.FindFirstValue(ClaimTypes.Role) == "Admin";
    }

    /// <summary>
    /// Get all applications
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplications()
    {
        try
        {
            var applications = await _applicationRepository.GetAllApplicationsAsync();
            var applicationDtos = _mapper.Map<IEnumerable<ApplicationDto>>(applications);
            return Ok(applicationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get application by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDto>> GetApplication(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid application ID");
        }

        try
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound($"Application with ID {id} not found");
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && (currentUserId == null || application.UserId != currentUserId))
            {
                return Forbid("You are not authorized to access this application");
            }

            var applicationDto = _mapper.Map<ApplicationDto>(application);
            return Ok(applicationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new application
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> CreateApplication(CreateApplicationDto createDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Forbid("User ID not found in token");
            }

            // Check if user has already applied to this job
            var hasApplied = await _applicationRepository.HasUserAppliedToJobAsync(currentUserId.Value, createDto.JobId);
            if (hasApplied)
            {
                return Conflict("User has already applied to this job");
            }

            var application = _mapper.Map<Application>(createDto);
            application.UserId = currentUserId.Value;
            
            var createdApplication = await _applicationRepository.CreateApplicationAsync(application);
            var applicationDto = _mapper.Map<ApplicationDto>(createdApplication);

            _logger.LogInformation("Created application {Id} for job {JobId} by user {UserId}",
                createdApplication.Id, createDto.JobId, currentUserId.Value);

            return CreatedAtAction(nameof(GetApplication), new { id = applicationDto.Id }, applicationDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update application status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateApplicationStatus(int id, UpdateApplicationStatusDto updateDto)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid application ID");
        }

        try
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound($"Application with ID {id} not found");
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && (currentUserId == null || application.UserId != currentUserId))
            {
                return Forbid("You are not authorized to update this application status");
            }

            var success = await _applicationRepository.UpdateApplicationStatusAsync(id, updateDto.Status, updateDto.ReviewerNotes);
            if (!success)
            {
                return NotFound($"Application with ID {id} not found");
            }

            _logger.LogInformation("Updated application {Id} status to {Status}", id, updateDto.Status);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application {Id} status", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get applications by job ID
    /// </summary>
    [HttpGet("job/{jobId}")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplicationsByJob(int jobId)
    {
        if (jobId <= 0)
        {
            return BadRequest("Invalid job ID");
        }

        try
        {
            if (!IsAdmin())
            {
                var isJobOwner = await _jobServiceClient.IsJobOwnedByUserAsync(jobId);
                if (!isJobOwner)
                {
                    return Forbid("You are not authorized to view applications for this job");
                }
            }

            var applications = await _applicationRepository.GetApplicationsByJobIdAsync(jobId);
            var applicationDtos = _mapper.Map<IEnumerable<ApplicationDto>>(applications);
            return Ok(applicationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications for job {JobId}", jobId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get applications by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetApplicationsByUser(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest("Invalid user ID");
        }

        try
        {
            var currentUserId = GetCurrentUserId();
            
            // Users can only view their own applications unless they are admin
            if (!IsAdmin() && (currentUserId == null || currentUserId != userId))
            {
                return Forbid("You can only view your own applications");
            }

            var applications = await _applicationRepository.GetApplicationsByUserIdAsync(userId);
            var applicationDtos = _mapper.Map<IEnumerable<ApplicationDto>>(applications);
            return Ok(applicationDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing application
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationDto>> UpdateApplication(int id, ApplicationDto applicationDto)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid application ID");
        }

        if (id != applicationDto.Id)
        {
            return BadRequest("Application ID mismatch");
        }

        try
        {
            var existingApplication = await _applicationRepository.GetApplicationByIdAsync(id);
            if (existingApplication == null)
            {
                return NotFound($"Application with ID {id} not found");
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && (currentUserId == null || existingApplication.UserId != currentUserId))
            {
                return Forbid("You are not authorized to update this application");
            }

            var application = _mapper.Map<Application>(applicationDto);
            var updatedApplication = await _applicationRepository.UpdateApplicationAsync(application);

            var updatedDto = _mapper.Map<ApplicationDto>(updatedApplication);
            _logger.LogInformation("Updated application {Id}", id);
            return Ok(updatedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete an application
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteApplication(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid application ID");
        }

        try
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(id);
            if (application == null)
            {
                return NotFound($"Application with ID {id} not found");
            }

            var currentUserId = GetCurrentUserId();
            if (!IsAdmin() && (currentUserId == null || application.UserId != currentUserId))
            {
                return Forbid("You are not authorized to delete this application");
            }

            var success = await _applicationRepository.DeleteApplicationAsync(id);
            if (!success)
            {
                return NotFound($"Application with ID {id} not found");
            }

            _logger.LogInformation("Deleted application {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting application {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get application count by job ID
    /// </summary>
    [HttpGet("job/{jobId}/count")]
    public async Task<ActionResult<int>> GetApplicationCountByJob(int jobId)
    {
        if (jobId <= 0)
        {
            return BadRequest("Invalid job ID");
        }

        try
        {
            if (!IsAdmin())
            {
                var isJobOwner = await _jobServiceClient.IsJobOwnedByUserAsync(jobId);
                if (!isJobOwner)
                {
                    return Forbid("You are not authorized to view application count for this job");
                }
            }

            var count = await _applicationRepository.GetApplicationCountByJobIdAsync(jobId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application count for job {JobId}", jobId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get application count by status (Admin only)
    /// </summary>
    [HttpGet("status/{status}/count")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<int>> GetApplicationCountByStatus(ApplicationStatus status)
    {
        try
        {
            var count = await _applicationRepository.GetApplicationCountByStatusAsync(status);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application count for status {Status}", status);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if current user has applied to a job
    /// </summary>
    [HttpGet("job/{jobId}/applied")]
    public async Task<ActionResult<bool>> HasCurrentUserAppliedToJob(int jobId)
    {
        if (jobId <= 0)
        {
            return BadRequest("Invalid job ID");
        }

        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Forbid("User ID not found in token");
            }

            var hasApplied = await _applicationRepository.HasUserAppliedToJobAsync(currentUserId.Value, jobId);
            return Ok(hasApplied);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if current user applied to job {JobId}", jobId);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Check if specific user has applied to a job (Admin only)
    /// </summary>
    [HttpGet("user/{userId}/job/{jobId}/exists")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> HasUserAppliedToJob(int userId, int jobId)
    {
        if (userId <= 0 || jobId <= 0)
        {
            return BadRequest("Invalid user ID or job ID");
        }

        try
        {
            var hasApplied = await _applicationRepository.HasUserAppliedToJobAsync(userId, jobId);
            return Ok(hasApplied);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} applied to job {JobId}", userId, jobId);
            return StatusCode(500, "Internal server error");
        }
    }
}