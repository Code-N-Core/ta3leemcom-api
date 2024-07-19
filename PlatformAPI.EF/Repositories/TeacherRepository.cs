
namespace PlatformAPI.EF.Repositories
{
    public class TeacherRepository:BaseRepository<Teacher>,ITeacherRepository
    {
        private readonly ApplicationDbContext _context;
        public TeacherRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<Teacher> GetByAppUserIdAsync(string id)
            => await _context.Teachers.SingleOrDefaultAsync(t => t.ApplicationUserId == id);

    }
}