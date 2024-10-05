using PlatformAPI.Core.CustomValidation;
using PlatformAPI.Core.DTOs.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Quiz
{
    public class CreateQuizDTO
    {
        [Required, MaxLength(100), MinLength(2)]
        public string Title { get; set; }
        [Required]
        public int Mark { get; set; }
        public DateTime StartDate { get; set; }

        [Required]
        public string Type { get; set; }
        [Required]
        public int TeacherId { get; set; }
        public List<int> GroupsIds { get; set; }
    }
}
