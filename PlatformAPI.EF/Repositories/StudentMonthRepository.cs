namespace PlatformAPI.EF.Repositories
{
    public class StudentMonthRepository:BaseRepository<StudentMonth>,IStudentMonthRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentMonthRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}