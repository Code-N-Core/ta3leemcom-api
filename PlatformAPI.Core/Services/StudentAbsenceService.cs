using AutoMapper;
using PlatformAPI.Core.DTOs.StudentAbsence;
using PlatformAPI.Core.Interfaces;
using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Services
{
    public class StudentAbsenceService:IStudentAbsenceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public StudentAbsenceService(IUnitOfWork unitOfWork,IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddAsync(int dayId)
        {
            var day = await _unitOfWork.Day.GetByIdAsync(dayId);
            var month = await _unitOfWork.Month.GetByIdAsync(day.MonthId);
            var group = await _unitOfWork.Group.GetByIdAsync(month.GroupId);

            var students = await _unitOfWork.Student.FindAllAsync(s => s.GroupId == group.Id);
            foreach (var student in students)
            {
                var model = new StudentAbsence {DayId=dayId,StudentId=student.Id,Attended=false };
                await _unitOfWork.StudentAbsence.AddAsync(model);
            }
            await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<StudentAbsenceDTO>> GetAllAsync(int dayId)
        {
            var absenceStudents = await _unitOfWork.StudentAbsence.FindAllAsync(sa => sa.DayId == dayId);
            var absenceStudentsDTO = new List<StudentAbsenceDTO>();
            foreach (var absence in absenceStudents)
            {
                var model = _mapper.Map<StudentAbsenceDTO>(absence);
                absenceStudentsDTO.Add(model);
            }
            return absenceStudentsDTO;
        }
        public async Task<StudentAbsenceDTO> UpdateAsync(StudentAbsenceDTO model)
        {
            var studentAbsence=_mapper.Map<StudentAbsence>(model);
            var updated= _unitOfWork.StudentAbsence.Update(studentAbsence);
            var updatedDTO=_mapper.Map<StudentAbsenceDTO>(updated);
            await _unitOfWork.CompleteAsync();
            return updatedDTO;
        }
    }
}