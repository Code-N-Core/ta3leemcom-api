
namespace PlatformAPI.EF.Repositories
{
    public class GroupRepository:BaseRepository<Group>,IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

    }
}