using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.DTOs.Notification
{
    public class TeacherNotificationDto
    {
        public int TeacherId { get; set; }
        public int NotificationId { get; set; }
        public string NotificationType { get; set; }
        public bool IsReaded { get; set; }
        public DateTime DateTime { get; set; }
        public int? quizId { get; set; }
        public string Message { get; set; }
    }
}
