using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.OverTime
{
    public class OvertimeModel : LongBaseViewModel
    {
        public OvertimeModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.UtcNow;

            YearList = new List<SelectListItem>();
            MonthList = new List<SelectListItem>();
            EmployeeList = new List<OvertimeModel>();
            EmployeeListForSearch = new List<SelectListItem>();

            strMode = "Create";
        }

        [Required]
        [DisplayName("EmployeeId")]
        public int EmployeeId { get; set; }

        [DisplayName("DesignationId")]
        public int DesignationId { get; set; }

        [DisplayName("Account No")]
        public string AccountNo { get; set; }

        [Required]
        [DisplayName("IsImpactToSalary")]
        public bool IsImpactToSalary { get; set; }

        [Required]
        [DisplayName("Year")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Year")]
        public int OTYear { get; set; }

        [Required]
        [DisplayName("Month")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Month")]
        public int OTMonth { get; set; }
        public string MonthName { get; set; }

        [Required]
        [DisplayName("Basic Salary")]
        public decimal BasicSalary { get; set; }

        [Required]
        [DisplayName("Worked Hours")]
        [UIHint("_OnlyNumber")]
        [Range(1, 9999.99)]
        public decimal WorkedHours { get; set; }

        [Required]
        [DisplayName("Approved Hours")]
        [UIHint("_OnlyNumber")]
        [Range(1, 9999.99)]
        public decimal ApprovedHours { get; set; }

        [Required(ErrorMessage = "Overtime rate is auto calculating based on basic salary of selected year and month.")]
        [DisplayName("Overtime Rate")]
        [Range(0.000001, 9999, ErrorMessage = "Overtime rate is auto calculating based on basic salary of selected year and month.")]
        public decimal OvertimeRate { get; set; }

        [Required]
        [DisplayName("Revenue Stamp")]
        [UIHint("_OnlyNumber")]
        [Range(0, 999)]
        public decimal RevenueStamp { get; set; }

        [DisplayName("Deduction Percentage")]
        [UIHint("_OnlyNumber")]
        [Range(0, 100)]
        public decimal? DeductionPercentage { get; set; }

        [DisplayName("Net Payable")]
        public decimal NetPayable { get; set; }

        [DisplayName("Remarks")]
        [StringLength(200)]
        public string Remarks { get; set; }


        public int ZoneIdDuringOvertime { get; set; }

        #region Others Properties

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<OvertimeModel> EmployeeList { get; set; }
        public IList<SelectListItem> EmployeeListForSearch { get; set; }

        [DisplayName("Employee ID")]
        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Designation")]
        public string Designation { get; set; }

        #endregion
    }
}