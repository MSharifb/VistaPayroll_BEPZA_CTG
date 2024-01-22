using System;

namespace DAL.PGM.CustomEntities
{
    public class GratuitySettlementSearchModel
    {
        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string FullName { get; set; }
        public string ServiceLength { get; set; }
        public decimal PayableAmount { get; set; }
        public DateTime DateofPayment { get; set; }
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        
        public bool IsPaid { get; set; }
        public bool IsPaidWithFinalSettlement { get; set; } 

        public string PaymentStatus { get; set; }
    }
}
