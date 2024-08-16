using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class StudentQuizResult
    {
        public int StudentId { get; set; }

        public int QuizId { get; set; }

        public int StudentMark { get; set; }
        public int? QuizMark { get; set; }
        public bool IsAttend { get; set; }
    }
}
