using TalentHire.Services.JobService.Models; // Update this line to the correct namespace
using System.Threading.Tasks;
using TalentHire.Services.JobService.DTOs;


namespace TalentHire.Services.JobService.Interfaces
{


    public interface IJobRepository
    {
        Task<IEnumerable<JobDto>> GetJobsAsync();
        Task<JobDto> GetJobByIdAsync(int id);
        Task<JobDto> CreateJobAsync(JobDto job);
        Task<JobDto> UpdateJobAsync(JobDto job);
        Task DeleteJobAsync(int id);
    }
}