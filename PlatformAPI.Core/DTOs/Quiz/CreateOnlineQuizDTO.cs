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
        public List<UQDTO>? Questions { get; set; }
        public timeDuration timeDuration { get; set; }


    }
   
    public class timeDuration
    {
        [Range(0, 12)]
        public int Hours { get; set; }
        [Range(0, 59)]
        public int Minute { get; set; }
        [ModeDeticated]
        public String Mode { get; set; }
        [Range(0,7)]
        public int Days {  get; set; }

    }
}
