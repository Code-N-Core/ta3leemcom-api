using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface IStudentMonthRepository:IBaseRepository<StudentMonth>
    {
        public Task<StudentMonth> FindStudentMonthAsync(int studentId,int monthId);
    }
}