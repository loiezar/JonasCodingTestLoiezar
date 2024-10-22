using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataAccessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;

namespace DataAccessLayer.Repositories
{
    public sealed class Repository<TEntity> : IRepository<TEntity>
        where TEntity : DataEntity,
        new()
    {
        private readonly IDbWrapper<TEntity> _dbWrapper;

        public Repository(IDbWrapper<TEntity> dbWrapper)
        {
            _dbWrapper = dbWrapper;
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TEntity>> selector = null)
        {
            return (await GetAllAsync(predicate, selector)).FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return await GetAllAsync(predicate, null);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, TEntity>> selector)
        {
            return await GetAllAsync(null, selector);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TEntity>> selector ,
            int pageSize = -1,
            int currentPage = -1,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = predicate == null
                ? (await _dbWrapper.FindAllAsync()).AsQueryable()
                : (await _dbWrapper.FindAsync(predicate)).AsQueryable();

            if (selector  != null)
            {
                query = query.Select(selector);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (currentPage > -1)
            {
                query = query.Skip(currentPage);
            }

            if (pageSize > -1)
            {
                query = query.Take(pageSize);
            }

            return query;
        }

        public async Task<bool> InsertAsync(TEntity entity)
        {
            return await _dbWrapper.InsertAsync(entity);
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            return await _dbWrapper.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbWrapper.DeleteAsync(filter);
        }

        public async Task<long> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return (await GetAllAsync(predicate)).Count();
        }

    }
}
