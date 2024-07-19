namespace PlatformAPI.Core.DTOs
{
    public class StudentDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string LevelName { get; set; }
        public string LevelYearName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? StudentParentId { get; set; }
        public string? StudentParentName { get; set; }
        public string? StudentParentPhone { get; set; }
    }
}