using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface IParentRepository:IBaseRepository<Parent>
    {
        Task<Parent> GetByAppUserIdAsync(string id);
    }
}