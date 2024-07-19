namespace PlatformAPI.EF.Repositories
{
    public class StudentQuizRepository:BaseRepository<StudentQuiz>,IStudentQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentQuizRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}