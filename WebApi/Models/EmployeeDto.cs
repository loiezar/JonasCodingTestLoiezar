using Newtonsoft.Json;
using System;

namespace WebApi.Models
{
    public class EmployeeDto : BaseDto
    {
        public string EmployeeName { get; set; }
        public string OccupationName { get; set; }
        public string EmployeeStatus { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }

        public string CompanyName { get; set; }
        public DateTime LastModifiedDateTime { get; set; }

        [JsonIgnore]
        public new DateTime? LastModified { get; set; } 
    }
}