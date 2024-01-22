using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.EmpChargeAllowance
{
    public class EmpChargeAllowanceModel : LongBaseViewModel
    {
        #region Ctor

        public EmpChargeAllowanceModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;

            this.SalaryHeadList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

        [DisplayName("Employee Id")]
        [Required]
        public int EmployeeId { get; set; }

        [DisplayName("Basic Salary")]
        public Decimal BasicSalary { get; set; }

        [DisplayName("Start Date")]
        [Required]
        [UIHint("_Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        [UIHint("_Date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Actual End Date")]
        [UIHint("_Date")]
        public DateTime? ActualEndDate { get; set; }

        [DisplayName("Continuous")]
        [Required]
        public bool IsContinuous { get; set; }

        [DisplayName("Monthly Allowance")]
        [UIHint("_OnlyCurrency")]
        public decimal MonthlyAllowance { get; set; }

        [DisplayName("Charge/Duty Name")]
        [MaxLength(500)]
        public string ChargeDutyName { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        public IList<SelectListItem> BaseOnList { get; set; }

        public bool HasData { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }

        public String Designation { get; set; }
        public String Department { get; set; }
        [DisplayName("Max Allownace")]
        public Decimal MaxAllowance { get; set; }
        public String EmpID { get; set; }
        public int DesignationId { get; set; }
        public string FullName { get; set; }

        #endregion
    }
}