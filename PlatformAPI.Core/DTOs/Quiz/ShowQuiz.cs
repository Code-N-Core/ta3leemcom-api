using PlatformAPI.Core.CustomValidation;
using PlatformAPI.Core.DTOs.Questions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class ShowQuiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Mark { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan Duration { get; set; }
        [NotMapped]
        public DateTime? EndDate
        {
            get { return Duration != null ? StartDate.Add(Duration) : null; }
        }

        public string? QuestionForm { get; set; }
        public string? AnswerForm { get; set; }

        [Required]
        public int TeacherId { get; set; }
        public List<int> GroupsIds { get; set; }
        public List<ShowQuestionsOfQuiz> questionsOfQuizzes { get; set; }

    }
}
