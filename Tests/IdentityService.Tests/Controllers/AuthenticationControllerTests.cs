using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using TalentHire.Services.IdentityService.Controllers;
using TalentHire.Services.IdentityService.DTOs;
using TalentHire.Services.IdentityService.Interfaces;
using TalentHire.Services.IdentityService.Repositories;
using TalentHire.Services.IdentityService.Services;
using TalentHire.Services.IdentityService.Models; // Add this using statement

namespace IdentityService.Tests.Controllers
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IAuthenticationRepository> _mockAuthRepository;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenService = new Mock<ITokenService>();
            _mockAuthRepository = new Mock<IAuthenticationRepository>();
            
            _controller = new AuthenticationController(
                _mockUserRepository.Object,
                _mockTokenService.Object,
                _mockAuthRepository.Object
            );
        }

        private void SetupControllerContext(string username = "testuser")
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, username),
                new(JwtRegisteredClaimNames.Sub, username)
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

        #region Login Tests

        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokens()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "testuser",
                Password = "password123",
                Role = "User"
            };

            // Use proper User model instead of object
            var mockUser = new User 
            { 
                Username = "testuser", 
                Role = userRoles.User 
            };
            var accessToken = "mock-access-token";
            var refreshToken = "mock-refresh-token";

            _mockUserRepository.Setup(r => r.GetUser(userDto.Username, userDto.Password))
                              .ReturnsAsync(mockUser);
            _mockTokenService.Setup(t => t.CreateAccessTokenAsync(userDto))
                            .ReturnsAsync(accessToken);
            _mockTokenService.Setup(t => t.CreateRefreshTokenAsync(userDto))
                            .ReturnsAsync(refreshToken);

            // Act
            var result = await _controller.Login(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var credentials = Assert.IsType<CredentialsDto>(okResult.Value);
            Assert.Equal(accessToken, credentials.AccessToken);
            Assert.Equal(refreshToken, credentials.RefreshToken);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "testuser",
                Password = "wrongpassword",
                Role = "User"
            };

            _mockUserRepository.Setup(r => r.GetUser(userDto.Username, userDto.Password))
                              .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Login(userDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task Login_EmptyUsername_ReturnsUnauthorized()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "",
                Password = "password123",
                Role = "User"
            };

            _mockUserRepository.Setup(r => r.GetUser(userDto.Username, userDto.Password))
                              .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Login(userDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task Login_EmptyPassword_ReturnsUnauthorized()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "testuser",
                Password = "",
                Role = "User"
            };

            _mockUserRepository.Setup(r => r.GetUser(userDto.Username, userDto.Password))
                              .ReturnsAsync((User?)null);

            // Act
            var result = await _controller.Login(userDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task Login_TokenServiceException_ThrowsException()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "testuser",
                Password = "password123",
                Role = "User"
            };

            var mockUser = new User 
            { 
                Username = "testuser", 
                Role = userRoles.User 
            };
            _mockUserRepository.Setup(r => r.GetUser(userDto.Username, userDto.Password))
                              .ReturnsAsync(mockUser);
            _mockTokenService.Setup(t => t.CreateAccessTokenAsync(userDto))
                            .ThrowsAsync(new Exception("Token creation failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Login(userDto));
        }

        #endregion

        #region Register Tests

        [Fact]
        public async Task Register_ValidUser_ReturnsSuccessMessage()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "newuser",
                Password = "password123",
                Role = "User"
            };

            var createdUser = new User 
            { 
                Username = "newuser", 
                Role = userRoles.User 
            };

            _mockUserRepository.Setup(r => r.CreateUser(userDto.Username, userDto.Password, userDto.Role))
                              .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("User created successfully", okResult.Value);
        }

        [Fact]
        public async Task Register_RepositoryException_ThrowsException()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "newuser",
                Password = "password123",
                Role = "User"
            };

            _mockUserRepository.Setup(r => r.CreateUser(userDto.Username, userDto.Password, userDto.Role))
                              .ThrowsAsync(new Exception("User creation failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Register(userDto));
        }

        [Fact]
        public async Task Register_EmptyUsername_CallsRepository()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "",
                Password = "password123",
                Role = "User"
            };

            var createdUser = new User 
            { 
                Username = "", 
                Role = userRoles.User 
            };

            _mockUserRepository.Setup(r => r.CreateUser(userDto.Username, userDto.Password, userDto.Role))
                              .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("User created successfully", okResult.Value);
            _mockUserRepository.Verify(r => r.CreateUser(userDto.Username, userDto.Password, userDto.Role), Times.Once);
        }

        [Fact]
        public async Task Register_EmptyPassword_CallsRepository()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "newuser",
                Password = "",
                Role = "User"
            };

            var createdUser = new User 
            { 
                Username = "newuser", 
                Role = userRoles.User 
            };

            _mockUserRepository.Setup(r => r.CreateUser(userDto.Username, userDto.Password, userDto.Role))
                              .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("User created successfully", okResult.Value);
            _mockUserRepository.Verify(r => r.CreateUser(userDto.Username, userDto.Password, userDto.Role), Times.Once);
        }

        [Fact]
        public async Task Register_NullRole_CallsRepository()
        {
            // Arrange
            var userDto = new UserDto
            {
                Username = "newuser",
                Password = "password123",
                Role = null
            };

            var createdUser = new User 
            { 
                Username = "newuser", 
                Role = userRoles.User // Default to User role instead of null
            };

            _mockUserRepository.Setup(r => r.CreateUser(userDto.Username, userDto.Password, userDto.Role))
                              .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("User created successfully", okResult.Value);
            _mockUserRepository.Verify(r => r.CreateUser(userDto.Username, userDto.Password, null), Times.Once);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Logout_AuthenticatedUser_ReturnsSuccessMessage()
        {
            // Arrange
            var username = "testuser";
            SetupControllerContext(username);

            _mockAuthRepository.Setup(a => a.DeleteRefreshTokenAsync(username))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);
            
            // Check if the response has a message property
            var messageProperty = response.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            Assert.Equal("Logged out successfully", messageProperty.GetValue(response));
        }

        [Fact]
        public async Task Logout_NoUsernameInClaims_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.Logout();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Logout_EmptyUsernameInClaims_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, ""),
                new(JwtRegisteredClaimNames.Sub, "")
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

            // Act
            var result = await _controller.Logout();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Logout_UsernameFromSubClaim_ReturnsSuccess()
        {
            // Arrange
            var username = "testuser";
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, username)
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

            _mockAuthRepository.Setup(a => a.DeleteRefreshTokenAsync(username))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockAuthRepository.Verify(a => a.DeleteRefreshTokenAsync(username), Times.Once);
        }

        [Fact]
        public async Task Logout_UsernameFromNameIdentifierClaim_ReturnsSuccess()
        {
            // Arrange
            var username = "testuser";
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, username)
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

            _mockAuthRepository.Setup(a => a.DeleteRefreshTokenAsync(username))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockAuthRepository.Verify(a => a.DeleteRefreshTokenAsync(username), Times.Once);
        }

        [Fact]
        public async Task Logout_BothClaimsPresent_PrefersNameIdentifier()
        {
            // Arrange
            var nameIdentifierUsername = "user1";
            var subUsername = "user2";
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, nameIdentifierUsername),
                new(JwtRegisteredClaimNames.Sub, subUsername)
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

            _mockAuthRepository.Setup(a => a.DeleteRefreshTokenAsync(nameIdentifierUsername))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockAuthRepository.Verify(a => a.DeleteRefreshTokenAsync(nameIdentifierUsername), Times.Once);
            _mockAuthRepository.Verify(a => a.DeleteRefreshTokenAsync(subUsername), Times.Never);
        }

        [Fact]
        public async Task Logout_AuthRepositoryException_ThrowsException()
        {
            // Arrange
            var username = "testuser";
            SetupControllerContext(username);

            _mockAuthRepository.Setup(a => a.DeleteRefreshTokenAsync(username))
                              .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Logout());
        }

        #endregion
    }
}