using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using TalentHire.Services.JobService.Controllers;
using TalentHire.Services.JobService.DTOs;
using TalentHire.Services.JobService.Interfaces;

namespace JobService.Tests.Controllers
{
    public class JobsControllerTests
    {
        private readonly Mock<IJobRepository> _mockRepository;
        private readonly JobsController _controller;

        public JobsControllerTests()
        {
            _mockRepository = new Mock<IJobRepository>();
            _controller = new JobsController(_mockRepository.Object);
        }

        #region GetJobs Tests

        [Fact]
        public async Task GetJobs_ReturnsAllJobs()
        {
            // Arrange
            var jobs = new List<JobDto>
            {
                new() { 
                    Id = 1, 
                    Title = "Software Developer", 
                    Description = "Develop software",
                    Category = "IT",
                    Location = "New York",
                    SalaryRange = "$70,000 - $90,000",
                    RequiredSkills = new List<string> { "C#", ".NET" },
                    EmployerId = 1,
                    Status = "Open"
                },
                new() { 
                    Id = 2, 
                    Title = "Data Analyst", 
                    Description = "Analyze data",
                    Category = "Analytics",
                    Location = "Boston",
                    SalaryRange = "$60,000 - $80,000",
                    RequiredSkills = new List<string> { "SQL", "Python" },
                    EmployerId = 2,
                    Status = "Open"
                }
            };

            _mockRepository.Setup(r => r.GetJobsAsync()).ReturnsAsync(jobs);

            // Act
            var result = await _controller.GetJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedJobs = Assert.IsAssignableFrom<IEnumerable<JobDto>>(okResult.Value);
            Assert.Equal(2, returnedJobs.Count());
        }

        [Fact]
        public async Task GetJobs_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var jobs = new List<JobDto>();
            _mockRepository.Setup(r => r.GetJobsAsync()).ReturnsAsync(jobs);

            // Act
            var result = await _controller.GetJobs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedJobs = Assert.IsAssignableFrom<IEnumerable<JobDto>>(okResult.Value);
            Assert.Empty(returnedJobs);
        }

        [Fact]
        public async Task GetJobs_RepositoryException_ThrowsException()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetJobsAsync()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetJobs());
        }

        #endregion

        #region GetJob Tests

        [Fact]
        public async Task GetJob_ValidId_ReturnsJob()
        {
            // Arrange
            var jobId = 1;
            var job = new JobDto 
            { 
                Id = jobId, 
                Title = "Software Developer",
                Description = "Develop software applications",
                Category = "IT",
                Location = "New York",
                SalaryRange = "$70,000 - $90,000",
                RequiredSkills = new List<string> { "C#", ".NET", "SQL" },
                EmployerId = 1,
                Status = "Open"
            };

            _mockRepository.Setup(r => r.GetJobByIdAsync(jobId)).ReturnsAsync(job);

            // Act
            var result = await _controller.GetJob(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedJob = Assert.IsType<JobDto>(okResult.Value);
            Assert.Equal(jobId, returnedJob.Id);
            Assert.Equal("Software Developer", returnedJob.Title);
        }

        [Fact]
        public async Task GetJob_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var jobId = 999;
            _mockRepository.Setup(r => r.GetJobByIdAsync(jobId)).ReturnsAsync((JobDto?)null);

            // Act
            var result = await _controller.GetJob(jobId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetJob_RepositoryException_ThrowsException()
        {
            // Arrange
            var jobId = 1;
            _mockRepository.Setup(r => r.GetJobByIdAsync(jobId)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetJob(jobId));
        }

        #endregion

        #region CreateJob Tests

        [Fact]
        public async Task CreateJob_ValidJob_ReturnsCreatedJob()
        {
            // Arrange
            var jobDto = new JobDto
            {
                Title = "Frontend Developer",
                Description = "Develop user interfaces",
                Category = "IT",
                Location = "San Francisco",
                SalaryRange = "$80,000 - $100,000",
                RequiredSkills = new List<string> { "JavaScript", "React", "CSS" },
                EmployerId = 1,
                Status = "Open"
            };

            var createdJob = new JobDto
            {
                Id = 1,
                Title = "Frontend Developer",
                Description = "Develop user interfaces",
                Category = "IT",
                Location = "San Francisco",
                SalaryRange = "$80,000 - $100,000",
                RequiredSkills = new List<string> { "JavaScript", "React", "CSS" },
                EmployerId = 1,
                Status = "Open"
            };

            _mockRepository.Setup(r => r.CreateJobAsync(jobDto)).ReturnsAsync(createdJob);

            // Act
            var result = await _controller.CreateJob(jobDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedJob = Assert.IsType<JobDto>(createdResult.Value);
            Assert.Equal(1, returnedJob.Id);
            Assert.Equal("Frontend Developer", returnedJob.Title);
            Assert.Equal("GetJob", createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues?["id"]);
        }

        [Fact]
        public async Task CreateJob_NullJob_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CreateJob(null!);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task CreateJob_RepositoryException_ThrowsException()
        {
            // Arrange
            var jobDto = new JobDto
            {
                Title = "Backend Developer",
                Category = "IT",
                EmployerId = 1
            };

            _mockRepository.Setup(r => r.CreateJobAsync(jobDto)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.CreateJob(jobDto));
        }

        [Fact]
        public async Task CreateJob_ValidJobWithMinimalFields_ReturnsCreatedJob()
        {
            // Arrange
            var jobDto = new JobDto
            {
                Title = "Minimal Job",
                Category = "Test",
                EmployerId = 1,
                Status = "Open"
            };

            var createdJob = new JobDto
            {
                Id = 1,
                Title = "Minimal Job",
                Category = "Test",
                EmployerId = 1,
                Status = "Open",
                RequiredSkills = new List<string>()
            };

            _mockRepository.Setup(r => r.CreateJobAsync(jobDto)).ReturnsAsync(createdJob);

            // Act
            var result = await _controller.CreateJob(jobDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedJob = Assert.IsType<JobDto>(createdResult.Value);
            Assert.Equal(1, returnedJob.Id);
        }

        #endregion

        #region UpdateJob Tests

        [Fact]
        public async Task UpdateJob_ValidJob_ReturnsUpdatedJob()
        {
            // Arrange
            var jobId = 1;
            var jobDto = new JobDto
            {
                Id = jobId,
                Title = "Updated Software Developer",
                Description = "Updated description",
                Category = "IT",
                Location = "Updated Location",
                SalaryRange = "$75,000 - $95,000",
                RequiredSkills = new List<string> { "C#", ".NET", "Azure" },
                EmployerId = 1,
                Status = "Open"
            };

            _mockRepository.Setup(r => r.UpdateJobAsync(jobDto)).ReturnsAsync(jobDto);

            // Act
            var result = await _controller.UpdateJob(jobId, jobDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedJob = Assert.IsType<JobDto>(okResult.Value);
            Assert.Equal(jobId, returnedJob.Id);
            Assert.Equal("Updated Software Developer", returnedJob.Title);
        }

        [Fact]
        public async Task UpdateJob_JobNotFound_ReturnsNotFound()
        {
            // Arrange
            var jobId = 999;
            var jobDto = new JobDto
            {
                Id = jobId,
                Title = "Non-existent Job",
                Category = "IT",
                EmployerId = 1
            };

            _mockRepository.Setup(r => r.UpdateJobAsync(jobDto)).ReturnsAsync((JobDto?)null);

            // Act
            var result = await _controller.UpdateJob(jobId, jobDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateJob_RepositoryException_ThrowsException()
        {
            // Arrange
            var jobId = 1;
            var jobDto = new JobDto
            {
                Id = jobId,
                Title = "Test Job",
                Category = "IT",
                EmployerId = 1
            };

            _mockRepository.Setup(r => r.UpdateJobAsync(jobDto)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdateJob(jobId, jobDto));
        }

        [Fact]
        public async Task UpdateJob_IdMismatch_StillCallsRepository()
        {
            // Note: The controller doesn't validate ID mismatch, it just uses the jobDto
            // This test verifies the current behavior
            
            // Arrange
            var jobId = 1;
            var jobDto = new JobDto
            {
                Id = 2, // Different ID
                Title = "Test Job",
                Category = "IT",
                EmployerId = 1
            };

            _mockRepository.Setup(r => r.UpdateJobAsync(jobDto)).ReturnsAsync(jobDto);

            // Act
            var result = await _controller.UpdateJob(jobId, jobDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedJob = Assert.IsType<JobDto>(okResult.Value);
            Assert.Equal(2, returnedJob.Id); // Returns the job with ID from DTO
        }

        #endregion

        #region DeleteJob Tests

        [Fact]
        public async Task DeleteJob_ValidId_ReturnsNoContent()
        {
            // Arrange
            var jobId = 1;
            _mockRepository.Setup(r => r.DeleteJobAsync(jobId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteJob(jobId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepository.Verify(r => r.DeleteJobAsync(jobId), Times.Once);
        }

        [Fact]
        public async Task DeleteJob_RepositoryException_ThrowsException()
        {
            // Arrange
            var jobId = 1;
            _mockRepository.Setup(r => r.DeleteJobAsync(jobId)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.DeleteJob(jobId));
        }

        [Fact]
        public async Task DeleteJob_ZeroId_StillCallsRepository()
        {
            // Note: The controller doesn't validate the ID, it just passes it to the repository
            
            // Arrange
            var jobId = 0;
            _mockRepository.Setup(r => r.DeleteJobAsync(jobId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteJob(jobId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepository.Verify(r => r.DeleteJobAsync(jobId), Times.Once);
        }

        [Fact]
        public async Task DeleteJob_NegativeId_StillCallsRepository()
        {
            // Note: The controller doesn't validate the ID, it just passes it to the repository
            
            // Arrange
            var jobId = -1;
            _mockRepository.Setup(r => r.DeleteJobAsync(jobId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteJob(jobId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockRepository.Verify(r => r.DeleteJobAsync(jobId), Times.Once);
        }

        #endregion

        #region Integration-style Tests

        [Fact]
        public async Task JobWorkflow_CreateThenGetThenUpdateThenDelete_WorksCorrectly()
        {
            // This test simulates a complete workflow
            
            // Arrange
            var jobDto = new JobDto
            {
                Title = "Full Stack Developer",
                Description = "Develop full stack applications",
                Category = "IT",
                Location = "Remote",
                SalaryRange = "$90,000 - $120,000",
                RequiredSkills = new List<string> { "JavaScript", "Node.js", "React", "MongoDB" },
                EmployerId = 1,
                Status = "Open"
            };

            var createdJob = new JobDto
            {
                Id = 1,
                Title = "Full Stack Developer",
                Description = "Develop full stack applications",
                Category = "IT",
                Location = "Remote",
                SalaryRange = "$90,000 - $120,000",
                RequiredSkills = new List<string> { "JavaScript", "Node.js", "React", "MongoDB" },
                EmployerId = 1,
                Status = "Open"
            };

            var updatedJob = new JobDto
            {
                Id = 1,
                Title = "Senior Full Stack Developer",
                Description = "Lead full stack development",
                Category = "IT",
                Location = "Remote",
                SalaryRange = "$100,000 - $130,000",
                RequiredSkills = new List<string> { "JavaScript", "Node.js", "React", "MongoDB", "AWS" },
                EmployerId = 1,
                Status = "Open"
            };

            // Setup mocks
            _mockRepository.Setup(r => r.CreateJobAsync(jobDto)).ReturnsAsync(createdJob);
            _mockRepository.Setup(r => r.GetJobByIdAsync(1)).ReturnsAsync(createdJob);
            _mockRepository.Setup(r => r.UpdateJobAsync(It.IsAny<JobDto>())).ReturnsAsync(updatedJob);
            _mockRepository.Setup(r => r.DeleteJobAsync(1)).Returns(Task.CompletedTask);

            // Act & Assert - Create
            var createResult = await _controller.CreateJob(jobDto);
            var createdResult = Assert.IsType<CreatedAtActionResult>(createResult.Result);
            var returnedCreatedJob = Assert.IsType<JobDto>(createdResult.Value);
            Assert.Equal(1, returnedCreatedJob.Id);

            // Act & Assert - Get
            var getResult = await _controller.GetJob(1);
            var getOkResult = Assert.IsType<OkObjectResult>(getResult.Result);
            var retrievedJob = Assert.IsType<JobDto>(getOkResult.Value);
            Assert.Equal("Full Stack Developer", retrievedJob.Title);

            // Act & Assert - Update
            var updateResult = await _controller.UpdateJob(1, updatedJob);
            var updateOkResult = Assert.IsType<OkObjectResult>(updateResult.Result);
            var returnedUpdatedJob = Assert.IsType<JobDto>(updateOkResult.Value);
            Assert.Equal("Senior Full Stack Developer", returnedUpdatedJob.Title);

            // Act & Assert - Delete
            var deleteResult = await _controller.DeleteJob(1);
            Assert.IsType<NoContentResult>(deleteResult);

            // Verify all repository calls
            _mockRepository.Verify(r => r.CreateJobAsync(It.IsAny<JobDto>()), Times.Once);
            _mockRepository.Verify(r => r.GetJobByIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateJobAsync(It.IsAny<JobDto>()), Times.Once);
            _mockRepository.Verify(r => r.DeleteJobAsync(1), Times.Once);
        }

        #endregion
    }
}
