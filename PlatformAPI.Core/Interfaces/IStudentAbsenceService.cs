using PlatformAPI.Core.DTOs.StudentAbsence;

namespace PlatformAPI.Core.Interfaces
{
    public interface IStudentAbsenceService
    {
        public Task AddAsync(int dayId);
        public Task<IEnumerable<StudentAbsenceDTO>> GetAllAsync(int dayId);
        public Task<StudentAbsenceDTO> UpdateAsync(StudentAbsenceDTO model);
    }
}
