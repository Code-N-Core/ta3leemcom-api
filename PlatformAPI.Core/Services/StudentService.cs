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
                Code = student.Code,
               
                Name = _userManager.FindByEmailAsync(student.Code + StudentConst.EmailComplete).Result.Name,
               
            };
            return s;
        }
    }
}
