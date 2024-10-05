namespace PlatformAPI.Core.Models
{
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public virtual Parent Parent { get; set; }
        public virtual List<Student> Students { get; set;}
    }
}