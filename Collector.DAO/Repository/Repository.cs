using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Collector.DAO.Data;
using Collector.DAO.Entities;
using Microsoft.EntityFrameworkCore;

namespace Collector.DAO.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _entities;

        public Repository(DataContext dataContext)
        {
            _context = dataContext;
            _entities = _context.Set<T>();
        }

        public async Task<T> InsertAsync(T entity)
        {
            if (entity == null)
                throw new NullReferenceException();

            _entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();


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

        public async Task<T> GetByIdAsync(long id) =>
            await _entities.FirstOrDefaultAsync(e => e.Id == id);

        public async Task RemoveByIdAsync(long id)
        {
            var entity = await _entities.FirstOrDefaultAsync(e => e.Id == id);
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression = null) =>
            expression != null ? await _entities.Where(expression).AnyAsync() : await _entities.AnyAsync();


        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null) =>
            await Task.Run(() => expression != null ? _entities.Where(expression) : _entities);

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> expression) =>
            await _entities.Where(expression).FirstOrDefaultAsync();
    }
}