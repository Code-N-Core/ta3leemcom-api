namespace PlatformAPI.Core.DTOs
{
    public class AddToRoleDTO
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
