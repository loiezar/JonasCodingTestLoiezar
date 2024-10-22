using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Repositories;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class CompanyController : BaseApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;

        public CompanyController(ICompanyService companyService, IMapper mapper)
        {
            _companyService = companyService;
            _mapper = mapper;
        }

        // GET api/<controller>
        public async Task<IEnumerable<CompanyDto>> GetAll()
        {
            await _companyService.GetAll();
            return _mapper.Map<IEnumerable<CompanyDto>>(_companyService.DataListInfo);   
        }

        // GET api/<controller>/5
        public async Task<CompanyDto> Get(string code)
        {
            await _companyService.FindByCode(code);
            return _mapper.Map<CompanyDto>(_companyService.DataInfo);
        }

        // POST api/<controller>
        public async Task<CompanyDto> Post([FromBody]CompanyDto companyDto)
        {
            companyDto.Code = string.Empty;
            await _companyService.AddOrEdit(_mapper.Map<CompanyInfo>(companyDto));
            await _companyService.FindByCode(_companyService.DataInfo.Code);
            return _mapper.Map<CompanyDto>(_companyService.DataInfo);
        }

        // PUT api/<controller>/5
        public async Task<CompanyDto> Put(string code, [FromBody] CompanyDto companyDto)
        {
            companyDto.Code = code;
            await _companyService.AddOrEdit(_mapper.Map<CompanyInfo>(companyDto));
            await _companyService.FindByCode(_companyService.DataInfo.Code);
            return _mapper.Map<CompanyDto>(_companyService.DataInfo);
        }

        // DELETE api/<controller>/5
        public async Task Delete(string code)
        {
            await _companyService.Delete(code);
        }
    }
}