
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.SalaryWithheld
{
    public class SalaryWithheldModel : BaseViewModel
    {
        public SalaryWithheldModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.UtcNow;

            this.EmployeeList = new List<SelectListItem>();
        }

        #region Standard Property

        [Required]
        [DisplayName("Employee ID")]
        public int EmployeeId { get; set; }

        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        [DisplayName("Employee Designation")]
        [UIHint("_ReadOnly")]
        public string EmployeeDesignation { get; set; }

        [DisplayName("Salary Month")]
        [Required]
        [StringLength(50)]
        public string SalaryMonth { get; set; }

        [DisplayName("Salary Year")]
        [Required]
        [StringLength(4)]
        public string SalaryYear { get; set; }
        
        [DisplayName("Reason")]
        [StringLength(500)]
        public string Reason { get; set; }

        [DisplayName("Remarks")]
        [StringLength(500)]
        public string Remarks { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }
        public IList<SelectListItem> SalaryMonthList { get; set; }
        public IList<SelectListItem> SalaryYearList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }
        #endregion
    }
}