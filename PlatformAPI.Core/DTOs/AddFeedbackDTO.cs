using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs
{
    public class AddFeedbackDTO
    {
        [Required, MaxLength(500)]
        public string Message { get; set; }
        [Required, StarsDeticated]
        public int Stars { get; set; }
        [Required]
        [RoleDeticatedForFeedback]
        public string UserRole { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
