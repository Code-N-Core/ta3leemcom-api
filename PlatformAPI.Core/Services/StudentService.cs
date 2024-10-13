using Microsoft.AspNetCore.Identity;
using PlatformAPI.Core.Const;
using PlatformAPI.Core.DTOs.Student;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformAPI.Core.Services
{
    public class StudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<StudentDTO> GetMapStudnt(Student student)
        {
            var s = new StudentDTO()
            {
                Id = student.Id,
                Code = student.Code,
                GroupId = student.GroupId,
                GroupName = student.Group.Name,
                Name = _userManager.FindByEmailAsync(student.Code + StudentConst.EmailComplete).Result.Name,
                LevelName = student.Group.LevelYear.Level.Name,
                LevelYearName = student.Group.LevelYear.Name,

                StudentParentId = student.Parent != null ? student.Parent.Id : null,
                StudentParentName = student.Parent != null ? student.Parent.ApplicationUser.Name : null,
                StudentParentPhone = student.Parent != null ? student.Parent.ApplicationUser.PhoneNumber : null,
            };
            return s;
        }
        public async Task<StudentMapDTO> GetMapStudntSimple(Student student)
        {
            var s = new StudentMapDTO()
            {
                Id = student.Id,
               
                Name = _userManager.FindByEmailAsync(student.Code + StudentConst.EmailComplete).Result.Name,
               
            };
            return s;
        }
        public async Task HandleGroupChangeAsync(int studentId, int oldGroupId)
        {
            var lastMonthInOldGroup = (await _unitOfWork.Month
                .FindAllAsync(m => m.GroupId == oldGroupId))
                .OrderByDescending(m => m.Id)
                .FirstOrDefault();

            if (lastMonthInOldGroup != null)
            {
                var studentMonth = await _unitOfWork.StudentMonth.FindTWithExpression<StudentMonth>(sm => sm.StudentId == studentId && sm.MonthId == lastMonthInOldGroup.Id);
                if (studentMonth != null)
                {
                    await _unitOfWork.StudentMonth.DeleteAsync(studentMonth);
                }

                var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == lastMonthInOldGroup.Id);
                foreach (var day in days)
                {
                    var studentAbsence = await _unitOfWork.StudentAbsence.FindTWithExpression<StudentAbsence>(sa => sa.StudentId == studentId && sa.DayId == day.Id);
                    if (studentAbsence != null)
                    {
                        await _unitOfWork.StudentAbsence.DeleteAsync(studentAbsence);
                    }
                }
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task HandleNewGroupAdditionAsync(int studentId, int newGroupId)
        {
            var lastMonthInNewGroup = (await _unitOfWork.Month
                .FindAllAsync(m => m.GroupId == newGroupId))
                .OrderByDescending(m => m.Id)
                .FirstOrDefault();

            if (lastMonthInNewGroup != null)
            {
                var newStudentMonth = new StudentMonth { MonthId = lastMonthInNewGroup.Id, Pay = false, StudentId = studentId };
                await _unitOfWork.StudentMonth.AddAsync(newStudentMonth);

                var days = await _unitOfWork.Day.FindAllAsync(d => d.MonthId == lastMonthInNewGroup.Id);
                foreach (var day in days)
                {
                    var studentAbsence = new StudentAbsence { DayId = day.Id, Attended = false, StudentId = studentId };
                    await _unitOfWork.StudentAbsence.AddAsync(studentAbsence);
                }
            }
            await _unitOfWork.CompleteAsync();
        }

        public async Task<(string Name, string PhoneNumber)> GetParentDataAsync(int? parentId)
        {
            if (parentId == null)
                return (null, null);

            var parent = await _unitOfWork.Parent.GetByIdAsync(parentId.Value);
            if (parent == null)
                return (null, null);

            var parentUser = await _userManager.FindByIdAsync(parent.ApplicationUserId);
            return parentUser != null ? (parentUser.Name, parentUser.PhoneNumber) : (null, null);
        }
    }
}
