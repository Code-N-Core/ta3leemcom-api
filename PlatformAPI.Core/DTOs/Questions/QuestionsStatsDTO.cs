using PlatformAPI.Core.DTOs.Choose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Questions
{
    public class QuestionsStatsDTO
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public int CorrectAnswerCount { get; set; }
        public int IncorrectAnswerCount { get; set; }
        public List<ChooseStatsDTO> Choices { get; set; }

    }
}
