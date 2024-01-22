using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.FinalSettlement
{
    public class FinalSettlementViewModel : BaseViewModel
    {
        public FinalSettlementViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.Mode = CrudeAction.Create;
        }

        #region Standard Properties

        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [Required]
        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        public string FullName { get; set; }

        [DisplayName("Initial")]
        public string EmployeeInitial { get; set; }

        [DisplayName("Department")]
        public string Division { get; set; }

        [DisplayName("Designation")]
        public string Designation { get; set; }

        [DisplayName("Date of Settlement")]
        [UIHint("_Date")]
        [Required]
        public DateTime DateofSettlement { get; set; }

        [DisplayName("Date of Confirmation")]
        public string DateofConfirmation { get; set; }

        [DisplayName("Date of Joining")]
        public string DateofJoining { get; set; }

        [DisplayName("Date of Seperation")]
        public string DateofSeperation { get; set; }

        [DisplayName("Basic Salary")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal BasicSalary { get; set; }

        [DisplayName("Gross Salary")]
        [UIHint("_OnlyCurrency")]
        public decimal GrossSalary { get; set; }

        [DisplayName("Shortage Days of notice period")]
        public int ShortageDays { get; set; }

        [DisplayName("Earn Leave Balance")]
        public decimal EarnLeaveBalance { get; set; }

        [DisplayName("Adjusted Leave days")]
        public int AdjustLeave { get; set; }

        [DisplayName("Unadjusted Leave days")]
        public int UnAdjsutedLeave { get; set; }

        [DisplayName("No. of days worked in last month")]
        [Range(1, 9999, ErrorMessage = "No. of days worked in last month must be greater than zero.")]
        public int LastMonthWorkedDays { get; set; }

        [DisplayName("Salary Payable")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal SalaryPayable { get; set; }

        [DisplayName("Leave Encashment")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal LeaveEncasement { get; set; }

        [DisplayName("Gratuity Payable")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal GratuityPayable { get; set; }

        [DisplayName("Other Adjustment")]
        [UIHint("_OnlyCurrency")]
        public decimal OtherAdjustment { get; set; }

        [DisplayName("Shortage of Notice Period Deduction")]
        [UIHint("_OnlyCurrency")]
        public decimal ShortageofNoticePeriod { get; set; }

        [DisplayName("Other Deduction")]
        [UIHint("_OnlyCurrency")]
        public decimal OtherDeduction { get; set; }

        [DisplayName("Advance Deduction")]
        [UIHint("_OnlyCurrency")]
        public decimal AdvanceDeduction { get; set; }

        [DisplayName("Net Payable")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal NetPayable { get; set; }

        [DisplayName("Date of Birth")]
        public string DateofBirth { get; set; }

        [DisplayName("Service Length")]
        public string ServiceDuration { get; set; }

        [DisplayName("Yearly Salary Increase")]
        public decimal YearlySalaryIncrease { get; set; }
        [DisplayName("Basic Salary after Increase")]
        public decimal BasicSalaryIncrease { get; set; }
        public decimal MultipleBy1 { get; set; }
        public decimal MultipleBy2 { get; set; }


        public decimal TotalFullEarnLeave { get; set; }
        public decimal TotalEncashLeave { get; set; }
        public decimal NetEncashLeave { get; set; }

        public decimal EmpContribution { get; set; }
        public decimal ComContribution { get; set; }
        public decimal EmpProftInPeriod { get; set; }
        public decimal ComProftInPeriod { get; set; }
        public decimal ForfeitedAmount { get; set; }
        public decimal WithdrawnAmount { get; set; }
        public decimal DueLoan { get; set; }
        public decimal NetPFBalance { get; set; }



        [DisplayName("Remarks")]
        public string Remarks { get; set; }


        #endregion


        #region Other properties

        public string Mode { get; set; }

        public string ErrClss { set; get; }

        #endregion
    }
}