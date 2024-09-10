using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface IQuizRepository:IBaseRepository<Quiz>
    {
        public Task<IEnumerable<Quiz>> GetQuizzesByGroupsIds(List<int> groupsIds);
        public  Task<List<QuizStatusDto>> GetQuizzesStatusByStudentId(int studentId);


    }
}