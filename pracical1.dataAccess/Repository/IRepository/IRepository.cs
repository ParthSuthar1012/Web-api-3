using Microsoft.CodeAnalysis.Operations;
using System.Linq.Expressions;
namespace Repository.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeProperties=null);

        T GetFirstOrDefault(Expression<Func<T,bool>> filter,string? includeProperties=null);
      
        void Add(T entitiy);

        void Remove(T entitiy);

        void RemoveRange(IEnumerable<T> entities);
    }
}
