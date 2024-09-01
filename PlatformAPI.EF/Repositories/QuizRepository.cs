using Microsoft.AspNetCore.Mvc;

namespace PlatformAPI.EF.Repositories
{
    public class QuizRepository:BaseRepository<Quiz>,IQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public QuizRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Quiz>> GetQuizzesByGroupsIds(List<int> groupsIds)
        {
            var quizzes = await _context.Quizzes
                                        .Where(q => q.GroupsQuizzes.Any(gq =>groupsIds.Contains(gq.GroupId)))
                                        .ToListAsync();
          /*  var quizzes = await _context.GroupsQuizzes
                                        .Where(g => g.GroupId == groupId)
                                        .Select(gq=>gq.Quiz)
                                       .ToListAsync();*/

            if (quizzes == null || !quizzes.Any())
            {
                return null;
            }

            return quizzes;
        }
    }
}