using DataAccessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using AutoMapper;
using BusinessLayer.Model.Models;
using BusinessLayer.Model.Interfaces;
using Ninject.Extensions.Logging;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Services
{
    public class EmployeeService : BaseService<Employee, EmployeeInfo>, IEmployeeService
    {
        private readonly ICompanyService _companyService;

        public EmployeeService(IRepository<Employee> repository, 
            IMapper mapper,
            ILogger logger,
            ICompanyService companyService)
            : base(repository, mapper, logger)
        {
            _companyService = companyService;
        }

        public override async Task<bool> OnBeforeSave()
        {
            await base.OnBeforeSave();

            if (!await _companyService.FindByCode(DataInfo.CompanyCode, true))
                throw new ApplicationException("Company Code does not exist.");

            return true;
        }

        public override async Task<bool> FindByFilter(Expression<Func<Employee, bool>> filter,
            bool onlyCheckIfExists = false)
        {
            if (!await base.FindByFilter(filter, onlyCheckIfExists)) return false;

            if (!onlyCheckIfExists)
            {
                await _companyService.FindByCode(DataInfo.CompanyCode);
                DataInfo.CompanyName = _companyService.DataInfo.CompanyName;
            }

            return true;
        }

        public override async Task<bool> GetAll(Expression<Func<Employee, bool>> filter = null,
            Expression<Func<Employee, Employee>> selector = null,
            int currentPage = -1, int pageSize = -1,
            Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null)
        { 
            await base.GetAll();

            List<string> companyCodes = DataListInfo.Select(x => x.CompanyCode).Distinct().ToList();
            await _companyService.GetAll(filter: x => companyCodes.Contains(x.Code));

            foreach (var data in DataListInfo)
            {
                data.CompanyName = _companyService.DataListInfo
                    .First(x => x.CompanyCode == data.CompanyCode).CompanyName;
            }

            return true;
        }

    }
}
