using PlatformAPI.Core.DTOs.Notification;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Interfaces
{
    public interface INotificationRepository :IBaseRepository<Notification>
    {
        public Task<List<TeacherNotificationDto>> GetAllNots(int TeacherId);
        public Task<List<Notification>> GetAllTecNots(int TeacherId);

    }
}
