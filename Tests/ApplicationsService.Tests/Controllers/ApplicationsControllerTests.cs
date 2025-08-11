using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TalentHire.Services.ApplicationsService.DTOs;
using TalentHire.Services.ApplicationsService.Models;
using TalentHire.Services.ApplicationsService.Repositories;
using TalentHire.Services.ApplicationsService.Services;
using ApplicationsService.Controllers;
using IdentityModel;

namespace ApplicationsService.Tests.Controllers
{
    public class ApplicationsControllerTests
    {
        private readonly Mock<IApplicationRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<ApplicationsController>> _mockLogger;
        private readonly Mock<IJobServiceClient> _mockJobServiceClient;
        private readonly ApplicationsController _controller;

        public ApplicationsControllerTests()
        {
            _mockRepository = new Mock<IApplicationRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<ApplicationsController>>();
            _mockJobServiceClient = new Mock<IJobServiceClient>();
            
            _controller = new ApplicationsController(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockJobServiceClient.Object
            );
        }

        private void SetupControllerContext(string role = "User", string userId = "1")
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, role),
                new(JwtClaimTypes.SessionId, userId)
            };
            
            var identity = new ClaimsIdentity(claims, "test");
            var principal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        #region GetApplications Tests

        [Fact]
        public async Task GetApplications_AdminRole_ReturnsAllApplications()
        {
            // Arrange
            SetupControllerContext("Admin");
            var applications = new List<Application> 
            { 
                new() { Id = 1, UserId = 1, JobId = 1 },
                new() { Id = 2, UserId = 2, JobId = 2 }
            };
            var applicationDtos = new List<ApplicationDto>
            {
                new() { Id = 1, UserId = 1, JobId = 1 },
                new() { Id = 2, UserId = 2, JobId = 2 }
            };

            _mockRepository.Setup(r => r.GetAllApplicationsAsync()).ReturnsAsync(applications);
            _mockMapper.Setup(m => m.Map<IEnumerable<ApplicationDto>>(applications)).Returns(applicationDtos);

            // Act
            var result = await _controller.GetApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedApplications = Assert.IsAssignableFrom<IEnumerable<ApplicationDto>>(okResult.Value);
            Assert.Equal(2, returnedApplications.Count());
        }

        [Fact]
        public async Task GetApplications_NonAdminRole_ReturnsForbid()
        {
            // Arrange
            SetupControllerContext("User");

            // Act
            var result = await _controller.GetApplications();

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetApplications_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            SetupControllerContext("Admin");
            _mockRepository.Setup(r => r.GetAllApplicationsAsync()).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetApplications();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region GetApplication Tests

        [Fact]
        public async Task GetApplication_ValidIdAndOwner_ReturnsApplication()
        {
            // Arrange
            var applicationId = 1;
            var userId = 1;
            SetupControllerContext("User", userId.ToString());

            var application = new Application { Id = applicationId, UserId = userId, JobId = 1 };
            var applicationDto = new ApplicationDto { Id = applicationId, UserId = userId, JobId = 1 };

            _mockRepository.Setup(r => r.GetApplicationByIdAsync(applicationId)).ReturnsAsync(application);
            _mockMapper.Setup(m => m.Map<ApplicationDto>(application)).Returns(applicationDto);

            // Act
            var result = await _controller.GetApplication(applicationId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedApplication = Assert.IsType<ApplicationDto>(okResult.Value);
            Assert.Equal(applicationId, returnedApplication.Id);
        }

        [Fact]
        public async Task GetApplication_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var result = await _controller.GetApplication(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid application ID", badRequestResult.Value);
        }

        [Fact]
        public async Task GetApplication_NotFound_ReturnsNotFound()
        {
            // Arrange
            var applicationId = 999;
            SetupControllerContext("User", "1");
            _mockRepository.Setup(r => r.GetApplicationByIdAsync(applicationId)).ReturnsAsync((Application?)null);

            // Act
            var result = await _controller.GetApplication(applicationId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Application with ID {applicationId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetApplication_NotOwnerNotAdmin_ReturnsForbid()
        {
            // Arrange
            var applicationId = 1;
            var userId = 1;
            var otherUserId = 2;
            SetupControllerContext("User", otherUserId.ToString());

            var application = new Application { Id = applicationId, UserId = userId, JobId = 1 };
            _mockRepository.Setup(r => r.GetApplicationByIdAsync(applicationId)).ReturnsAsync(application);

            // Act
            var result = await _controller.GetApplication(applicationId);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetApplication_AdminRole_ReturnsApplicationRegardlessOfOwnership()
        {
            // Arrange
            var applicationId = 1;
            var userId = 1;
            SetupControllerContext("Admin", "999");

            var application = new Application { Id = applicationId, UserId = userId, JobId = 1 };
            var applicationDto = new ApplicationDto { Id = applicationId, UserId = userId, JobId = 1 };

            _mockRepository.Setup(r => r.GetApplicationByIdAsync(applicationId)).ReturnsAsync(application);
            _mockMapper.Setup(m => m.Map<ApplicationDto>(application)).Returns(applicationDto);

            // Act
            var result = await _controller.GetApplication(applicationId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<ApplicationDto>(okResult.Value);
        }

        #endregion

        #region CreateApplication Tests

        [Fact]
        public async Task CreateApplication_ValidRequest_ReturnsCreatedApplication()
        {
            // Arrange
            var userId = 1;
            SetupControllerContext("User", userId.ToString());

            var createDto = new CreateApplicationDto
            {
                UserId = userId,
                JobId = 1,
                ApplicantName = "John Doe",
                ApplicantEmail = "john@example.com",
                ResumeUrl = "https://resume.com",
                CoverLetter = "Cover letter"
            };

            var application = new Application { Id = 1, UserId = userId, JobId = 1 };
            var applicationDto = new ApplicationDto { Id = 1, UserId = userId, JobId = 1 };

            _mockRepository.Setup(r => r.HasUserAppliedToJobAsync(userId, 1)).ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<Application>(createDto)).Returns(application);
            _mockRepository.Setup(r => r.CreateApplicationAsync(It.IsAny<Application>())).ReturnsAsync(application);
            _mockMapper.Setup(m => m.Map<ApplicationDto>(application)).Returns(applicationDto);

            // Act
            var result = await _controller.CreateApplication(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedApplication = Assert.IsType<ApplicationDto>(createdResult.Value);
            Assert.Equal(1, returnedApplication.Id);
        }

        [Fact]
        public async Task CreateApplication_AlreadyApplied_ReturnsConflict()
        {
            // Arrange
            var userId = 1;
            SetupControllerContext("User", userId.ToString());

            var createDto = new CreateApplicationDto { UserId = userId, JobId = 1 };
            _mockRepository.Setup(r => r.HasUserAppliedToJobAsync(userId, 1)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateApplication(createDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("User has already applied to this job", conflictResult.Value);
        }

        [Fact]
        public async Task CreateApplication_UnauthorizedUser_ReturnsForbid()
        {
            // Arrange
            var userId = 1;
            var otherUserId = 2;
            SetupControllerContext("User", otherUserId.ToString());

            var createDto = new CreateApplicationDto { UserId = userId, JobId = 1 };
            _mockRepository.Setup(r => r.HasUserAppliedToJobAsync(userId, 1)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateApplication(createDto);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(result.Result);
        }

        #endregion

        #region UpdateApplicationStatus Tests

        [Fact]
        public async Task UpdateApplicationStatus_ValidRequest_ReturnsNoContent()
        {
            // Arrange
            var applicationId = 1;
            var userId = 1;
            SetupControllerContext("User", userId.ToString());

            var updateDto = new UpdateApplicationStatusDto
            {
                Status = ApplicationStatus.Withdrawn,
                ReviewerNotes = "Withdrawn by applicant"
            };

            var application = new Application { Id = applicationId, UserId = userId, JobId = 1 };
            _mockRepository.Setup(r => r.GetApplicationByIdAsync(applicationId)).ReturnsAsync(application);
            _mockRepository.Setup(r => r.UpdateApplicationStatusAsync(applicationId, updateDto.Status, updateDto.ReviewerNotes))
                          .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateApplicationStatus(applicationId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateApplicationStatus_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = 0;
            var updateDto = new UpdateApplicationStatusDto { Status = ApplicationStatus.Accepted };

            // Act
            var result = await _controller.UpdateApplicationStatus(invalidId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid application ID", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateApplicationStatus_ApplicationNotFound_ReturnsNotFound()
        {
            // Arrange
            var applicationId = 999;
            SetupControllerContext("User", "1");
            var updateDto = new UpdateApplicationStatusDto { Status = ApplicationStatus.Accepted };

            _mockRepository.Setup(r => r.GetApplicationByIdAsync(applicationId)).ReturnsAsync((Application?)null);

            // Act
            var result = await _controller.UpdateApplicationStatus(applicationId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Application with ID {applicationId} not found", notFoundResult.Value);
        }

        #endregion

        #region GetApplicationsByJob Tests

        [Fact]
        public async Task GetApplicationsByJob_JobOwner_ReturnsApplications()
        {
            // Arrange
            var jobId = 1;
            SetupControllerContext("User", "1");

            var applications = new List<Application>
            {
                new() { Id = 1, JobId = jobId, UserId = 2 },
                new() { Id = 2, JobId = jobId, UserId = 3 }
            };

            var applicationDtos = new List<ApplicationDto>
            {
                new() { Id = 1, JobId = jobId, UserId = 2 },
                new() { Id = 2, JobId = jobId, UserId = 3 }
            };

            _mockJobServiceClient.Setup(j => j.IsJobOwnedByUserAsync(jobId)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.GetApplicationsByJobIdAsync(jobId)).ReturnsAsync(applications);
            _mockMapper.Setup(m => m.Map<IEnumerable<ApplicationDto>>(applications)).Returns(applicationDtos);

            // Act
            var result = await _controller.GetApplicationsByJob(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedApplications = Assert.IsAssignableFrom<IEnumerable<ApplicationDto>>(okResult.Value);
            Assert.Equal(2, returnedApplications.Count());
        }

        [Fact]
        public async Task GetApplicationsByJob_InvalidJobId_ReturnsBadRequest()
        {
            // Arrange
            var invalidJobId = 0;

            // Act
            var result = await _controller.GetApplicationsByJob(invalidJobId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid job ID", badRequestResult.Value);
        }

        [Fact]
        public async Task GetApplicationsByJob_NotJobOwnerNotAdmin_ReturnsForbid()
        {
            // Arrange
            var jobId = 1;
            SetupControllerContext("User", "1");

            _mockJobServiceClient.Setup(j => j.IsJobOwnedByUserAsync(jobId)).ReturnsAsync(false);

            // Act
            var result = await _controller.GetApplicationsByJob(jobId);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        #endregion

        #region GetApplicationsByUser Tests

        [Fact]
        public async Task GetApplicationsByUser_ValidUserId_ReturnsApplications()
        {
            // Arrange
            var userId = 1;
            var applications = new List<Application>
            {
                new() { Id = 1, UserId = userId, JobId = 1 },
                new() { Id = 2, UserId = userId, JobId = 2 }
            };

            var applicationDtos = new List<ApplicationDto>
            {
                new() { Id = 1, UserId = userId, JobId = 1 },
                new() { Id = 2, UserId = userId, JobId = 2 }
            };

            _mockRepository.Setup(r => r.GetApplicationsByUserIdAsync(userId)).ReturnsAsync(applications);
            _mockMapper.Setup(m => m.Map<IEnumerable<ApplicationDto>>(applications)).Returns(applicationDtos);

            // Act
            var result = await _controller.GetApplicationsByUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedApplications = Assert.IsAssignableFrom<IEnumerable<ApplicationDto>>(okResult.Value);
            Assert.Equal(2, returnedApplications.Count());
        }

        [Fact]
        public async Task GetApplicationsByUser_InvalidUserId_ReturnsBadRequest()
        {
            // Arrange
            var invalidUserId = 0;

            // Act
            var result = await _controller.GetApplicationsByUser(invalidUserId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid user ID", badRequestResult.Value);
        }

        #endregion

        #region UpdateApplication Tests

        [Fact]
        public async Task UpdateApplication_ValidRequest_ReturnsUpdatedApplication()
        {
            // Arrange
            var applicationId = 1;
            var applicationDto = new ApplicationDto { Id = applicationId, UserId = 1, JobId = 1 };
            var application = new Application { Id = applicationId, UserId = 1, JobId = 1 };

            _mockMapper.Setup(m => m.Map<Application>(applicationDto)).Returns(application);
            _mockRepository.Setup(r => r.UpdateApplicationAsync(application)).ReturnsAsync(application);
            _mockMapper.Setup(m => m.Map<ApplicationDto>(application)).Returns(applicationDto);

            // Act
            var result = await _controller.UpdateApplication(applicationId, applicationDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedApplication = Assert.IsType<ApplicationDto>(okResult.Value);
            Assert.Equal(applicationId, returnedApplication.Id);
        }

        [Fact]
        public async Task UpdateApplication_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = 0;
            var applicationDto = new ApplicationDto { Id = 1 };

            // Act
            var result = await _controller.UpdateApplication(invalidId, applicationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid application ID", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateApplication_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var applicationId = 1;
            var applicationDto = new ApplicationDto { Id = 2 };

            // Act
            var result = await _controller.UpdateApplication(applicationId, applicationDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Application ID mismatch", badRequestResult.Value);
        }

        #endregion

        #region DeleteApplication Tests

        [Fact]
        public async Task DeleteApplication_ValidId_ReturnsNoContent()
        {
            // Arrange
            var applicationId = 1;
            _mockRepository.Setup(r => r.DeleteApplicationAsync(applicationId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteApplication(applicationId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteApplication_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var result = await _controller.DeleteApplication(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid application ID", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteApplication_NotFound_ReturnsNotFound()
        {
            // Arrange
            var applicationId = 999;
            _mockRepository.Setup(r => r.DeleteApplicationAsync(applicationId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteApplication(applicationId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Application with ID {applicationId} not found", notFoundResult.Value);
        }

        #endregion

        #region GetApplicationCountByJob Tests

        [Fact]
        public async Task GetApplicationCountByJob_ValidJobId_ReturnsCount()
        {
            // Arrange
            var jobId = 1;
            var expectedCount = 5;
            _mockRepository.Setup(r => r.GetApplicationCountByJobIdAsync(jobId)).ReturnsAsync(expectedCount);

            // Act
            var result = await _controller.GetApplicationCountByJob(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedCount, okResult.Value);
        }

        [Fact]
        public async Task GetApplicationCountByJob_InvalidJobId_ReturnsBadRequest()
        {
            // Arrange
            var invalidJobId = 0;

            // Act
            var result = await _controller.GetApplicationCountByJob(invalidJobId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid job ID", badRequestResult.Value);
        }

        #endregion

        #region GetApplicationCountByStatus Tests

        [Fact]
        public async Task GetApplicationCountByStatus_ValidStatus_ReturnsCount()
        {
            // Arrange
            var status = ApplicationStatus.Pending;
            var expectedCount = 3;
            _mockRepository.Setup(r => r.GetApplicationCountByStatusAsync(status)).ReturnsAsync(expectedCount);

            // Act
            var result = await _controller.GetApplicationCountByStatus(status);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedCount, okResult.Value);
        }

        #endregion

        #region HasUserAppliedToJob Tests

        [Fact]
        public async Task HasUserAppliedToJob_ValidIds_ReturnsBoolean()
        {
            // Arrange
            var userId = 1;
            var jobId = 1;
            var hasApplied = true;
            _mockRepository.Setup(r => r.HasUserAppliedToJobAsync(userId, jobId)).ReturnsAsync(hasApplied);

            // Act
            var result = await _controller.HasUserAppliedToJob(userId, jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(hasApplied, okResult.Value);
        }

        [Fact]
        public async Task HasUserAppliedToJob_InvalidIds_ReturnsBadRequest()
        {
            // Arrange
            var invalidUserId = 0;
            var invalidJobId = 0;

            // Act
            var result = await _controller.HasUserAppliedToJob(invalidUserId, invalidJobId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid user ID or job ID", badRequestResult.Value);
        }

        #endregion
    }
}
