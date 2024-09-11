namespace PlatformAPI.Core.DTOs.Auth
{
    public class ForgetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
