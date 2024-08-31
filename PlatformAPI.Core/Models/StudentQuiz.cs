using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class StudentQuiz
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int QuizId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }

        [ForeignKey(nameof(QuizId))]
        public virtual Quiz Quiz { get; set; }

        [Required]
        public int StudentMark { get; set; }
        public int? StudentBounce {  get; set; }

        public bool IsAttend { get; set; }
        public DateTime SubmitAnswerDate { get; set; }

        public virtual List<StudentAnswer> StudentAnswers { get; set; }
    }

}