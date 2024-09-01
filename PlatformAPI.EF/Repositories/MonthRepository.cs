using PlatformAPI.Core;
namespace PlatformAPI.EF.Repositories
{
    public class MonthRepository:BaseRepository<Month>,IMonthRepository
    {
        private readonly ApplicationDbContext _context;

        public MonthRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckMonthExistAsync(Month month)
        {
            var result= await _context.Months.FirstOrDefaultAsync(m=> m.Name == month.Name && m.Year == month.Year );
            return result!=null;
        }
    }
}