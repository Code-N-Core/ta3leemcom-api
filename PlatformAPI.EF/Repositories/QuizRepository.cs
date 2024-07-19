namespace PlatformAPI.EF.Repositories
{
    public class QuizRepository:BaseRepository<Quiz>,IQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public QuizRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}