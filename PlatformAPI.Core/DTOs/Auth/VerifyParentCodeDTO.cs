namespace PlatformAPI.Core.DTOs.Auth
{
    public class VerifyParentCodeDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string VerificationCode { get; set; }
    }
}