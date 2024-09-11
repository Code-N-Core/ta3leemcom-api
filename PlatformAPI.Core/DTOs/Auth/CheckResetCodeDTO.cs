namespace PlatformAPI.Core.DTOs.Auth
{
    public class CheckResetCodeDTO
    {
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required]
        public string ResetCode { get; set; }
    }
}
