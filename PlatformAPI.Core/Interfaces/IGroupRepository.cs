using PlatformAPI.Core.Models;
namespace PlatformAPI.Core.Interfaces
{
    public interface IGroupRepository:IBaseRepository<Group>
    {
         Task<IEnumerable<Group>> GetAllGroupsOfTechId(int id=0);
    }
}