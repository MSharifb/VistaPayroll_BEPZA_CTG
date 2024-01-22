using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxWithheld
{
    public class TaxWithheldViewModel : BaseViewModel
    {
        public TaxWithheldViewModel()
        {

            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.TaxYearList = new List<SelectListItem>();
            this.TaxMonthList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
        }

        #region Standard Property
        
        [Required]
        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        [StringLength(20)]
        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        [StringLength(500)]
        public string EmployeeName { get; set; }

        [DisplayName("Employee Designation")]
        [UIHint("_ReadOnly")]
        [StringLength(500)]
        public string EmployeeDesignation { get; set; }

        [DisplayName("Tax Month")]
        [Required]
        [StringLength(50)]
        public string TaxMonth { get; set; }

        [DisplayName("Tax Year")]
        [Required]
        [StringLength(4)]
        public string TaxYear { get; set; }


        [DisplayName("Reason")]
        [StringLength(500)]
        public string Reason { get; set; }

        [DisplayName("Remarks")]
        [StringLength(500)]
        public string Remarks { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }

        public IList<SelectListItem> TaxMonthList { get; set; }
        public IList<SelectListItem> TaxYearList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }

        #endregion
    }
}