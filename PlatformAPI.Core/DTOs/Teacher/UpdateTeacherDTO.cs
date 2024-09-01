using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs.Teacher
{
    public class UpdateTeacherDTO
    {
        [Required]
        public int TeacherId { get; set; }
        [Required,FullName]
        public string TeacherName { get; set; }
        [Required]
        public string Phone { get; set; }
    }
}