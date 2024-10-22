using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Models;
using System.Collections.Generic;

namespace BusinessLayer.Model.Interfaces
{
    public interface ITestService
    {
        IEnumerable<CompanyInfo> GetAllCompanies();
        CompanyInfo GetCompanyByCode(string companyCode);
        bool SaveCompany(CompanyInfo companyInfo);
    }
}
