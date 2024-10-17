using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using PlatformAPI.Core.DTOs.Choose;
using PlatformAPI.Core.DTOs.Questions;
using PlatformAPI.Core.Models;

namespace PlatformAPI.EF.Repositories
{
    public class QuestionRepository:BaseRepository<Question>,IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<List<QuestionsStatsDTO>> GetStatsOfQuiz(int quizId)
        {
            var results = await (from sa in _context.StudentAnswers
                                 join sq in _context.StudentsQuizzes on sa.StudentQuizId equals sq.Id
                                 where sq.QuizId == quizId
                                 group sa by sa.QuestionId into g
                                 select new QuestionsStatsDTO
                                 {
                                     QuestionId = g.Key,
                                     QuestionContent = _context.Questions
                                         .Where(q => q.Id == g.Key)
                                         .Select(q => q.Content)
                                         .FirstOrDefault(),
                                     CorrectAnswerCount = g.Count(sa => sa.IsCorrect),
                                     IncorrectAnswerCount = g.Count(sa => !sa.IsCorrect),
                                     Choices = new List<ChooseStatsDTO> { }

                                 }).ToListAsync();

            foreach (var res in results)
            {
                var choiceResults = await (from c in _context.Chooses
                                               // Left join with StudentAnswers
                                           join sa in _context.StudentAnswers on c.Id equals sa.ChosenOptionId into saGroup
                                           from sa in saGroup.DefaultIfEmpty() // This keeps choices without student answers
                                           where c.QuestionId == res.QuestionId
                                           group sa by new { c.Id, c.Content, c.IsCorrect } into gg
                                           select new ChooseStatsDTO
                                           {
                                               Id = gg.Key.Id,
                                               Content = gg.Key.Content,
                                               IsCorrect = gg.Key.IsCorrect,
                                               // Count only non-null student answers, otherwise it's 0 (for unchosen options)
                                               ChoiceSelectionCount = gg.Count(sa => sa != null)
                                           })
                                           .OrderBy(c => c.Id)
                                           .ToListAsync();

                res.Choices = choiceResults;
            }

            if (results is null)
                return null;

            return results;

        }

        public async Task<int> GetNumbersOfQuestionInQuizId(int quizid)
        {
            if (quizid == 0)
                return 0;
            var numq =await _context.Questions.Where(q => q.QuizId == quizid).CountAsync();
            return numq;
        }
    }
}
