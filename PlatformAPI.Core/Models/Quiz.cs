using PlatformAPI.Core.CustomValidation;

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
        [Required,Deticated]
        public string Type { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool TimeExist { get; set; }
        /*
        * TODO : BasedOnTimeExist
        */
        public int? Time { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual List<StudentQuiz> StudentsQuizzes { get; set; }
        public virtual List<GroupQuiz> GroupsQuizzes { get; set; }
        public virtual List<Question> Questions { get; set; }
    }
}
