using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs.Auth
{
    public class ChangePasswordDTO
    {
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required,PasswordComplexity]
        public string NewPassword { get; set; }
    }
}
