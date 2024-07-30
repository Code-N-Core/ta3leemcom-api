using PlatformAPI.Core.CustomValidation;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public int GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public Group Group { get; set; }

        public int? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public Parent? Parent { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual List<StudentMonth> StudentMonths { get; set; }
        public virtual List<StudentAbsence> StudentAbsences { get; set; }
        public virtual List<StudentQuiz> StudentQuizs { get; set; }
    }
}