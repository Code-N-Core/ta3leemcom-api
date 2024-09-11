using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required,MaxLength(100),MinLength(5),FullName]
        public string Name { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Parent Parent { get; set; }
        public virtual Student Student { get; set; }
        // Add the ResetPasswordCode property
        public string? ResetPasswordCode { get; set; }

        // Add the ResetCodeExpiry property
        public DateTime? ResetCodeExpiry { get; set; }
    }
}