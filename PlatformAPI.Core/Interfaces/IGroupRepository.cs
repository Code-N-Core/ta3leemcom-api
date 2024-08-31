using PlatformAPI.Core.Models;
namespace PlatformAPI.Core.Interfaces
{
    public interface IGroupRepository:IBaseRepository<Group>
    {
        public  Task<IEnumerable<StudentQuiz>> Getquizsresults(List<int> ids);

    }
}