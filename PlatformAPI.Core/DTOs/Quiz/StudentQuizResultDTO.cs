using PlatformAPI.Core.DTOs.Questions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class StudentQuizResultDTO
    {
        public int Id {  get; set; }
        public int QuizId { get; set; }
        public string? StuentName { get; set; }
        public int StudentMark { get; set; }
        public int? StudentBounce { get; set; }
        public int QuizMark { get; set; }
        public int? QuizBounce { get; set; }

        public bool IsAttend { get; set; }
        public DateTime SubmitAnswerTime { get; set; }
        public List<StudentAnswerDTO> Answers {  get; set; }
    }
}
