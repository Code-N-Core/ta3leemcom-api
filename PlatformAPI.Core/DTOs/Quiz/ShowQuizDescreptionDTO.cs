using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class ShowQuizDescreptionDTO
    {
        public int id {  get; set; }
        public string Title { get; set; }
        public int Mark {  get; set; }
        public int Bounce { get; set; }
        public int NumQuestions { get; set; }
        public int NumStuedntsSolveQuiz { get; set; }
        public int NumStuedntsNotSolveQuiz { get; set; }
        public List<QuestionsDes> QuestionsDes { get; set; }
    }

    public class QuestionsDes
    {
        public int id { get; set; }
        public string Type { get; set; }
        public int Mark { get; set; }
    }
}
