using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.Refreshment
{
    public class RefreshmentViewModel : BaseViewModel
    {
        public RefreshmentViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.UtcNow;

            this.MonthList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
        }

        #region Standard Property

        [Required]
        [DisplayName("Employee ID")]
        public int EmployeeId { get; set; }

        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        public string Designation { get; set; }
        public int DesignationId { get; set; }

        [DisplayName("Account No")]
        public string AccountNo { get; set; }

        [Required]
        [DisplayName("Month")]
        public string RMonth { get; set; }

        [Required]
        [DisplayName("Year")]
        public string RYear { get; set; }

        [Required]
        [DisplayName("Per-Day Amount")]
        public decimal PerDayAmount { get; set; }

        [Required]
        [UIHint("_OnlyNumber")]
        [Range(1, 999)]
        [DisplayName("Total Days")]
        public decimal TotalDays { get; set; }

        [Required]
        [UIHint("_OnlyNumber")]
        [Range(0, 999.99)]
        [DisplayName("Revenue Stamp")]
        public decimal RevenueStamp { get; set; }

        [Required]
        [UIHint("_OnlyNumber")]
        [DisplayName("Net Payable")]
        public decimal NetPayable { get; set; }


        public int ZoneIdDuringRefreshment { get; set; }
        #endregion

        #region Other

        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }

        #endregion
    }
}