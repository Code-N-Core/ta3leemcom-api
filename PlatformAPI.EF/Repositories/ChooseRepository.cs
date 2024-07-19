namespace PlatformAPI.EF.Repositories
{
    public class ChooseRepository:BaseRepository<Choose>,IChooseRepository
    {
        private readonly ApplicationDbContext _context;

        public ChooseRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}