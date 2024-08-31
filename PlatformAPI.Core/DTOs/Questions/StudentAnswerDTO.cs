using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Questions
{
    public class StudentAnswerDTO
    {
        public int id {  get; set; }
        public int questionId { get; set; }
        public int QuestionMark {  get; set; }
        public bool iscorrect { get; set; }
    }
}
