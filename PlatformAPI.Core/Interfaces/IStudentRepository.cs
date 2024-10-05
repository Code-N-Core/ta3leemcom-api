using PlatformAPI.Core.Models;
using System.Linq.Expressions;

namespace PlatformAPI.Core.Interfaces
{
    public interface IStudentRepository:IBaseRepository<Student>
    {
        Task<Student> FindByCodeAsync(string Code);
        public Task<IEnumerable<Student>> GetStudentNotEnter(int quizid);
        public Task<IEnumerable<Student>> GetTopStudents(List<int> ids);
        public Task<int> GetAllStudentsSolveQuiz(int quizid);



    }
}