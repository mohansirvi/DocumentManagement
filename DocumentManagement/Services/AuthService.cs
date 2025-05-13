using DocumentManagement.Data;
using DocumentManagement.Models.DTO;
using DocumentManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentManagement.Utility;
using Microsoft.Extensions.Options;
using DocumentManagement.Services.IService;

namespace DocumentManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly List<string> _validRoles = new() { "admin", "editor", "viewer" };
        private readonly ILogger<AuthService> _logger;

        public AuthService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            try
            {
                if (!_validRoles.Contains(dto.Role))
                {
                    return new AuthResultDto { Success = false, Message = "Invalid role specified" };
                }

                if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                {
                    return new AuthResultDto { Success = false, Message = "Username already exists" };
                }

                if (!IsPasswordStrong(dto.Password))
                {
                    return new AuthResultDto { Success = false, Message = "Weak Password. The password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 8 characters long." };
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = hashedPassword,
                    Role = dto.Role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user);
                return new AuthResultDto { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration");
                return new AuthResultDto { Success = false, Message = "An error occurred. Please try again later." };
            }
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return new AuthResultDto { Success = false, Message = "Invalid credentials" };
            }

            var token = GenerateJwtToken(user);
            return new AuthResultDto { Success = true, Token = token };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthResultDto> SetUserRoleAsync(SetUserRoleDto request)
        {
            try
            {
                if (!_validRoles.Contains(request.Role.ToLower()))
                {
                    return new AuthResultDto { Success = false, Message = "Invalid role specified" };
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                if (user == null)
                {
                    return new AuthResultDto { Success = false, Message = "User not found" };
                }

                user.Role = request.Role.ToLower();
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new AuthResultDto
                {
                    Success = true,
                    Message = $"Role '{request.Role}' assigned to user '{request.Username}'"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting user role");
                return new AuthResultDto { Success = false, Message = "An error occurred. Please try again later." };
            }
        }

        private bool IsPasswordStrong(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(char.IsSymbol);
        }
    }
}
