
namespace PlatformAPI.EF.Repositories
{
    public class GroupRepository:BaseRepository<Group>,IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Group>> GetAllGroupsOfTechId(int id=0)
        {
           var groubs= _context.Groups.Where(g=>g.TeacherId==id || id==0);
            return await groubs.ToListAsync();
        }
    }
}