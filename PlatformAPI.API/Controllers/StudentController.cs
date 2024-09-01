using Microsoft.AspNetCore.Identity;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.DTOs.Student;

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

        [HttpGet("GetAllStudentWithId")]
        public async Task<IActionResult> GetAll(int id)
        {
            var students = await _unitOfWork.Student.FindAllAsync(s=>s.GroupId==id||id==0);
            if (students is null)
                return NotFound($"there is no Group With Id {id}");

            return Ok(students);

        }

        [HttpGet("GetStudent")]
        public async Task<IActionResult> GetById(int id)
        {
            var student =await _unitOfWork.Student.FindTWithIncludes<Student>(id,
                 s=>s.Group,
                 s=>s.Group.LevelYear,
                 s=>s.Group.LevelYear.Level,
                 s=>s.Parent,
                 s=>s.Parent.ApplicationUser
                );
            if (student is null)
                return NotFound($"there is no student with id {id}");
            var s = new StudentDTO()
            {
                Code=student.Code,
                GroupId=student.GroupId,
                GroupName=student.Group.Name,
                Name=student.Code,
                LevelName= student.Group.LevelYear.Level.Name,
                LevelYearName= student.Group.LevelYear.Name,

                StudentParentId=student.Parent!=null?student.Parent.Id:null,
                StudentParentName= student.Parent != null?student.Parent.ApplicationUser.Name:null,
                StudentParentPhone= student.Parent != null?student.Parent.ApplicationUser.PhoneNumber:null,
            };
            return Ok(s);
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
                    await _unitOfWork.CompleteAsync();
                    var studentDto = new StudentDTO
                    {
                        Id=student.Id,   
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
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _unitOfWork.Student.GetByIdAsync(id);

            if (student == null)
            {
                return NotFound();
            }
            await _unitOfWork.Student.DeleteAsync(student);
            await _unitOfWork.CompleteAsync();
            return Ok();
        }
        [HttpPut("Edit")]
        public async Task<IActionResult> Update([FromForm] UpdateStudentDTO student)
        {
            if (ModelState.IsValid)
            {
                var s = await _unitOfWork.Student.GetByIdAsync(student.Id);
                if (s == null) return NotFound($"No Group was found with ID {student.Id}");

                s.GroupId = student.GroupId;
                s = _unitOfWork.Student.Update(s);
                await _unitOfWork.CompleteAsync();
                return Ok(s);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("GetAllResultsOfStudentId")]
        public async Task<IActionResult> GetResults(int id)
        {
            if (id == 0) return BadRequest($"There is no Student With ID: {id}");
            var studentQuizzes = await _unitOfWork.StudentQuiz.FindAllWithIncludes<StudentQuiz>(s => s.StudentId == id,
                Sq => Sq.Quiz
                );
            List<StudentQuizResult> quizsResults = new List<StudentQuizResult>();
            foreach (var sq in studentQuizzes)
            {
                var sqr = new StudentQuizResult
                {
                    StudentId = sq.StudentId,
                    QuizId = sq.QuizId,
                    StudentMark = sq.StudentMark,
                    IsAttend = sq.IsAttend,
                    QuizMark = sq.Quiz.Mark,
                };
                quizsResults.Add(sqr);
            }
            return Ok(quizsResults);
        }

    }
}