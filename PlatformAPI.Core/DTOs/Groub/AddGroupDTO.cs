namespace PlatformAPI.Core.DTOs.Groub
{
    public class AddGroupDTO
    {
        [Required, MaxLength(50), MinLength(2)]
        public string Name { get; set; }
        [Required]
        public int LevelYearId { get; set; }
        [Required]
        public int TeacherId { get; set; }
    }
}