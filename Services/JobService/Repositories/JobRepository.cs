using TalentHire.Services.JobService.DTOs;
using TalentHire.Services.JobService.Interfaces;
using TalentHire.Services.JobService.Models; // Update this line to the correct namespace
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TalentHire.Services.JobService.Data;
using AutoMapper;
namespace TalentHire.Services.JobService.Repositories
{


    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public JobRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JobDto>> GetJobsAsync()
        {
            return await _context.Jobs
                .Select(job => _mapper.Map<JobDto>(job))
                .ToListAsync();
        }

        public async Task<JobDto> GetJobByIdAsync(int id)
        {
            var job = await _context.Jobs
                .Where(job => job.Id == id)
                .Select(job => _mapper.Map<JobDto>(job))
                .FirstOrDefaultAsync();

            return job ?? throw new KeyNotFoundException($"Job with ID {id} not found.");
        }

        public async Task<JobDto> CreateJobAsync(JobDto job)
        {
            var newJob = _mapper.Map<Job>(job);
            _context.Jobs.Add(newJob);
            await _context.SaveChangesAsync();

            return job;
        }
        public async Task<JobDto> UpdateJobAsync(JobDto job)
        {
            var existingJob = await _context.Jobs.FindAsync(job.Id);
            if (existingJob == null)
            {
                return null;
            }

           existingJob= _mapper.Map(job, existingJob);

            await _context.SaveChangesAsync();

            return _mapper.Map<JobDto>(existingJob);
        }
        public async Task DeleteJobAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
            }
        }
    }
}   