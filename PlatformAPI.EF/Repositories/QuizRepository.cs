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
            // Define Egypt's time zone
            TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            // Get the current time in Egypt
            DateTime currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);

            var quizzes = await (
                from q in _context.Quizzes
                join gq in _context.GroupsQuizzes on q.Id equals gq.QuizId
                join s in _context.Students on gq.GroupId equals s.GroupId
                join sq in _context.StudentsQuizzes on new { QuizId = q.Id, StudentId = s.Id } equals new { QuizId = sq.QuizId, StudentId = sq.StudentId } into studentQuizzes
                from sq in studentQuizzes.DefaultIfEmpty() // Left join
                where s.Id == studentId // Use the provided student ID

                let rank = _context.StudentsQuizzes
                   .Where(otherSq => otherSq.QuizId == q.Id)
                   .Count(otherSq => (otherSq.StudentMark + otherSq.StudentBounce) > (sq.StudentMark + sq.StudentBounce)) + 1 // Compute rank

                select new QuizStatusDto
                {
                    QuizId = q.Id,
                    Title = q.Title,
                    StudentQuizId = sq.Id,
                    StartDate = q.StartDate,
                    EndDate = q.StartDate.Add(q.Duration), // Use Duration directly
                    Duration = q.Duration,
                    IsAttend = sq.IsAttend,
                    SubmitAnswerDate = sq.SubmitAnswerDate,
                    QuizStatus = q.StartDate > currentDate
                        ? "Not Started"
                        : currentDate < q.EndDate && currentDate >= q.StartDate
                        ?  "Started"
                        : "Ended",
                    SolveStatus = sq.IsAttend
                        ? "Solved"
                        :  "Not Solved",
                    MandatoryQuestionCount = (from qq in _context.Questions
                                              where qq.QuizId == q.Id && qq.Type == QuestionType.Mandatory
                                              select qq).Count(),
                    OptionalQuestionCount = (from qq in _context.Questions
                                             where qq.QuizId == q.Id && qq.Type == QuestionType.Optional
                                             select qq).Count(),
                    TotalMark = q.Mark,
                    StudentMark = sq.Id > 0 ? sq.StudentMark : (int?)null, // StudentMark is not nullable
                    Bounce = q.Bounce,
                    StudentBounce = sq.StudentBounce,
                    OrderOfStudent = sq.Id > 0 ? rank : (int?)null
                }
            ).ToListAsync();

            return quizzes;
        }
        public async Task<List<Quiz>> GetEndedQuiz(DateTime datenow)
        {
            // Fetch non-notified quizzes first
            // Get them all in memory first
            try
            {
                var allQuizzes = await _context.Quizzes
    .Include(q => q.GroupsQuizzes)
        .ThenInclude(gq => gq.Group)
            .ThenInclude(g => g.Teacher)
    .ToListAsync();

                var endedQuizzes = allQuizzes
                    .Where(q => q.EndDate <= datenow && !q.IsNotfy)
                    .ToList();

                return endedQuizzes;




            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

    }
}
