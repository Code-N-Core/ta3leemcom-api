using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Models
{
    public class StudentAnswer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StudentQuizId { get; set; }
        [ForeignKey(nameof(StudentQuizId))]
        public virtual StudentQuiz StudentQuiz { get; set; }


        [Required]
        public int QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public virtual Question Question { get; set; }

        [Required]
        public int ChosenOptionId { get; set; }
        [ForeignKey(nameof(ChosenOptionId))]
        public virtual Choose ChosenOption { get; set; }

        public bool IsCorrect { get; set; }
    }
}
