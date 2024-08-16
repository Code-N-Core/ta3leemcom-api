using PlatformAPI.Core.DTOs.Student;

namespace PlatformAPI.Core.DTOs.Groub
{
    public class GroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? LevelYearId { get; set; }
        public int? TeacherId { get; set; }
        public List<StudentDTO>? Students { get; set; }
    }
}