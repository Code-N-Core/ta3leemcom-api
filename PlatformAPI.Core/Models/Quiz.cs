using PlatformAPI.Core.CustomValidation;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }
        [Required,MaxLength(100),MinLength(2)]
        public string Title { get; set; }
        [Required]
        public int Mark { get; set; }
        public int? Bounce { get; set; }
        [Required,Deticated]
        public string Type { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        //online
        public TimeSpan Duration { get; set; }
        [NotMapped]
        public DateTime? EndDate 
        { 
            get {return StartDate.Add(Duration); }
        }

        //offline
        public string? QuestionForm { get; set; }
        public string? AnswerForm { get; set; }

        [Required]
        public int TeacherId { get; set; }
        public bool IsNotfy { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual List<StudentQuiz> StudentsQuizzes { get; set; }
        public virtual List<GroupQuiz> GroupsQuizzes { get; set; }
        public virtual List<Question> Questions { get; set; }
    }
}
