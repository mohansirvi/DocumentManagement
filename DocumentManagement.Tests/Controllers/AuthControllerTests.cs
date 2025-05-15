using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DocumentManagement.Controllers;
using DocumentManagement.Models.DTO;
using DocumentManagement.Services.IService;

namespace DocumentManagement.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsOk()
        {
            var request = new RegisterDto { Username = "user", Password = "pass" };
            var response = new AuthResultDto { Success = true, Token = "fake-jwt-token" };
            _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(response);

            var result = await _controller.Register(request);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var responseValue = okResult.Value.Should().BeAssignableTo<AuthResultDto>().Subject;
            responseValue.Success.Should().BeTrue();
        }

        [Fact]
        public async Task Register_InvalidRequest_ReturnsBadRequest()
        {
            var request = new RegisterDto { Username = "", Password = "" };
            var response = new AuthResultDto { Success = false, Message = "Invalid data" };
            _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(response);

            var result = await _controller.Register(request);

            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Invalid data");
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var request = new LoginDto { Username = "admin", Password = "pass" };
            var response = new AuthResultDto { Success = true, Token = "valid-token" };
            _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(response);

            var result = await _controller.Login(request);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var auth = okResult.Value.Should().BeAssignableTo<AuthResultDto>().Subject;
            auth.Token.Should().Be("valid-token");
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new LoginDto { Username = "wrong", Password = "invalid" };
            var response = new AuthResultDto { Success = false, Message = "Unauthorized" };
            _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(response);

            var result = await _controller.Login(request);

            var unauthorized = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorized.Value.Should().Be("Unauthorized");
        }

        [Fact]
        public void Logout_ReturnsOk()
        {
            var result = _controller.Logout();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var message = ok.Value.GetType().GetProperty("Message")?.GetValue(ok.Value)?.ToString();
            message.Should().Be("Logged out successfully");
        }

        [Fact]
        public async Task SetUserRole_Success_ReturnsOk()
        {
            var request = new SetUserRoleDto { Username = "user1", Role = "editor" };
            var response = new AuthResultDto { Success = true, Message = "Role set" };
            _mockAuthService.Setup(s => s.SetUserRoleAsync(request)).ReturnsAsync(response);

            var result = await _controller.SetUserRole(request);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var auth = ok.Value.Should().BeAssignableTo<AuthResultDto>().Subject;
            auth.Message.Should().Be("Role set");
        }

        [Fact]
        public async Task SetUserRole_Failure_ReturnsBadRequest()
        {
            var request = new SetUserRoleDto { Username = "user1", Role = "invalidRole" };
            var response = new AuthResultDto { Success = false, Message = "Invalid role" };
            _mockAuthService.Setup(s => s.SetUserRoleAsync(request)).ReturnsAsync(response);

            var result = await _controller.SetUserRole(request);

            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Invalid role");
        }
    }
}
