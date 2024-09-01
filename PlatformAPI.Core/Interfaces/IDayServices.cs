using PlatformAPI.Core.DTOs.Day;

namespace PlatformAPI.Core.Interfaces
{
    public interface IDayServices
    {
        Task<IEnumerable<ViewDayDTO>> GetAllAsync(int monthId);
    }
}