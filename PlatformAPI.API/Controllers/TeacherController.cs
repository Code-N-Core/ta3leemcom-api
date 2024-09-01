using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PlatformAPI.Core.DTOs.Teacher;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Teacher")]
    public class TeacherController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public TeacherController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
            => Ok( await _unitOfWork.Teacher.GetAllAsync());
        [HttpGet("GetTeacherInformation")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var teacher = await _unitOfWork.Teacher.GetByIdAsync(id);
            if (teacher == null)
                return NotFound($"No teacher with id: {id}");
            var user = await _userManager.FindByIdAsync(teacher.ApplicationUserId);
            var info = new TeacherInfoDTO
            {
                TeacherId=teacher.Id,
                Name=user.Name,
                Email=user.Email,
                Phone=user.PhoneNumber
            };
            return Ok(info);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(UpdateTeacherDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if(await _unitOfWork.Teacher.GetByIdAsync(model.TeacherId)==null)
                return BadRequest($"No teacher with id: {model.TeacherId}");
            var updated = await _userManager.FindByIdAsync(_unitOfWork.Teacher.GetByIdAsync(model.TeacherId).Result.ApplicationUserId);
            updated.PhoneNumber = model.Phone;
            updated.Name = model.TeacherName;
            await _userManager.UpdateAsync(updated);
            var infoUpdated=new TeacherInfoDTO { Name=updated.Name,Phone=updated.PhoneNumber,Email=updated.Email,TeacherId=model.TeacherId };
            return Ok(infoUpdated);
        }
    }
}