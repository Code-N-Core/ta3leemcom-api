﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.Parent;
using PlatformAPI.Core.DTOs.Student;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PlatformAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Parent")]
    public class ParentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ParentController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager) 
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllParentData(int id)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (id != int.Parse(loggedInId))
                return BadRequest("Do Not Have Premission");

            var parent=await _unitOfWork.Parent.GetByIdAsync(id);
            if(parent == null)
                return NotFound($"No parent with id: {id}");

            
            
            var parentData = await _userManager.FindByIdAsync(parent.ApplicationUserId);

            

            var students= await _unitOfWork.Student.FindAllAsync(s=>s.ParentId == id);
            var studentsDto = new List<StudentParentDTO>();
            foreach(var student in students)
            {
                var teacher = await _unitOfWork.Teacher.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.TeacherId);
                var teacherData = await _userManager.FindByIdAsync(teacher.ApplicationUserId);

                var studentMonths=await _unitOfWork.StudentMonth.FindAllAsync(sm=>sm.StudentId == student.Id);

                var studentMonthsDto=new List<StudentMonthParentDTO>();

                foreach(var  month in studentMonths)
                {
                    var studentMonthDto=new StudentMonthParentDTO();

                    var monthData = await _unitOfWork.Month.GetByIdAsync(month.MonthId);
                    var studentMonth=await _unitOfWork.StudentMonth.FindTWithExpression<StudentMonth>(sm=>sm.StudentId==student.Id&&sm.MonthId==monthData.Id);
                    var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == month.MonthId);
                    var daysDto=new List<StudentMonthDayParentDTO>();
                    foreach (var day in days)
                    {
                        var studentAbsence = await _unitOfWork.StudentAbsence.FindTWithExpression<StudentAbsence>(sa => sa.DayId == day.Id && sa.StudentId == student.Id);
                        if (studentAbsence != null)
                        {
                            var dayDto = new StudentMonthDayParentDTO
                            {
                                Date=day.Date,
                                Attended=studentAbsence.Attended
                            };
                            daysDto.Add(dayDto);
                        }
                    }
                    studentMonthDto.MonthName = monthData.Name;
                    studentMonthDto.Year= monthData.Year;
                    studentMonthDto.Pay = studentMonth.Pay;
                    studentMonthDto.Days = daysDto;

                    studentMonthsDto.Add(studentMonthDto);
                }

                var studentQuizzesDto = new List<StudentQuizParentDTO>();
                var studentQuizzes = await _unitOfWork.GroupQuiz.FindAllAsync(gq => gq.GroupId == student.GroupId);
                foreach (var quiz in studentQuizzes)
                {
                    var studentQuizData = await _unitOfWork.StudentQuiz.FindTWithExpression<StudentQuiz>(sq => sq.QuizId == quiz.QuizId && sq.StudentId == student.Id);
                    var studentMark = 0;
                    if(studentQuizData!=null)studentMark=studentQuizData.StudentMark;
                    var quizData = await _unitOfWork.Quiz.GetByIdAsync(quiz.QuizId);
                    if (quizData != null&& quizData.StartDate+quizData.Duration<DateTime.Now)
                    {
                        var studentQuizDto = new StudentQuizParentDTO
                        {
                            Date = quizData.StartDate,
                            QuizMark=quizData.Mark,
                            StudentMark= studentMark,
                            Title =quizData.Title
                        };
                        studentQuizzesDto.Add(studentQuizDto);
                    }
                }
                
                // Full Student Data
                var studentDto = new StudentParentDTO
                {
                    Code = student.Code,
                    GroupName = _unitOfWork.Group.GetByIdAsync(student.GroupId).Result.Name,
                    LevelYearName = _unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId).Result.Name,
                    LevelName=_unitOfWork.Level.GetByIdAsync(_unitOfWork.LevelYear.GetByIdAsync(_unitOfWork.Group.GetByIdAsync(student.GroupId).Result.LevelYearId).Result.LevelId).Result.Name,
                    Id = student.Id,
                    Name=_userManager.FindByIdAsync(student.ApplicationUserId).Result.Name,
                    TeacherEmail=teacherData.Email,
                    TeacherName=teacherData.Name,
                    TeacherPhone=teacherData.PhoneNumber,
                    Months= studentMonthsDto,
                    Quizzes= studentQuizzesDto
                };
                studentsDto.Add(studentDto);


            }
            // Full Parent Data
            var parentDto = new ParentDataDTO
            {
                Email = parentData.Email,
                Id = id,
                Name = parentData.Name,
                Phone = parentData.PhoneNumber,
                Students = studentsDto
            };
            return Ok(parentDto);
        }
        [HttpPost("add-student-to-parent")]
        public async Task<IActionResult> AddStudentToParent(AddStudentToParentDTO model)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (model.ParentId != int.Parse(loggedInId))
                return BadRequest("Do Not Have Premission");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if(await _unitOfWork.Parent.GetByIdAsync(model.ParentId)==null)
                return BadRequest($"There is no parent with id: {model.ParentId}");
            if (await _unitOfWork.Student.FindTWithExpression<Student>(s => s.Code == model.StudentCode) == null)
                return NotFound("لا يوجد طالب بهذا الكود");
            if (_unitOfWork.Student.FindTWithExpression<Student>(s => s.Code == model.StudentCode).Result.ParentId != null)
                return BadRequest("هذا الطالب تمة اضافة ولي امر له مسبقا");
            var student = await _unitOfWork.Student.FindTWithExpression<Student>(s => s.Code == model.StudentCode);
            student.ParentId= model.ParentId;
            _unitOfWork.Student.Update(student);
            await _unitOfWork.CompleteAsync();
            return Ok("تم اضافة الطالب بنجاح");
        }
        [HttpPut("edit-parent-name")]
        public async Task<IActionResult> EditParentNameAsync(EditParentNameDTO model)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (model.Id != int.Parse(loggedInId))
                return BadRequest("Do Not Have Premission");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (await _unitOfWork.Parent.GetByIdAsync(model.Id) == null)
                return NotFound($"There is no parent with id: {model.Id}");
            var parentData = await _userManager.FindByIdAsync(_unitOfWork.Parent.GetByIdAsync(model.Id).Result.ApplicationUserId);
            parentData.Name=model.Name;
            await _userManager.UpdateAsync(parentData);
            return Ok(model);
        }
        [HttpPut("edit-parent-phone")]
        public async Task<IActionResult> EditParentPhoneAsync(EditParentPhoneDTO model)
        {
            var loggedInId = User.FindFirst("LoggedId")?.Value;

            if (string.IsNullOrEmpty(loggedInId))
            {
                return Unauthorized("User not found");
            }
            if (model.Id != int.Parse(loggedInId))
                return BadRequest("Do Not Have Premission");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _unitOfWork.Parent.GetByIdAsync(model.Id) == null)
                return NotFound($"There is no parent with id: {model.Id}");
            var parentData = await _userManager.FindByIdAsync(_unitOfWork.Parent.GetByIdAsync(model.Id).Result.ApplicationUserId);
            parentData.PhoneNumber = model.Phone;
            await _userManager.UpdateAsync(parentData);
            return Ok(model);
        }
    }
}