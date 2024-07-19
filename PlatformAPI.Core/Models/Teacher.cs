using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual List<TeacherNotification> TeacherNotifications { get; set; }
        public bool IsSubscribed { get; set; }
        public virtual List<Group> Groups { get; set; }
    }
}