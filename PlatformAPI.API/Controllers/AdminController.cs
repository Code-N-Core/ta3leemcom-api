using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task DeactivateTeacherAsync(string teacherId)
        {
            var teacher = await _unitOfWork.Teacher.FindTWithExpression<Teacher>(t => t.ApplicationUserId == teacherId);
            if (teacher != null)
            {
                teacher.IsActive = false;
                await _unitOfWork.CompleteAsync();

                var user = await _userManager.FindByIdAsync(teacher.ApplicationUserId);
                if (user != null)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }
            }
        }

    }
}
