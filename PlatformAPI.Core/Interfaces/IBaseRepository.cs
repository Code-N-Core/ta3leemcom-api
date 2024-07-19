namespace PlatformAPI.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task <IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> GetByIdAsync(int id);
        T Update(T entity);
        Task DeleteAsync(int id);
    }
}