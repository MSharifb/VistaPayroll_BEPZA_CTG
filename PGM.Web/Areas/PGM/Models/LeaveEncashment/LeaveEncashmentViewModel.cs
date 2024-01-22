using System;
using System.Collections.Generic;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.LeaveEncashment
{
    public class LeaveEncashmentViewModel:BaseViewModel
    {
        public LeaveEncashmentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.ProjectList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.LeaveTypeList = new List<SelectListItem>();

            this.Mode = CrudeAction.Create;
        }


        #region Standard Properties
        
        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [Required]
        public string EmpID { get; set; }

        [DisplayName("Leave Type")]
        [Required]
        public int LeaveTypeId { get; set; } 

        [DisplayName("Payment Month")]
        [Required]
        public string SalaryMonth { get; set; }

        [DisplayName("Payment Year")]
        [Required]
        public string SalaryYear { get; set; }

        [DisplayName("Employee Name")]
        //[ReadOnly(true)]
        public string FullName { get; set; }

        [DisplayName("Department")]
        //[ReadOnly(true)]
        public string Division { get; set; } 

        [DisplayName("Balance (Days)")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal LeaveBalance { get; set; }

        [DisplayName("Encashment Days")]
        [Required]
        [Range(1, 99999999, ErrorMessage = "Encashment Days must be greater than zero.")]
        public Decimal EncashmentDays { get; set; }

        [DisplayName("Encashment Rate")]
        [Required]
        [UIHint("_OnlyCurrency")]
        [Range(1, 99999999, ErrorMessage = "Encashment Rate must be greater than zero.")]
        public Decimal EncashmentRate { get; set; }

        [DisplayName("Amount")]
        [Required]
        [UIHint("_OnlyCurrency")]
        [Range(1, 99999999, ErrorMessage = "Amount must be greater than zero.")]
        public Decimal EncashmentAmount { get; set; }

        [DisplayName("Current Basic Salary")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal BasicSalary { get; set; }

        [DisplayName("Gross Salary")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal GrossSalary { get; set; } 

        //[DisplayName("Account No.")]
        //public string AccountNo { get; set; } 

        //public int? BankId { get; set; }
        //public int? BranchId { get; set; }

        //[DisplayName("Bank and Branch")]
        //public string BankNBranch { get; set; }

        
        [DisplayName("Order Ref. No.")]
        public string OrderNo { get; set; }
       
        [DisplayName("Order Date")]
        [UIHint("_Date")]
        public DateTime? OrderDate { get; set; }
        public string Remarks { get; set; }

        public int? intLeaveYearID { get; set; }

        [DisplayName("Leave Year")]
        public string LeaveYear { get; set; }

        [DisplayName("Is Adjust With Salary")]
        public Boolean IsAdjustWithSalary { get; set; }

        [DisplayName("Salary Head")]
        public int? SalaryHeadId { get; set; }
        
        [DisplayName("Payment Date")]
        [UIHint("_Date")]
        public DateTime? PaymentDate { get; set; }

        #endregion

        #region Other properties

        public string Mode { get; set; }

        public IList<SelectListItem> ProjectList { get; set; }
        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> LeaveTypeList { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }

        #endregion
    }
}