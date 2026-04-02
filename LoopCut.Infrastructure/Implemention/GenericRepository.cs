using LoopCut.Domain.Abstractions;
using LoopCut.Infrastructure.DatabaseSettings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Infrastructure.Implemention
{
    public class GenericRepository <T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext appDb)
        {
            _context = appDb;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Entity => _dbSet;

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<IList<T>> FilterByAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            if (include != null)
                query = include(query);
            return await query.ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync(
         Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public async Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
                query = include(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetByIdAsync(object id, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return null;
            
            // If include is provided, we need to reload with includes
            if (include != null)
            {
                var id_value = _context.Entry(entity).Property("Id").CurrentValue;
                IQueryable<T> query = _dbSet;
                query = include(query);
                entity = await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id_value));
            }
            return entity;
        }

        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);           
        }

        public async Task<T> UpdateAsync(T entity, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            var id = _context.Entry(entity).Property("Id").CurrentValue;
            
            T? exist;
            if (include != null)
            {
                IQueryable<T> query = _dbSet;
                query = include(query);
                exist = await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
            }
            else
            {
                exist = await _dbSet.FindAsync(id);
            }

            if (exist == null)
            {
                throw new KeyNotFoundException($"Entity with Id {id} not found.");
            }
            _context.Entry(exist).CurrentValues.SetValues(entity);
            return exist;
        }

        public async Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return new BasePaginatedList<T>(items, count, index, pageSize);
        }
        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await Task.CompletedTask;
        }

    }
}
