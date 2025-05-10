using DocumentManagement.Models.DTO;

namespace DocumentManagement.Services.IService
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
    }
}
