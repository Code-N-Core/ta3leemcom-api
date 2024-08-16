using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PlatformAPI.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
        }


        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return entity;
        }
        public async Task<T> FindTWithIncludes<T>(int id, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            IQueryable<T> query =  _context.Set<T>();

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

          var Entity=await query.FirstOrDefaultAsync(e=> Microsoft.EntityFrameworkCore.EF.Property<object>(e,"Id").Equals(id));
            return Entity;
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria)
        {
            if (criteria == null)
                return await GetAllAsync();
            return  await _context.Set<T>().Where(criteria).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllWithIncludes<T>(Expression<Func<T, bool>>? criteria, params Expression<Func<T, object>>[]? includeProperties) where T : class
        {
            if (criteria == null)
                return null;

            IQueryable<T> query =  _context.Set<T>().Where(criteria);

            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));


            return await query.ToListAsync();
        }

    }
}