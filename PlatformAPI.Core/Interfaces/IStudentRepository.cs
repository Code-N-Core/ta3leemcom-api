using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface IStudentRepository:IBaseRepository<Student>
    {
        Task<Student> FindByCodeAsync(string Code);
    }
}