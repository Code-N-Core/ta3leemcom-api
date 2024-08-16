using System.Linq.Expressions;

namespace PlatformAPI.EF.Repositories
{
    public class ChooseRepository:BaseRepository<Choose>,IChooseRepository
    {
        private readonly ApplicationDbContext _context;

        public ChooseRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<Choose> ValidAnswer(int questionid)
        {
            if (questionid == 0) return null;
            var c=await _context.Chooses.Where(c=>c.QuestionId==questionid && c.IsCorrect==true).SingleOrDefaultAsync();
            return c;
        }
    }
}