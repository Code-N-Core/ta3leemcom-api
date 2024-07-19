namespace PlatformAPI.EF.Repositories
{
    public class StudentAbsenceRepository:BaseRepository<StudentAbsence>,IStudentAbsenceRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentAbsenceRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}