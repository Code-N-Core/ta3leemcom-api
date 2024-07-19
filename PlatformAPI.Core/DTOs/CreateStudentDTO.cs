using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs
{
    public class CreateStudentDTO
    {
        [Required, MaxLength(100), MinLength(5), FullName]
        public string Name { get; set; }
        public int GroupId { get; set; }
    }
}
