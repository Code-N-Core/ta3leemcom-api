namespace PlatformAPI.EF.Repositories
{
    public class GroupQuizRepository:BaseRepository<GroupQuiz>,IGroupQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupQuizRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
        }
    }
}