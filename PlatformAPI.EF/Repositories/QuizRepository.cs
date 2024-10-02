using PlatformAPI.Core.DTOs.Quiz;
using PlatformAPI.Core.Const;
using System.Data;

namespace PlatformAPI.EF.Repositories
{
    public class QuizRepository : BaseRepository<Quiz>, IQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public QuizRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Retrieve quizzes by Group IDs
        public async Task<IEnumerable<Quiz>> GetQuizzesByGroupsIds(List<int> groupsIds)
        {
            var quizzes = await _context.Quizzes
                                        .Where(q => q.GroupsQuizzes.Any(gq => groupsIds.Contains(gq.GroupId)))
                                        .ToListAsync();

            if (quizzes == null || !quizzes.Any())
            {
                return null;
            }

            return quizzes;
        }

        public async Task<List<QuizStatusDto>> GetQuizzesStatusByStudentId(int studentId)
        {
            DateTime currentDate = DateTime.Now;

            var quizzes = await (
                from q in _context.Quizzes
                join gq in _context.GroupsQuizzes on q.Id equals gq.QuizId
                join s in _context.Students on gq.GroupId equals s.GroupId
                join sq in _context.StudentsQuizzes on new { QuizId = q.Id, StudentId = s.Id } equals new { QuizId = sq.QuizId, StudentId = sq.StudentId } into studentQuizzes
                from sq in studentQuizzes.DefaultIfEmpty() // Left join
                where s.Id == studentId // Use the provided student ID
                select new QuizStatusDto
                {
                    QuizId = q.Id,
                    StudentQuizId=sq.Id,
                    StartDate = q.StartDate,
                    EndDate = q.StartDate.Add(q.Duration), // Use Duration directly
                    Duration = q.Duration,
                    IsAttend = sq.IsAttend,
                    SubmitAnswerDate = sq.SubmitAnswerDate,
                    QuizStatus = q.StartDate > currentDate
                        ? "Not Started Yet"
                        :   currentDate < q.EndDate&&currentDate >= q.StartDate
                        ? (sq != null && sq.IsAttend ? "Solved" : "Available")
                        : "Ended",
                        
                    SolveStatus = sq.IsAttend // Always true or false, no null check needed
                        ? (sq.SubmitAnswerDate <= q.EndDate
                            ? "Solved In Time"
                            : "Solved Late")
                        : "Not Solved",
                    MandatoryQuestionCount = (from qq in _context.Questions
                                              where qq.QuizId == q.Id && qq.Type == QuestionType.Mandatory
                                              select qq).Count(),
                    OptionalQuestionCount = (from qq in _context.Questions
                                             where qq.QuizId == q.Id && qq.Type == QuestionType.Optional
                                             select qq).Count(),
                    TotalMark = q.Mark,
                    StudentMark = sq.IsAttend ? sq.StudentMark : (int?)null, // StudentMark is not nullable
                    Bounce=q.Bounce,
                    StudentBounce=sq.StudentBounce,
                }
            ).ToListAsync();

            return quizzes;
        }
        public async Task<List<Quiz>> GetEndedQuiz(DateTime datenow)
        {
            // Fetch non-notified quizzes first
            // Get them all in memory first
            var endedQuizzes =await _context.Quizzes
                    .Where(q => q.EndDate <= datenow && !q.IsNotfy)
                    .Include(q => q.GroupsQuizzes)
                    .ThenInclude(gq => gq.Group)
                    .ThenInclude(g => g.Teacher)
                    .ToListAsync();

            return endedQuizzes;
        }

    }
}
