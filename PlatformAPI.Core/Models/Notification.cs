using PlatformAPI.Core.CustomValidation;

namespace PlatformAPI.Core.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [MinLength(2),MaxLength(10000)]
        public string Message { get; set; }
        [NotificationTypeDeticated]
        public string Type { get; set; }
        public bool IsReaded { get; set; }
        public virtual List<TeacherNotification> TeacherNotifications { get; set; }
    }
}