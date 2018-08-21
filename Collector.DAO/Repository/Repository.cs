using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Collector.DAO.Data;
using Collector.DAO.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collector.DAO.Repository
{
    class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _entities;

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
            await _context.SaveChangesAsync();
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
            return await Task.Run(() =>_entities.FirstOrDefault(e => e.ID == id));
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null)
        {
            if (expression != null)
                return await Task.Run(() => _entities.AsQueryable().Where(expression));
            return await Task.Run(() => _entities.AsQueryable());
        }
    }
}
