using PlatformAPI.Core.DTOs.StudentMonth;

namespace PlatformAPI.Core.Interfaces
{
    public interface IStudentMonthService
    {
        public Task<IEnumerable<StudentMonthDto>> GetAllAsync(int monthId);
        public Task AddAsync(int monthId);
        public Task<StudentMonthDto> UpdateAsync(StudentMonthDto model);

    }
}