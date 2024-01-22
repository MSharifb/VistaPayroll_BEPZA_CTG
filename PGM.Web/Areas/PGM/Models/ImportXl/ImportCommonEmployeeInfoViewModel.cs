using System;

namespace PGM.Web.Areas.PGM.Models.ImportXl
{
    public class ImportCommonEmployeeInfoViewModel
    {   
        public int Id { get; set; }
        public String EmpID { get; set; }
        public String FullName { get; set; }
        public String DesigName { get; set; }
        public String DeptName { get; set; }
        public String AccountNumber { get; set; }
        public Decimal BasicSalary { get; set; }
        public Boolean IsEligibleForOvertime { get; set; }
        public Boolean IsEligibleForRefreshment { get; set; }
    }
}