using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Models
{
    public class LevelYear
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50), MinLength(2)]
        public string Name { get; set; }
        [Required]
        public int LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public virtual Level Level { get; set; }
        public virtual List<Group> Groups { get; set; }
    }
}
