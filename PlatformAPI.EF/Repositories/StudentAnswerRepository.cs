namespace PlatformAPI.EF.Repositories
{
    public class StudentAnswerRepository : BaseRepository<StudentAnswer>, IStudentAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentAnswerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<StudentAnswer>> GetStudentAnswers(int StudentQuizId)
        {
/*            var answers = await _context.StudentAnswers.Where(sa => sa.StudentId == studentid && sa.QuizId == quizid).ToListAsync();
*/            var answers = await _context.StudentAnswers.Where(sa =>sa.StudentQuizId==StudentQuizId
              ).Include(sa=>sa.Question).Include(sa=>sa.Question.Chooses).Include(sa=>sa.Question.Quiz).ToListAsync();
            if (answers == null || !answers.Any())
            {
                return null;
            }

            return answers;
        }
        }
}
