using System;

namespace DAL.PGM.CustomEntities
{
    public class OtherAdjustDeductSearchModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; }

        public String HeadType { get; set; }
        public String SalaryHead { get; set; }
        public String AmountType { get; set; }
        public Decimal Amount { get; set; }

        public bool IsOverrideStructureAmount { get; set; }
    }
}
