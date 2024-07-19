using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class Level
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(50),MinLength(2)]
        public string Name { get; set; }
        public virtual List<LevelYear> Years { get; set; }
    }
}