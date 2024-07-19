namespace PlatformAPI.EF.Repositories
{
    public class QuestionRepository:BaseRepository<Question>,IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
