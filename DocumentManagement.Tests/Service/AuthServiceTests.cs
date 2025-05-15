using DocumentManagement.Data;
using DocumentManagement.Models.DTO;
using DocumentManagement.Models;
using DocumentManagement.Services;
using DocumentManagement.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace DocumentManagement.Tests.Service
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<AuthService>> _loggerMock = new();
        private readonly JwtSettings _jwtSettings = new()
        {
            Key = "TestKey1234567890!StrongSecretDocTests",
            Issuer = "test-issuer",
            Audience = "test-audience"
        };

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthDbTest")
                .Options;

            _context = new ApplicationDbContext(options);

            var jwtOptionsMock = new Mock<IOptions<JwtSettings>>();
            jwtOptionsMock.Setup(x => x.Value).Returns(_jwtSettings);

            _authService = new AuthService(_context, jwtOptionsMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenValidInput()
        {
            var dto = new RegisterDto
            {
                Username = "newuserxyz",
                Password = "Strong@123",
                Role = "admin"
            };

            var result = await _authService.RegisterAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenUsernameExists()
        {
            var user = new User
            {
                Username = "existing",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass"),
                Role = "admin"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var dto = new RegisterDto
            {
                Username = "existing",
                Password = "Strong@123",
                Role = "admin"
            };

            var result = await _authService.RegisterAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Username already exists", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenRoleIsInvalid()
        {
            var dto = new RegisterDto
            {
                Username = "userx",
                Password = "Strong@123",
                Role = "invalid"
            };

            var result = await _authService.RegisterAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Invalid role specified", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenPasswordIsWeak()
        {
            var dto = new RegisterDto
            {
                Username = "userx",
                Password = "weak",
                Role = "admin"
            };

            var result = await _authService.RegisterAsync(dto);

            Assert.False(result.Success);
            Assert.Contains("Weak Password", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceed_WithValidCredentials()
        {
            var user = new User
            {
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                Role = "viewer"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _authService.LoginAsync(new LoginDto
            {
                Username = "testuser",
                Password = "Test@123"
            });

            Assert.True(result.Success);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task LoginAsync_ShouldFail_WithInvalidCredentials()
        {
            var result = await _authService.LoginAsync(new LoginDto
            {
                Username = "nouser",
                Password = "wrongpass"
            });

            Assert.False(result.Success);
            Assert.Equal("Invalid credentials", result.Message);
        }

        [Fact]
        public async Task SetUserRoleAsync_ShouldSucceed_WhenValid()
        {
            var user = new User { Username = "john", PasswordHash = "xxx", Role = "viewer" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _authService.SetUserRoleAsync(new SetUserRoleDto
            {
                Username = "john",
                Role = "editor"
            });

            Assert.True(result.Success);
            Assert.Contains("assigned", result.Message);
        }

        [Fact]
        public async Task SetUserRoleAsync_ShouldFail_WhenInvalidRole()
        {
            var result = await _authService.SetUserRoleAsync(new SetUserRoleDto
            {
                Username = "john",
                Role = "invalidRole"
            });

            Assert.False(result.Success);
            Assert.Equal("Invalid role specified", result.Message);
        }

        [Fact]
        public async Task SetUserRoleAsync_ShouldFail_WhenUserNotFound()
        {
            var result = await _authService.SetUserRoleAsync(new SetUserRoleDto
            {
                Username = "nouser",
                Role = "admin"
            });

            Assert.False(result.Success);
            Assert.Equal("User not found", result.Message);
        }
    }
}
