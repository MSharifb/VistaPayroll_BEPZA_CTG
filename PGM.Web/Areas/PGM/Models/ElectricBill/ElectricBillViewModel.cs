using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.ElectricBill
{
    public class ElectricBillViewModel : BaseViewModel
    {
        public ElectricBillViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = CrudeAction.Create;
        }

        #region Standard Properties
        [Required]
        [DisplayName("Employee Id")]
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("Bill No.")]
        public string BillNo { get; set; }
        [UIHint("_Date")]
        [DisplayName("Prepare Date")]
        public DateTime? PrepareDate { get; set; }
        [Required]
        [UIHint("_Date")]
        [DisplayName("From Date")]
        public DateTime FromDate { get; set; }
        [Required]
        [UIHint("_Date")]
        [DisplayName("To Date")]
        public DateTime ToDate { get; set; }
        [Required] 
        [DisplayName("Month")]
        public string BillMonth { get; set; }
        [Required]
        [DisplayName("Year")]
        public int BillYear { get; set; }
        [Required]
        [DisplayName("Meter Reading From")]
        public int? ReadingFrom { get; set; }
        [Required]
        [DisplayName("Meter Reading To")]
        public int? ReadingTo { get; set; }
        [Required]
        [DisplayName("Total Usages Unit")]
        public int? TotalUsageUnit { get; set; }
        [Required]
        [UIHint("_OnlyCurrency")]
        [DisplayName("Bill Amount")]
        public decimal BillAmount { get; set; }
        [DisplayName("Fine Amount")]
        [UIHint("_OnlyCurrency")]
        public decimal? FineAmount { get; set; }
        [Required]
        [DisplayName("Total Bill")]
        [UIHint("_OnlyCurrency")]
        public decimal? TotalBill { get; set; }
        public bool IsDeductFromSalary { get; set; }
        [DisplayName("Cash Paid Amount")]
        [UIHint("_OnlyCurrency")]
        public decimal? CashPaidAmount { get; set; }
        [DisplayName("Salary Head")]
        public int? SalaryHeadId { get; set; }
        [DisplayName("Salary Month")]
        public string SalaryMonth { get; set; }
        [DisplayName("Salary Year")]
        public int? SalaryYear { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

       
        #endregion 

        #region Others
        public string SalaryHeadName { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        
        public string Mode { get; set; }
        
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Employee ID")]
        public string ICNo { get; set; }
        
        public string Designation { get; set; }
        [DisplayName("Meter No.")]
        public string ElectricMeterNo { get; set; }
        public string Department { get; set; }

        public IList<SelectListItem> MonthList { get; set; }

        public IList<SelectListItem> YearList { get; set; }

        #endregion 
    

    }
}