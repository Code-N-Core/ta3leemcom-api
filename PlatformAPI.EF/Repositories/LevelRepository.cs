namespace PlatformAPI.EF.Repositories
{
    public class LevelRepository:BaseRepository<Level>,ILevelRepository
    {
        private readonly ApplicationDbContext _context;

        public LevelRepository(ApplicationDbContext context):base (context) 
        {
            _context = context;
        }
    }
}