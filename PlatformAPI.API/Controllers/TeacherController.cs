using Microsoft.AspNetCore.Authorization;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public TeacherController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
            => Ok( await _unitOfWork.Teacher.GetAllAsync());
    }
}