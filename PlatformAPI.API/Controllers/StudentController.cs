using Microsoft.AspNetCore.Identity;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(IUnitOfWork unitOfWork,IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(CreateStudentDTO model)
        {
            if(ModelState.IsValid)
            {
                if(await _unitOfWork.Group.GetByIdAsync(model.GroupId)==null)
                    return BadRequest($"there is no group with groupId {model.GroupId}");
                var student=_mapper.Map<Student>(model);
                try
                {
                    do
                    {
                        student.Code = StudentCodeGeneratorService.GenerateCode();
                    }
                    while (await _unitOfWork.Student.FindByCodeAsync(student.Code) is not null);
                    ApplicationUser user;
                    try
                    {
                        user = new ApplicationUser
                        {
                            Email=student.Code+StudentConst.EmailComplete,
                            Name=model.Name,
                            UserName= student.Code
                        };
                        await _userManager.CreateAsync(user, StudentConst.Password);
                        await _userManager.AddToRoleAsync(user,Roles.Student.ToString());
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                    student.ApplicationUserId = user.Id;
                    await _unitOfWork.Student.AddAsync(student);
                    _unitOfWork.Complete();
                    var studentDto = new StudentDTO
                    {
                        Code = student.Code,
                        Name = _userManager.FindByEmailAsync(student.Code +StudentConst.EmailComplete).Result.Name,
                        GroupId = student.GroupId,
                        GroupName = _unitOfWork.Group.GetByIdAsync(student.GroupId).Result.Name,
                        LevelYearName = _unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId).Result.Name,
                        LevelName = _unitOfWork.Level.GetByIdAsync(_unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId).Result.LevelId).Result.Name,
                        StudentParentId=null,
                        StudentParentName=null,
                        StudentParentPhone=null
                    };
                    return Ok(studentDto);
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
                return BadRequest(ModelState);
        }

    }
}