using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlatformAPI.Core.DTOs.Parent;
using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.DTOs.Student;
using PlatformAPI.Core.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
            var loggedInTeacherId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInTeacherId))
            {
                return Unauthorized("User not found");
            }
            var groups = (await _unitOfWork.Group.GetAllAsync()).Where(g => ids.Contains(g.Id));
            if (groups == null)
            {
                return NotFound("Group not found");
            }

            if (groups.Any(g => g.TeacherId != int.Parse(loggedInTeacherId)))
            {
                return BadRequest("You do not have permission to get These groups");
            }


            if (ids is null) return BadRequest("The IDs Of Groups is Null");
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
        [Authorize(Roles="Teacher,Student,Parent")]
        [HttpGet("GetStudent")]
        public async Task<IActionResult> GetById(int id)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (userRoles.Contains(Roles.Teacher.ToString()))
            {
                var studentGroupId = (await _unitOfWork.Student.GetByIdAsync(id)).GroupId;
                var TeacherOfStudent = ((await _unitOfWork.Group.GetAllAsync()).Where(g => g.Id == studentGroupId)).Select(g => g.TeacherId).FirstOrDefault();
                if (TeacherOfStudent != int.Parse(loggedInId))
                    return BadRequest("You Do Not Have Permission");
            }
            else if (userRoles.Contains(Roles.Student.ToString()))
            {
                if(id != int.Parse(loggedInId))
                    return BadRequest("You Do Not Have Permission");

            }
            else if (userRoles.Contains(Roles.Parent.ToString()))
            {
                var parentId = (await _unitOfWork.Student.GetByIdAsync(id)).ParentId;
                if (parentId != int.Parse(loggedInId))
                    return BadRequest("You Do Not Have Permission");
            }

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
            string parentName = null;
            string parentPhone = null;
            if (s.StudentParentId != null)
            {
                int pId = s.StudentParentId ?? 0;
                parentName = _userManager.FindByIdAsync(_unitOfWork.Parent.GetByIdAsync(pId).Result.ApplicationUserId).Result.Name;
                parentPhone = _userManager.FindByIdAsync(_unitOfWork.Parent.GetByIdAsync(pId).Result.ApplicationUserId).Result.PhoneNumber;
            }
            return Ok(new
            {
                id = s.Id,
                name = _userManager.FindByIdAsync(_unitOfWork.Student.GetByIdAsync(s.Id).Result.ApplicationUserId).Result.Name,
                code = s.Code,
                groupId = s.GroupId,
                groupName = _unitOfWork.Group.GetByIdAsync(s.GroupId).Result.Name,
                levelYearId = _unitOfWork.Group.GetByIdAsync(s.GroupId).Result.LevelYearId,
                levelYearName = _unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(s.GroupId).Result.LevelYearId).Result.Name,
                levelId = _unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(s.GroupId).Result.LevelYearId).Result.LevelId,
                levelName = _unitOfWork.Level.GetByIdAsync(_unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(s.GroupId).Result.LevelYearId).Result.LevelId).Result.Name,
                studentParentId = s.StudentParentId,
                studentParentName = parentName,
                studentParentPhone = parentPhone
            });
        }
        [Authorize(Roles = "Teacher,Student,Parent")]
        [HttpGet("GetAllResultsOfStudentId")]
        public async Task<IActionResult> GetResults(int id)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (userRoles.Contains(Roles.Teacher.ToString()))
            {
                var studentGroupId = (await _unitOfWork.Student.GetByIdAsync(id)).GroupId;
                var TeacherOfStudent = ((await _unitOfWork.Group.GetAllAsync()).Where(g => g.Id == studentGroupId)).Select(g => g.TeacherId).FirstOrDefault();
                if (TeacherOfStudent != int.Parse(loggedInId))
                    return BadRequest("You Do Not Have Permission");
            }
            else if (userRoles.Contains(Roles.Student.ToString()))
            {
                if (id != int.Parse(loggedInId))
                    return BadRequest("You Do Not Have Permission");

            }
            else if (userRoles.Contains(Roles.Parent.ToString()))
            {
                var parentId = (await _unitOfWork.Student.GetByIdAsync(id)).ParentId;
                if (parentId != int.Parse(loggedInId))
                    return BadRequest("You Do Not Have Permission");
            }

            /////////////////////////////////////////

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
            var loggedInId = User.FindFirst("LoggedId")?.Value;
            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            var quiz = (await _unitOfWork.Quiz.GetByIdAsync(quizid));
            if (quiz == null)
            {
                return NotFound("Quiz not found");
            }

            if (quiz.TeacherId != int.Parse(loggedInId))
            {
                return BadRequest("You do not have permission to get this Quiz");
            }

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
                var loggedInId = User.FindFirst("LoggedId")?.Value;

                if (string.IsNullOrEmpty(loggedInId))
                {
                    return Unauthorized("User not found");
                }
                
                    var TeacherOfStudent = (await _unitOfWork.Group.GetByIdAsync(model.GroupId)).TeacherId;
                    if (TeacherOfStudent != int.Parse(loggedInId))
                        return BadRequest("You Do Not Have Permission");
                




                ////////////////////////////////
                if (await _unitOfWork.Group.GetByIdAsync(model.GroupId)==null)
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
                    return Ok(new
                    {
                        id = student.Id,
                        name = _userManager.FindByIdAsync(_unitOfWork.Student.GetByIdAsync(student.Id).Result.ApplicationUserId).Result.Name,
                        code = student.Code,
                        groupId = student.GroupId,
                        groupName = _unitOfWork.Group.GetByIdAsync(student.GroupId).Result.Name,
                        levelYearId = _unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId,
                        levelYearName = _unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId).Result.Name,
                        levelId = _unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId).Result.LevelId,
                        levelName = _unitOfWork.Level.GetByIdAsync(_unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId).Result.LevelId).Result.Name,
                        studentParentId = (int?)null,
                        studentParentName = (string)null,
                        studentParentPhone = (string)null
                    });
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
            var loggedInId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
           
                var studentGroupId = (await _unitOfWork.Student.GetByIdAsync(id)).GroupId;
                var TeacherOfStudent = ((await _unitOfWork.Group.GetAllAsync()).Where(g => g.Id == studentGroupId)).Select(g => g.TeacherId).FirstOrDefault();
                if (TeacherOfStudent != int.Parse(loggedInId))
                    return BadRequest("You Do Not Have Permission");
            


            ////////////////////////
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
            try
            {
                // Validate the group existence
                var group = await _unitOfWork.Group.GetByIdAsync(student.GroupId);
                if (group == null)
                    return BadRequest($"No Group with id {student.GroupId}");

                // Get the logged-in user's ID
                var loggedInId = User.FindFirst("LoggedId")?.Value;
                if (string.IsNullOrEmpty(loggedInId))
                {
                    return Unauthorized("User not found");
                }

                // Check if the logged-in teacher has permission to update the student
                if (group.TeacherId != int.Parse(loggedInId))
                    return Forbid("You do not have permission to edit this student.");

                // Validate the student's data
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Fetch the student and related application user
                var studentEntity = await _unitOfWork.Student.GetByIdAsync(student.Id);
                if (studentEntity == null)
                    return NotFound($"No student was found with ID {student.Id}");

                var user = await _userManager.FindByEmailAsync(studentEntity.Code + StudentConst.EmailComplete);
                if (user == null)
                    return NotFound("No application user found for the student.");

                // Check if the group has changed
                bool hasGroupChanged = student.GroupId != studentEntity.GroupId;

                if (hasGroupChanged)
                {
                    await _studentService.HandleGroupChangeAsync(studentEntity.Id, studentEntity.GroupId);
                }

                // Update the student and user information
                studentEntity.GroupId = student.GroupId;
                user.Name = student.Name;

                var updateUserResult = await _userManager.UpdateAsync(user);
                if (!updateUserResult.Succeeded)
                    return BadRequest("Failed to update user information.");

                _unitOfWork.Student.Update(studentEntity);
                await _unitOfWork.CompleteAsync();

                if (hasGroupChanged)
                {
                    await _studentService.HandleNewGroupAdditionAsync(studentEntity.Id, student.GroupId);
                }

                // Fetch parent's information if available
                var parentData = await _studentService.GetParentDataAsync(studentEntity.ParentId);

                // Return the updated student details
                return Ok(new
                {
                    id = studentEntity.Id,
                    name = user.Name,
                    code = studentEntity.Code,
                    groupId = studentEntity.GroupId,
                    groupName = group.Name,
                    levelYearId = group.LevelYearId,
                    levelYearName = (await _unitOfWork.LevelYear.GetByIdAsync(group.LevelYearId)).Name,
                    levelId = (await _unitOfWork.LevelYear.GetByIdAsync(group.LevelYearId)).LevelId,
                    levelName = (await _unitOfWork.Level.GetByIdAsync((await _unitOfWork.LevelYear.GetByIdAsync(group.LevelYearId)).LevelId)).Name,
                    studentParentId = studentEntity.ParentId,
                    studentParentName = parentData.Name,
                    studentParentPhone = parentData.PhoneNumber
                });
            }
            catch (DbUpdateException dbEx)
            {
                // Log the detailed database update error with the inner exception if available
                var detailedError = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                return BadRequest($"Database update error: {detailedError}");
            }
            catch (Exception ex)
            {
                // Handle other types of general exceptions
                var generalError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest($"An error occurred while processing your request: {generalError}");
            }

        }
        [Authorize(Roles ="Student")]
        [HttpGet("GetMonthDataForStudent")]
        public async Task<IActionResult> GetMonthDataForStudentAsync(int studentId)
        {
            if (await _unitOfWork.Student.GetByIdAsync(studentId) == null)
                return BadRequest($"No student with id {studentId}");

            var loggedInId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (studentId != int.Parse(loggedInId))
                return BadRequest("You Do Not Have Permission");

            var studentMonths = await _unitOfWork.StudentMonth.FindAllAsync(sm => sm.StudentId == studentId);

            var studentMonthsDto = new List<StudentMonthParentDTO>();

            foreach (var month in studentMonths)
            {
                var studentMonthDto = new StudentMonthParentDTO();

                var monthData = await _unitOfWork.Month.GetByIdAsync(month.MonthId);
                var studentMonth = await _unitOfWork.StudentMonth.FindTWithExpression<StudentMonth>(sm => sm.StudentId == studentId && sm.MonthId == monthData.Id);
                var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == month.MonthId);
                var daysDto = new List<StudentMonthDayParentDTO>();
                foreach (var day in days)
                {
                    var studentAbsence = await _unitOfWork.StudentAbsence.FindTWithExpression<StudentAbsence>(sa => sa.DayId == day.Id && sa.StudentId == studentId);
                    if (studentAbsence != null)
                    {
                        var dayDto = new StudentMonthDayParentDTO
                        {
                            Date = day.Date,
                            Attended = studentAbsence.Attended
                        };
                        daysDto.Add(dayDto);
                    }
                }
                studentMonthDto.MonthName = monthData.Name;
                studentMonthDto.Year = monthData.Year;
                studentMonthDto.Pay = studentMonth.Pay;
                studentMonthDto.Days = daysDto;

                studentMonthsDto.Add(studentMonthDto);
            }
            return Ok(studentMonthsDto);
        }

    }
}