namespace PlatformAPI.Core.DTOs.Auth
{
    public class ResendVerificationCodeDTO
    {
        [Required]
        public string Email { get; set; }
    }
}
