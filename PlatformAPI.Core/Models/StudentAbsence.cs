using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class StudentAbsence
    {
        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }
        public int DayId { get; set; }
        public virtual Day Day { get; set; }
        [Required]
        public bool Attended { get; set; }
    }
}
