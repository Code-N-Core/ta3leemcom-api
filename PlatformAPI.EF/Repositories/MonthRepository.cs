namespace PlatformAPI.EF.Repositories
{
    public class MonthRepository:BaseRepository<Month>,IMonthRepository
    {
        private readonly ApplicationDbContext _context;

        public MonthRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}