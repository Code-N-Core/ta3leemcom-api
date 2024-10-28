using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Notification> SaveN(string message,int teacherId,int quizId)
        {
            try
            {
                // Define Egypt's time zone
                TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

                // Get the current time in Egypt
                DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

                var notification = new Notification
                {
                    Message = message,
                    Type = "Server",
                    IsReaded = false,
                    TeacherNotifications = new List<TeacherNotification>
                                {
                                    new TeacherNotification
                                    {
                                        TeacherId = teacherId,
                                        Date = currentDate,
                                        quizId = quizId,
                                    }
                                }
                };
                await _unitOfWork.Notification.AddAsync(notification);
                await _unitOfWork.CompleteAsync();
                return notification;

            }
            catch (Exception)
            {

                throw;
            }
           

        }
    }
}
