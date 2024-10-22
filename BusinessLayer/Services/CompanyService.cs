using DataAccessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using AutoMapper;
using BusinessLayer.Model.Models;
using BusinessLayer.Model.Interfaces;
using Ninject.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace BusinessLayer.Services
{
    public class CompanyService : BaseService<Company, CompanyInfo>, ICompanyService
    {
        public CompanyService(IRepository<Company> repository, 
            IMapper mapper,
            ILogger logger)
            : base(repository, mapper, logger)
        {
            
        }

        public override async Task<bool> OnBeforeSave()
        {
            await base.OnBeforeSave();

            if (await FindByFilter(x => x.CompanyName.ToLower() == DataInfo.CompanyName.ToLower(), true))
                throw new ApplicationException("Company Name already exists.");

            return true;
        }

        public override async Task<bool> OnBeforeInsert()
        {
            await base.OnBeforeInsert();

            DataInfo.CompanyCode = DataInfo.Code;

            return true;
        }

        public override async Task<bool> OnBeforeUpdate()
        {
            await base.OnBeforeUpdate();

            DataInfo.CompanyCode = DataInfo.Code;

            return true;
        }
    }
}
