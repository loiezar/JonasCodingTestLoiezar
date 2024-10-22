using System;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using DataAccessLayer.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly IServiceProvider _serviceProvider;
        public UnitOfWork(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public IRepository<TEntity> GetEntityRepository<TEntity>()
            where TEntity : DataEntity, new()
        {
            var dbWrapper = (IDbWrapper<TEntity>)_serviceProvider.GetService(typeof(IDbWrapper<TEntity>));

            var repository = new Repository<TEntity>(dbWrapper);
            if (repository == null)
                throw new ArgumentException("Repository not found.");
            return repository;
        }

        protected bool _disposed = false;

        /// <summary>
        /// Check if the UnitOfWork has been disposed.
        /// </summary>
        /// <returns>True when <see cref="Dispose()"/> as been called</returns>
        ~UnitOfWork()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed) _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            
            GC.SuppressFinalize(this);
        }
    }
}
