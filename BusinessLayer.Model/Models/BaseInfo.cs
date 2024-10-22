using System;

namespace BusinessLayer.Model.Models
{
    public class BaseInfo
    {
        public string SiteId { get; set; }// table name?
        public string Code { get; set; } // primary key?
        public string CompanyCode { get; set; } //Multi-tenant by company?
        public DateTime LastModified { get; set; }
    }
}