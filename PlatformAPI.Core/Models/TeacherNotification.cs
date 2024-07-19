using System.ComponentModel.DataAnnotations.Schema;

namespace PlatformAPI.Core.Models
{
    public class TeacherNotification
    {
        public int TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public Teacher Teacher { get; set; }
        public int NotificationId { get; set; }
        [ForeignKey(nameof(NotificationId))]
        public Notification Notification { get; set; }
        public DateTime Date { get; set; }
    }
}
