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

        public async Task SaveN(string message,int teacherId,int quizId)
        {
            try
            {
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
                                        Date = DateTime.UtcNow,
                                        quizId = quizId,
                                    }
                                }
                };
                await _unitOfWork.Notification.AddAsync(notification);

            }
            catch (Exception)
            {

                throw;
            }
            await _unitOfWork.CompleteAsync();

        }
    }
}
