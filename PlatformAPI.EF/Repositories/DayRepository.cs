namespace PlatformAPI.EF.Repositories
{
    public class DayRepository:BaseRepository<Day>,IDayRepository
    {
        private readonly ApplicationDbContext _context;

        public DayRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}