using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Collector.DAO.Entities;

namespace Collector.DAO.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null,
            params Expression<Func<T, object>>[] includeParams);

        Task<T> GetByIdAsync(long id,
            params Expression<Func<T, object>>[] includeParams);

        Task<T> GetFirstAsync(Expression<Func<T, bool>> expression = null,
            params Expression<Func<T, object>>[] includeParams);

        Task<T> GetByIdAsync(long id);
        Task RemoveByIdAsync(long id);
        Task<T> InsertAsync(T entity);
        Task RemoveAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression = null);
    }
}