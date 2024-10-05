
using PlatformAPI.Core.Models;
using System.Linq.Expressions;

namespace PlatformAPI.EF.Repositories
{
    public class StudentRepository:BaseRepository<Student>,IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<Student> GetByAppUserIdAsync(string id)
           => await _context.Students.SingleOrDefaultAsync(s =>s.ApplicationUserId  == id);
        public async Task<Student> FindByCodeAsync(string Code)
            => await _context.Students.FirstOrDefaultAsync(s => s.Code == Code);

        public async Task<IEnumerable<Student>> GetStudentNotEnter(int quizid)
        {
            var studentsNotInQuiz =await _context.Students
                 .Where(s => _context.GroupsQuizzes
                 .Any(gq => gq.QuizId == quizid && gq.GroupId == s.GroupId) // Check if the student's group is linked to the quiz
                 && !_context.StudentsQuizzes
                 .Any(sq => sq.StudentId == s.Id && sq.QuizId == quizid && sq.IsAttend)) // Ensure the student hasn't taken the quiz
                 .ToListAsync();

            if (studentsNotInQuiz == null)
                return null;

            return studentsNotInQuiz;

        }
        public async Task<int> GetAllStudentsSolveQuiz(int quizid)
        {
            var sts = await _context.StudentsQuizzes.Where(sq => sq.QuizId == quizid).CountAsync();
            return sts;
        }
        public async Task<IEnumerable<Student>> GetTopStudents(List<int> ids)
        {
            var topStudents =await _context.Students
                .Where(s => ids.Contains(s.GroupId))
                .Include(x => x.Group)
                .Include(x => x.Group.LevelYear)
                .Include(x => x.Group.LevelYear.Level)
                .Include(x => x.Parent)
                .Include(x => x.Parent.ApplicationUser)
                .Join(
                _context.StudentsQuizzes,
                s => s.Id,
                sq => sq.StudentId,
                (s, sq) => new {Student=s,sq.StudentMark}
            )
                .GroupBy(x=>x.Student)
            .Select(g=> new
            {
                Student=g.Key,
                averagemark=g.Average(x=>x.StudentMark)

            })
            .OrderByDescending(x=>x.averagemark)
            .Select(x=>x.Student)
            
            .ToListAsync();

            if (topStudents == null || !topStudents.Any())
                return null;

            return topStudents;


        }

    }
}