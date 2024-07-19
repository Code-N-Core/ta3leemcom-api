namespace PlatformAPI.EF.Repositories
{
    public class ParentRepository:BaseRepository<Parent>,IParentRepository
    {
        private readonly ApplicationDbContext _context;

        public ParentRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<Parent> GetByAppUserIdAsync(string id)
            => await _context.Parents.SingleOrDefaultAsync(t => t.ApplicationUserId == id);
    }
}
