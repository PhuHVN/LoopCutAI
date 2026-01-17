using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LoopCut.Domain.Abstractions
{
    public interface IGenericRepository <T> where T : class
    {
        IQueryable<T> Entity { get; }
        Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate);
        Task<IList<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T?> GetByIdAsync(object id);
        Task InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(object id);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<IList<T>> FilterByAsync(Expression<Func<T, bool>> predicate);
        Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);
        IQueryable<T> GetQueryable();
        Task DeleteRangeAsync(IEnumerable<T> entities);
    }
}
