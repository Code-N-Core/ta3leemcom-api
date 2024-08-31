using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs
{
    public class StudentAnswerSolveDTO
    {
        public int Id { get; set; }
        public int questionId { get; set; }
        public int ChosenId { get; set; }
        public bool IsChoosenCorrect { get; set; }
    }
}
