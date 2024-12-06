using Microsoft.AspNetCore.Http;
using PlatformAPI.Core.CustomValidation;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.Settings;

namespace PlatformAPI.Core.DTOs.Questions
{
    public class QDTO
    {
        [MinLength(2),MaxLength(200)]
        public string Content { get; set; }
        [Range(1,100)]
        public int Mark {  get; set; }
        public string? Explain { get; set; }
        [AllowedExtensions(FileSettings.AllowedExtensions)
           , MaxFileSize(FileSettings.MaxFileSizeInBytes)]
         public IFormFile? AttachFile { get; set; }
        public string Type { get; set; }
        public int? QuizId { get; set; }
        public List<ChooseDTO> Choices { get; set; }
        

    }
}
