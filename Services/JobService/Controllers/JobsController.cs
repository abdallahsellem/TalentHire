using Microsoft.AspNetCore.Mvc;

using TalentHire.Services;
using TalentHire.Services.JobService.Interfaces;
using TalentHire.Services.JobService.DTOs;
using TalentHire.Services.JobService.Models;

namespace TalentHire.Services.JobService.Controllers
{
    [Route("api/jobs")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;


        public JobsController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobDto>>> GetJobs()
        {
            var jobs = await _jobRepository.GetJobsAsync();
            return Ok(jobs);
        }
        [HttpGet("{id}", Name = "GetJob")]
        public async Task<ActionResult<JobDto>> GetJob(int id)
        {
            var job = await _jobRepository.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return Ok(job);
        }
        [HttpPost]
        public async Task<ActionResult<JobDto>> CreateJob(JobDto job)
        {
            if (job == null)
            {
                return BadRequest();
            }
            var createdJob = await _jobRepository.CreateJobAsync(job);
            return CreatedAtAction("GetJob", new { id = createdJob.Id }, createdJob);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<JobDto>> UpdateJob(int id, JobDto job)
        {
            
            var updatedJob = await _jobRepository.UpdateJobAsync(job);
            if (updatedJob == null)
            {
                return NotFound();
            }
            return Ok(updatedJob);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            await _jobRepository.DeleteJobAsync(id);
            return NoContent();
        }
    }
}