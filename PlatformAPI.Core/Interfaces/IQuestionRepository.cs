using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.Models;

namespace PlatformAPI.Core.Interfaces
{
    public interface IQuestionRepository:IBaseRepository<Question>
    {
        public Task<List<QuestionsStatsDTO>> GetStatsOfQuiz(int quizId);
        public Task<int> GetNumbersOfQuestionInQuizId(int quizid);


    }
}