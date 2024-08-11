using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(500)]
        public string Message { get; set; }
        [Required,StarsDeticated]
        public int Stars { get; set; }
        [Required]
        [RoleDeticatedForFeedback]
        public string UserRole { get; set; }
        [Required]
        public string Name { get; set; }
    }
}