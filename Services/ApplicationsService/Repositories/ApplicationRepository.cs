using TalentHire.Services.ApplicationsService.Models;
using TalentHire.Services.ApplicationsService.Data;
using Microsoft.EntityFrameworkCore;
using TalentHire.Services.ApplicationsService.DTOs;

namespace TalentHire.Services.ApplicationsService.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Application>> GetAllApplicationsAsync()
    {
        
        return await _context.Applications
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();
    }

    public async Task<Application?> GetApplicationByIdAsync(int id)
    {
        return await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId)
    {
        return await _context.Applications
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetApplicationsByUserIdAsync(int userId)
    {
        return await _context.Applications
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.ApplicationDate)
            .ToListAsync();
    }


    public async Task<Application> CreateApplicationAsync(Application application)
    {
        // Set the application date to current UTC time
        application.ApplicationDate = DateTime.UtcNow;
        application.Status = ApplicationStatus.Pending;

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return application;
    }

    public async Task<Application?> UpdateApplicationAsync(Application application)
    {
        var existingApplication = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == application.Id);

        if (existingApplication == null)
        {
            return null;
        }

        // Update properties
        existingApplication.ApplicantName = application.ApplicantName;
        existingApplication.ApplicantEmail = application.ApplicantEmail;
        existingApplication.ResumeUrl = application.ResumeUrl;
        existingApplication.CoverLetter = application.CoverLetter;
        existingApplication.Status = application.Status;
        existingApplication.ReviewerNotes = application.ReviewerNotes;

        // Update reviewed date if status changed
        if (existingApplication.Status != ApplicationStatus.Pending)
        {
            existingApplication.ReviewedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return existingApplication;
    }

    public async Task<bool> UpdateApplicationStatusAsync(int id, ApplicationStatus status, string? reviewerNotes = null)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
        {
            return false;
        }

        application.Status = status;
        application.ReviewerNotes = reviewerNotes;
        application.ReviewedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteApplicationAsync(int id)
    {
        var application = await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);

        if (application == null)
        {
            return false;
        }

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApplicationExistsAsync(int id)
    {
        return await _context.Applications
            .AnyAsync(a => a.Id == id);
    }

    public async Task<bool> HasUserAppliedToJobAsync(int userId, int jobId)
    {
        return await _context.Applications
            .AnyAsync(a => a.UserId == userId && a.JobId == jobId);
    }

    public async Task<int> GetApplicationCountByJobIdAsync(int jobId)
    {
        return await _context.Applications
            .CountAsync(a => a.JobId == jobId);
    }

    public async Task<int> GetApplicationCountByStatusAsync(ApplicationStatus status)
    {
        return await _context.Applications
            .CountAsync(a => a.Status == status);
    }
}

