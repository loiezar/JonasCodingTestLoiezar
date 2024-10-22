
namespace DataAccessLayer.Model.Models
{
    public class Employee : DataEntity
    {
        public string EmployeeName { get; set; }
        public string Occupation { get; set; }
        public string EmployeeStatus { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }

        public string CompanyName { get; set; }
    }
}
