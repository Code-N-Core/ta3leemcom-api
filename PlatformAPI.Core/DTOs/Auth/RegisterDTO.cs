using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs.Auth
{
    public class RegisterDTO
    {

        [Required, MaxLength(50), FullName]
        public string Name { get; set; }
        [Required, MaxLength(50), EmailAddress]
        public string Email { get; set; }
        [Required, Phone]
        public string PhoneNumber { get; set; }
        [Required, MaxLength(100)]
        public string Password { get; set; }
        [Required, RoleDeticated]
        public string Role { get; set; }
        public string? UserName { get; set; }
    }
}