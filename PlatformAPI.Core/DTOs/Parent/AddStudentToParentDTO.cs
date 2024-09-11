namespace PlatformAPI.Core.DTOs.Parent
{
    public class AddStudentToParentDTO
    {
        [Required]
        public string StudentCode { get; set; }
        [Required]
        public int ParentId { get; set; }
    }
}