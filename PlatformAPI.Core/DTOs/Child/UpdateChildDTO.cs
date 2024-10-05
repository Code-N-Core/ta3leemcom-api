using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs.Child
{
    public class UpdateChildDTO
    {
        [Required]
        public int ChildId { get; set; }
        [Required,FullName]
        public string Name { get; set; }
        [Required]
        public int ParentId { get; set; }
    }
}