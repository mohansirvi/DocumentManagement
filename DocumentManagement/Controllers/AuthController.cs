using DocumentManagement.Models.DTO;
using DocumentManagement.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // We can invalidate token on client side
            return Ok(new { Message = "Logged out successfully" });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("set-role")]
        public async Task<IActionResult> SetUserRole([FromBody] SetUserRoleDto request)
        {
            var result = await _authService.SetUserRoleAsync(request);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
