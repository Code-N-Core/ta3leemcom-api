namespace PlatformAPI.Core.DTOs.Parent
{
    public class EditParentPhoneDTO
    {
        [Required]
        public int Id { get; set; }
        [Required,Phone]
        public string Phone { get; set; }
    }
}
