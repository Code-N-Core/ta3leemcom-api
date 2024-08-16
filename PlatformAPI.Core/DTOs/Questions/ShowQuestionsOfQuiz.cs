using PlatformAPI.Core.DTOs.Choose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Questions
{
    public class ShowQuestionsOfQuiz
    {
        public int id {  get; set; }
        public string Content { get; set; }
        public int? AnswerId { get; set; }
        public string? Answer { get; set; }
        public int Mark { get; set; }
        public string? Explain { get; set; }
        public string Type { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public string? attachmentType { get; set; }
        public string? attachmentPath { get; set; }
        public List<ChooseDTO> Choices { get; set; }


    }
}
