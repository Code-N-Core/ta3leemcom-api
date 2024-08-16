
namespace PlatformAPI.EF.Repositories
{
    public class GroupRepository:BaseRepository<Group>,IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StudentQuiz>> Getquizsresults(int id)
        {
            if (id == 0) return null;
            var lsq = await _context.StudentsQuizzes
                .Where(sq=>sq.Quiz.GroupsQuizzes.Any(gq=>gq.GroupId==id))
                .ToListAsync();
                
            return lsq;

        }

    }
}