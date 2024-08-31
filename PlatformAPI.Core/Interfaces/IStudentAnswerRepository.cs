using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface  IStudentAnswerRepository: IBaseRepository<StudentAnswer>
    {
        public  Task<List<StudentAnswer>> GetStudentAnswers(int StudentQuizId);

    }
}
