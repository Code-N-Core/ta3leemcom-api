using PlatformAPI.Core.Models;
using System.Linq.Expressions;

namespace PlatformAPI.Core.Interfaces
{
    public interface IChooseRepository:IBaseRepository<Choose>
    {
        public  Task<Choose> ValidAnswer(int questionid);

    }
}