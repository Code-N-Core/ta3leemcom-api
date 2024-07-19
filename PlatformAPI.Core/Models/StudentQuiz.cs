using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class StudentQuiz
    {
        public string StudentCode { get; set; }
        [ForeignKey(nameof(StudentCode))]
        public virtual Student Student { get; set; }
        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
        [Required]
        public int StudentMark { get; set; }
    }
}