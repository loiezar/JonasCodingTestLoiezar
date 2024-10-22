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
    public class EmployeeController : BaseApiController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET api/<controller>
        public async Task<IEnumerable<EmployeeDto>> GetAll()
        {
            await _employeeService.GetAll();
            return _mapper.Map<IEnumerable<EmployeeDto>>(_employeeService.DataListInfo);   
        }

        // GET api/<controller>/5
        public async Task<EmployeeDto> Get(string code)
        {
            await _employeeService.FindByCode(code);
            return _mapper.Map<EmployeeDto>(_employeeService.DataInfo);
        }

        // POST api/<controller>
        public async Task<EmployeeDto> Post([FromBody]EmployeeDto employeeDto)
        {
            employeeDto.Code = string.Empty;
            await _employeeService.AddOrEdit(_mapper.Map<EmployeeInfo>(employeeDto));
            await _employeeService.FindByCode(_employeeService.DataInfo.Code);
            
            return _mapper.Map<EmployeeDto>(_employeeService.DataInfo);
        }

        // PUT api/<controller>/5
        public async Task<EmployeeDto> Put(string code, [FromBody] EmployeeDto employeeDto)
        {
            employeeDto.Code = code;
            await _employeeService.AddOrEdit(_mapper.Map<EmployeeInfo>(employeeDto));
            await _employeeService.FindByCode(_employeeService.DataInfo.Code);
            return _mapper.Map<EmployeeDto>(_employeeService.DataInfo);
        }

        // DELETE api/<controller>/5
        public async Task Delete(string code)
        {
            await _employeeService.Delete(code);
        }
    }
}