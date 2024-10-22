using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataAccessLayer.Model.Interfaces
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> selector = null);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, TEntity>> selector);
        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> selector,
            int pageSize = -1,
            int currentPage = -1,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);
        Task<bool> InsertAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter);
        Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
