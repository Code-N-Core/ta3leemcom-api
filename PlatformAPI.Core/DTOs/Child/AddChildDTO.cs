using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs.Child
{
    public class AddChildDTO
    {
        [Required,FullName]
        public string Name { get; set; }
        [Required]
        public int ParentId { get; set; }
    }
}