using PlatformAPI.Core.Models;
using System.Linq.Expressions;

namespace PlatformAPI.Core.Interfaces
{
    public interface IStudentRepository:IBaseRepository<Student>
    {
        Task<Student> FindByCodeAsync(string Code);
    }
}