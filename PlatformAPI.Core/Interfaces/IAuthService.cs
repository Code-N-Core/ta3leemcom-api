using PlatformAPI.Core.DTOs;

namespace PlatformAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDTO> RegisterAsync(RegisterDTO model);
        Task<AuthDTO> LoginAsync(LoginDTO model);
    }
}