using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformAPI.Core.DTOs.StudentMonth;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Services
{
    public class StudentMonthService:IStudentMonthService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
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
        public StudentMonthService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudentMonthDto>> GetAllAsync(int monthId)
        {
            var model = await _unitOfWork.StudentMonth.FindAllAsync(sm => sm.MonthId == monthId);
            List<StudentMonthDto> studentsMonths = new List<StudentMonthDto>();
            foreach (var studentMonth in model)
            {
                studentsMonths.Add(_mapper.Map<StudentMonthDto>(studentMonth));
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