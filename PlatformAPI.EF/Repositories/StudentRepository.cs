
namespace PlatformAPI.EF.Repositories
{
    public class StudentRepository:BaseRepository<Student>,IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<Student> FindByCodeAsync(string Code)
            => await _context.Students.FirstOrDefaultAsync(s => s.Code == Code);
    }
}