
using Microsoft.EntityFrameworkCore;
using PlatformAPI.Core.Models;

namespace PlatformAPI.EF.Repositories
{
    public class StudentMonthRepository:BaseRepository<StudentMonth>,IStudentMonthRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentMonthRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<StudentMonth> FindStudentMonthAsync(int studentId, int monthId)
            => await _context.StudentsMonths.FirstOrDefaultAsync(x=>x.StudentId==studentId&&x.MonthId==monthId);

    }
}