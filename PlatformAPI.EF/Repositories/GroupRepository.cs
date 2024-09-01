
using System.Linq;

namespace PlatformAPI.EF.Repositories
{
    public class GroupRepository:BaseRepository<Group>,IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StudentQuiz>> Getquizsresults(List<int> ids)
        {
            if (ids == null || !ids.Any()) return null;

            var lsq = await _context.StudentsQuizzes
                .Where(sq=> ids.Contains(sq.Student.GroupId ))
                .Include(s=>s.Quiz)
                .ToListAsync();
            return lsq;
        }

    }
}