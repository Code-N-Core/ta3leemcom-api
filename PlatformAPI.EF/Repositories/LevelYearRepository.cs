namespace PlatformAPI.EF.Repositories
{
    public class LevelYearRepository:BaseRepository<LevelYear>,ILevelYearRepository
    {
        private readonly ApplicationDbContext _context;

        public LevelYearRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}