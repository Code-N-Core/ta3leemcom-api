using PlatformAPI.Core.DTOs.Parent;

namespace PlatformAPI.Core.DTOs.Child
{
    public class ParentChildDTO
    {
        public int ChildId { get; set; }
        public string ChildName { get; set; }
        public IEnumerable<StudentParentDTO> Students { get; set; }
    }
}