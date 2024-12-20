﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.Student;
using PlatformAPI.Core.DTOs.StudentAbsence;
using PlatformAPI.Core.DTOs.StudentMonth;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Services
{
    public class StudentMonthService:IStudentMonthService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public async Task AddAsync(int monthId)
        {
            var groupId = _unitOfWork.Month.GetByIdAsync(monthId).Result.GroupId;
            var students = await _unitOfWork.Student.FindAllAsync(s => s.GroupId == groupId);
            foreach (var student in students)
            {
                var model = new StudentMonth { StudentId = student.Id, MonthId = monthId, Pay = false };
                await _unitOfWork.StudentMonth.AddAsync(model);
            }
            await _unitOfWork.CompleteAsync();
        }
        public StudentMonthService(IUnitOfWork unitOfWork,IMapper mapper,UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;

        }

        public async Task<IEnumerable<StudentMonthDTO>> GetAllAsync(int monthId)
        {
            var model = await _unitOfWork.StudentMonth.FindAllAsync(sm => sm.MonthId == monthId);
            List<StudentMonthDTO> studentsMonths = new List<StudentMonthDTO>();
            foreach (var studentMonth in model)
            {
                var studentMonthDto = _mapper.Map<StudentMonthDTO>(studentMonth);
                studentMonthDto.Name= _userManager.FindByIdAsync(_unitOfWork.Student.GetByIdAsync(studentMonth.StudentId).Result.ApplicationUserId).Result.Name;
                var studentAbsencesDto= new List<StudentAbsenceForMonthDTO>();
                var monthDays=await _unitOfWork.Day.FindAllAsync(d=>d.MonthId==monthId);
                foreach(var day in monthDays)
                {
                   var studentAbsenceDto=new StudentAbsenceForMonthDTO();
                    studentAbsenceDto.DayId= day.Id;
                    studentAbsenceDto.IsAttended= _unitOfWork.StudentAbsence.FindTWithExpression<StudentAbsence>(d=>d.DayId==day.Id&&d.StudentId==studentMonth.StudentId).Result.Attended;
                    studentAbsencesDto.Add(studentAbsenceDto);
                }
                studentMonthDto.StudentAbsences = studentAbsencesDto;
                studentsMonths.Add(studentMonthDto);
            }
            
            return studentsMonths;
        }
        public async Task<StudentMonthDto> UpdateAsync(StudentMonthDto model)
        {
            var monthStudent = _mapper.Map<StudentMonth>(model);
            var updated = _unitOfWork.StudentMonth.Update(monthStudent);
            var updatedDTO=_mapper.Map<StudentMonthDto>(updated);
            await _unitOfWork.CompleteAsync();
            return updatedDTO;
        }
    }
}