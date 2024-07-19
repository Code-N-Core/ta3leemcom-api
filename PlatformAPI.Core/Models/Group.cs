using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50), MinLength(2)]
        public string Name { get; set; }
        public int LevelYearId { get; set; }
        [ForeignKey(nameof(LevelYearId))]
        public virtual LevelYear LevelYear { get; set; }
        public virtual List<Student> Students { get; set; }
        public virtual List<GroupQuiz> GroupQuizzes { get; set; }
        public virtual List<Month> Months { get; set; }
        [Required]
        public int TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; }
    }
}