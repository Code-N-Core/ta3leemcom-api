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
    public class CreateOnlineQuizDTO:CreateQuizDTO
    {

        public int Bounce { get; set; }
        public List<QDTO>? Questions { get; set; }
        public timeStart timeStart { get; set; }
        public timeDuration timeDuration { get; set; }


    }
    public class timeStart
    {
        [Range(1,12)]
        public int Hours { get; set; }
        [Range(0,55)]
        public int Minute { get; set; }
        [ModeDeticated]
        public String Mode { get; set; }

    }
    public class timeDuration
    {
        [Range(1, 12)]
        public int Hours { get; set; }
        [Range(0, 55)]
        public int Minute { get; set; }
        [ModeDeticated]
        public String Mode { get; set; }
        [Range(0,7)]
        public int Days {  get; set; }

    }
}
