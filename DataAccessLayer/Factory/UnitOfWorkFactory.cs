using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using DataAccessLayer.Uow;
using System;

namespace DataAccessLayer.Factory
{
    public class UnitOfWorkFactory: IUnitOfWorkFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork Create()
        {
            return new UnitOfWork(_serviceProvider);
        }
    }
}
