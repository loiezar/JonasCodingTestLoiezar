using System;

namespace DataAccessLayer.Model.Models
{
	public class DataEntity
	{
		public string SiteId { get; set; }// table name?
        public string Code { get; set; } // primary key?
        public string CompanyCode { get; set; } //Multi-tenant by company?
        public DateTime LastModified { get; set; }
    }
}
