using Microsoft.AspNetCore.Http;
using PlatformAPI.Core.CustomValidation;
using PlatformAPI.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class CreateOffLineQuizDto:CreateQuizDTO
    {
       
        [AllowedExtensions(FileSettings.AllowedExtensions)
           , MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile QuestionForm { get; set; }
        [AllowedExtensions(FileSettings.AllowedExtensions)
           , MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile AnswerForm { get; set; }

       

    }
}
