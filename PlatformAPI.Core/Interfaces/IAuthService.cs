using PlatformAPI.Core.DTOs.Auth;

namespace PlatformAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDTO> RegisterAsync(RegisterDTO model);
        Task<AuthDTO> LoginAsync(LoginDTO model);
    }
}