using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface ITeacherRepository:IBaseRepository<Teacher>
    {
        Task<Teacher> GetByAppUserIdAsync(string id);
    }
}