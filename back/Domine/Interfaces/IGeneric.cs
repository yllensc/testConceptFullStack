using System.Linq.Expressions;
using Domine.Entities;

namespace Domine.Interfaces
{
    public interface IGeneric<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        Task<(int totalRecords, IEnumerable<T> records)> GetAllAsync(int pageIndex, int pageSize, string search);
        void Add(T entity);
        void AddRange (IEnumerable<T> entities);
        void Remove (T entity);
        void RemoveRange (IEnumerable<T> entities);
        void Update (T entity);
    }
}