using Humanizer;
using Microsoft.AspNetCore.Authorization;
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
        private readonly QuizService _studentQuizService;
        private readonly StudentService _studentService;

        public StudentController(IUnitOfWork unitOfWork, IMapper mapper,
            UserManager<ApplicationUser> userManager, QuizService studentQuizService, StudentService studentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _studentQuizService = studentQuizService;
            _studentService = studentService;
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllStudentOfGroupsIds")]
        public async Task<IActionResult> GetAll([FromQuery]List<int> ids)
        {
           if(ids is null) return BadRequest("The IDs Of Groups is Null");
            var students = await _unitOfWork.Student.FindAllAsync(s=>ids.Contains(s.GroupId));
            if (students is null)
                return NotFound($"There Is No Students");
            List<StudentMapDTO> result = new List<StudentMapDTO>();
            foreach (var student in students)
            {
                var st =await _studentService.GetMapStudntSimple(student);
                result.Add(st);
            }
            return Ok(result);

        }
        [Authorize]
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
            var s =await _studentService.GetMapStudnt(student);
            return Ok(s);
        }
        [Authorize]
        [HttpGet("GetAllResultsOfStudentId")]
        public async Task<IActionResult> GetResults(int id)
        {
            if (id == 0) return BadRequest($"There is no Student With ID: {id}");
            var studentQuizzes = await _unitOfWork.StudentQuiz.FindAllWithIncludes<StudentQuiz>(s => s.StudentId == id,
                Sq => Sq.Quiz
                );
            var res = await _studentQuizService.GetAllStudentQuizResults(studentQuizzes);


            return Ok(res.OrderByDescending(s=>s.Id));
        }
        [Authorize(Roles = "Teacher")]
        [HttpGet("GetAllStudnetDidntEnterQuizId")]
        public async Task<IActionResult> GetStudents(int quizid)
        {
            if (quizid == 0) return NotFound();
            var students = await _unitOfWork.Student.GetStudentNotEnter(quizid);
            List<StudentMapDTO> result = new List<StudentMapDTO>();
            foreach (var student in students)
            {
                var st = await _studentService.GetMapStudntSimple(student);
                result.Add(st);
            }
            return Ok(result);


        }
        [Authorize]
        [HttpGet("GetAllTopStudentOfGroupsIds")]
        public async Task<IActionResult> GetTopStudent([FromQuery]List<int> ids)
        {
            if (ids is null) return BadRequest("The IDs Of Groups is Null");
            var students = await _unitOfWork.Student.GetTopStudents(ids);
            if (students is null)
                return NotFound($"There Is No Students");

            List<StudentMapDTO> result = new List<StudentMapDTO>();
            foreach (var student in students)
            {
                var st = await _studentService.GetMapStudntSimple(student);
                result.Add(st);
            }
            return Ok(result);
        }
        [Authorize(Roles = "Teacher")]
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
                    try
                    {
                        student.ApplicationUserId = user.Id;
                        await _unitOfWork.Student.AddAsync(student);
                        await _unitOfWork.CompleteAsync();
                        // Add to studentMonth for all months
                        var months=await _unitOfWork.Month.FindAllAsync(m=>m.GroupId==model.GroupId);
                        foreach(var month in months)
                        {
                            var studentMonth = new StudentMonth { MonthId = month.Id, Pay = false, StudentId = student.Id };
                            await _unitOfWork.StudentMonth.AddAsync(studentMonth);
                            // Add to studentAbsence for all days
                            var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == month.Id);
                            foreach (var day in days)
                            {
                                var studentAbsenc = new StudentAbsence { DayId = day.Id, Attended = false, StudentId = student.Id };
                                await _unitOfWork.StudentAbsence.AddAsync(studentAbsenc);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(ex);
                    }
                    await _unitOfWork.CompleteAsync();
                    // complete
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
        [Authorize(Roles = "Teacher")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _unitOfWork.Student.GetByIdAsync(id);
            if (student == null)
            {
                return NotFound();

            }
            try
            {

                var months = await _unitOfWork.Month.FindAllAsync(m => m.GroupId == student.GroupId);
                foreach (var m in months)
                {
                    var studentMonth = await _unitOfWork.StudentMonth.FindTWithExpression<StudentMonth>(sm => sm.StudentId == student.Id && sm.MonthId == m.Id);
                    await _unitOfWork.StudentMonth.DeleteAsync(studentMonth);

                    var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == m.Id);
                    foreach (var d in days)
                    {
                        var studentAbsenc = await _unitOfWork.StudentAbsence.FindTWithExpression<StudentAbsence>(sa => sa.StudentId == student.Id && sa.DayId == d.Id);
                        await _unitOfWork.StudentAbsence.DeleteAsync(studentAbsenc);
                    }
                }

                await _unitOfWork.Student.DeleteAsync(student);
                await _unitOfWork.CompleteAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
           
        }
        [Authorize(Roles = "Teacher")]
        [HttpPut("Edit")]
        public async Task<IActionResult> Update([FromForm] UpdateStudentDTO student)
        {
            if (ModelState.IsValid)
            {
                var s = await _unitOfWork.Student.GetByIdAsync(student.Id);
                var User =await _userManager.FindByEmailAsync(s.Code + StudentConst.EmailComplete);
                if (s == null|| User==null)
                    return NotFound($"No Group was found with ID {student.Id} OR There Is No Application User of This Student");
                try
                {
                    bool flag = false;
                    if (student.GroupId != s.GroupId)
                    {
                        flag = true;
                        // delete from last studentMonth and studentAbsences
                        var months = await _unitOfWork.Month.FindAllAsync(m => m.GroupId == s.GroupId);
                        foreach(var m in months)
                        {
                            var studentMonth = await _unitOfWork.StudentMonth.FindTWithExpression<StudentMonth>(sm => sm.StudentId == student.Id && sm.MonthId == m.Id);
                            await _unitOfWork.StudentMonth.DeleteAsync(studentMonth);
                            
                            var days=await _unitOfWork.Day.FindAllAsync(d=>d.MonthId==m.Id);
                            foreach(var d in days)
                            {
                                var studentAbsenc = await _unitOfWork.StudentAbsence.FindTWithExpression<StudentAbsence>(sa => sa.StudentId == student.Id && sa.DayId == d.Id);
                                await _unitOfWork.StudentAbsence.DeleteAsync(studentAbsenc);
                            }
                        }

                    }
                    // update
                    try
                    {
                        s.GroupId = student.GroupId;
                        User.Name = student.Name;
                        var result = await _userManager.UpdateAsync(User);

                        s = _unitOfWork.Student.Update(s);

                    }
                    catch(Exception ex) 
                    {
                        return BadRequest(ex);
                    }
                    await _unitOfWork.CompleteAsync();

                    if (flag)
                    {
                        // Add to studentMonth for all months
                        var months = await _unitOfWork.Month.FindAllAsync(m => m.GroupId == s.GroupId);
                        foreach (var month in months)
                        {
                            var studentMonth = new StudentMonth { MonthId = month.Id, Pay = false, StudentId = student.Id };
                            await _unitOfWork.StudentMonth.AddAsync(studentMonth);
                            // Add to studentAbsence for all days
                            var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == month.Id);
                            foreach (var day in days)
                            {
                                var studentAbsenc = new StudentAbsence { DayId = day.Id, Attended = false, StudentId = student.Id };
                                await _unitOfWork.StudentAbsence.AddAsync(studentAbsenc);
                            }
                        }
                    }
                    await _unitOfWork.CompleteAsync();

                    return Ok("Student updated successfully");
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
               
            }
            else
                return BadRequest(ModelState);
        }

    }
}