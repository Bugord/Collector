using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Collector.DAO.Data;
using Collector.DAO.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collector.DAO.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private DataContext _context;
        private DbSet<T> _entities;

        public Repository(DataContext dataContext)
        {
            _context = dataContext;
            _entities = _context.Set<T>();

        }

        public async Task InsertAsync(T entity)
        {
            if (entity == null)
                throw new NullReferenceException();

            _entities.Add(entity);
            await _context.SaveChangesAsync(true);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new NullReferenceException();

            _entities.Update(entity);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            if (entity == null)
                throw new NullReferenceException();

            _entities.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await Task.Run(() => _entities.FirstOrDefaultAsync(e => e.ID == id));
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null)
        {

            if (expression != null)
            {
                return await Task.Run(() =>
                {
                    var r = _entities.AsQueryable().Where(expression);
                    return r;

                });
            }

            return await Task.Run(() => _entities.AsQueryable());
        }

        //public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null)
        //{
        //    if (predicate != null)
        //        return _entities.AsQueryable().Where(predicate);

        //    return _entities.AsQueryable();
        //}
    }


}
