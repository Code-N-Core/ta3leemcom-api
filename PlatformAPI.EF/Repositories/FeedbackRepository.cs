namespace PlatformAPI.EF.Repositories
{
    public class FeedbackRepository:BaseRepository<Feedback>,IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;
        public FeedbackRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
