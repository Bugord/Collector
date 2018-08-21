using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Collector.DAO.Entities;

namespace Collector.DAO.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> redicate = null);
        Task<T> GetByIdAsync(int id);
        Task InsertAsync(T entity);
        Task RemoveAsync(T entity);
        Task UpdateAsync(T entity);
    }
}
