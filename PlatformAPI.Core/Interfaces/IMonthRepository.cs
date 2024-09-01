using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface IMonthRepository:IBaseRepository<Month>
    {
        public Task<bool> CheckMonthExistAsync(Month month);
    }
}