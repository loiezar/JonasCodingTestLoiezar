using DataAccessLayer.Model.Interfaces;
using AutoMapper;
using BusinessLayer.Model.Models;
using BusinessLayer.Model.Interfaces;
using System.Collections.Generic;
using DataAccessLayer.Model.Models;

namespace BusinessLayer.Services
{
    public class TestService : ITestService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public TestService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }
        public IEnumerable<CompanyInfo> GetAllCompanies()
        {
            var res = _companyRepository.GetAll();
            return _mapper.Map<IEnumerable<CompanyInfo>>(res);
        }

        public CompanyInfo GetCompanyByCode(string companyCode)
        {
            var result = _companyRepository.GetByCode(companyCode);
            return _mapper.Map<CompanyInfo>(result);
        }

        public bool SaveCompany(CompanyInfo companyInfo)
        {
            _companyRepository.SaveCompany(_mapper.Map<Company>(companyInfo));
            return true;
        }
    }
}
