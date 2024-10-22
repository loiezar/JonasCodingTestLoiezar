using DataAccessLayer.Model.Models;
using System;

namespace DataAccessLayer.Model.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetEntityRepository<TEntity>()
            where TEntity : DataEntity, new();
    }
}
