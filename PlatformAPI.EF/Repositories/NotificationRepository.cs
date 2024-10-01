using Microsoft.EntityFrameworkCore;
using PlatformAPI.Core.DTOs.Notification;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.EF.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<TeacherNotificationDto>> GetAllNots(int TeacherId)
        {
            var nots =await(  from n in _context.Notifications
                       join tn in _context.TeachersNotifications
                       on n.Id equals tn.NotificationId
                       where tn.TeacherId == TeacherId
                       select new TeacherNotificationDto
                       {
                           TeacherId = TeacherId,
                           NotificationId=n.Id,
                           DateTime = tn.Date,
                           IsReaded=n.IsReaded,
                           Message=n.Message,
                           NotificationType= n.Type,
                           quizId = tn.quizId
                           
                       }).ToListAsync();
            if (nots is null ||!nots.Any())
            {
                return null;
            }
            return nots;
        }
        public async Task<List<Notification>> GetAllTecNots(int TeacherId)
        {
            var notifications =await _context.Notifications.Include(n=>n.TeacherNotifications)
               .Where(n => n.TeacherNotifications.Any(tn=>tn.TeacherId == TeacherId)  && !n.IsReaded)
               .ToListAsync();
            return notifications;
        }

    }
}
