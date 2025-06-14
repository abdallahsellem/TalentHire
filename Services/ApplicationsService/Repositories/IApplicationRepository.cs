using TalentHire.Services.ApplicationsService.Models;
using TalentHire.Services.ApplicationsService.DTOs;

namespace TalentHire.Services.ApplicationsService.Repositories
{

    public interface IApplicationRepository
    {
        Task<IEnumerable<Application>> GetAllApplicationsAsync();
        Task<Application?> GetApplicationByIdAsync(int id);
        Task<IEnumerable<Application>> GetApplicationsByJobIdAsync(int jobId);
        Task<IEnumerable<Application>> GetApplicationsByUserIdAsync(int userId);
        Task<Application> CreateApplicationAsync(Application application);
        Task<Application?> UpdateApplicationAsync(Application application);
        Task<bool> UpdateApplicationStatusAsync(int id, ApplicationStatus status, string? reviewerNotes = null);
        Task<bool> DeleteApplicationAsync(int id);
        Task<bool> HasUserAppliedToJobAsync(int userId, int jobId);
        Task<int> GetApplicationCountByJobIdAsync(int jobId);
        Task<int> GetApplicationCountByStatusAsync(ApplicationStatus status);
    }
}
