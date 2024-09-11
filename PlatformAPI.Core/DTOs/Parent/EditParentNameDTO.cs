using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.DTOs.Parent
{
    public class EditParentNameDTO
    {
        [Required]
        public int Id { get; set; }
        [Required,FullName]
        public string Name { get; set; }
    }
}