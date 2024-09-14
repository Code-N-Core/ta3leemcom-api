using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.Models;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAllNotificationOfTeacher")]
        public async Task<IActionResult> GetAllByTeacherId(int TeacherId)
        {
            var not =await _unitOfWork.Notification.GetAllNots(TeacherId);
            return Ok(not);
        }
        [HttpPut("UpdateAllNotificationsToBeReaded")]
        public async Task<IActionResult> UpdateNotOfTeacher(int TeacherId)
        {

            var notifications =await _unitOfWork.Notification.GetAllTecNots(TeacherId);

            foreach (var notification in notifications)
            {
                notification.IsReaded = true;
            }
            return Ok(notifications);
        }
    }
}
