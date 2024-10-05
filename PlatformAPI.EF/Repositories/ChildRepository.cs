
namespace PlatformAPI.EF.Repositories
{
    public class ChildRepository : BaseRepository<Child>, IChildRepository
    {
        private readonly ApplicationDbContext _context;
        public ChildRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
