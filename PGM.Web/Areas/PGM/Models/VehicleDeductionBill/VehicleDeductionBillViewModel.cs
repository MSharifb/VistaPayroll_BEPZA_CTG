using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.VehicleDeductionBill
{
    public class VehicleDeductionBillViewModel : BaseViewModel
    {
        public VehicleDeductionBillViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = CrudeAction.Create;

            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();


        }

        #region Standard Property

        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [Required]
        public string EmpID { get; set; }

        [DisplayName("Month")]
        [Required]
        public string SalaryMonth { get; set; }

        [DisplayName("Year")]
        [Required]
        public string SalaryYear { get; set; }

        [DisplayName("Employee Name")]
        [Required]
        public string FullName { get; set; }

        [DisplayName("Employee Initial")]
        public string EmployeeInitial { get; set; }

        [DisplayName("Designation")]
        public string Designation { get; set; }

        [DisplayName("Personal Use")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal PersonalAmount { get; set; }

        [DisplayName("Official Use")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal OfficalAmount { get; set; }

        public string Remarks { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }

        #endregion
    }
}