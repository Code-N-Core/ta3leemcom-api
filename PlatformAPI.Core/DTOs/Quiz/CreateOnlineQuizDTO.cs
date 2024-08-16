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
     
        //online
        public int Duration { get; set; }
        public DateTime EndDate
        {
            get { return StartDate.AddMinutes(Duration); }
        }


    }
}
