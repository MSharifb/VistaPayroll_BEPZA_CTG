using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxOpening
{
    public class TaxOpeningModel : BaseViewModel
    {
        public TaxOpeningModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            
            this.IncomeYearList = new List<SelectListItem>();
            this.TaxOpeningDetailList = new List<TaxOpeningDetailModel>();
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

        [DisplayName("Income Year")]
        [Required]
        [StringLength(9)]
        public string IncomeYear { get; set; }

        [DisplayName("Assessment Year")]
        [Required]
        [StringLength(9)]
        [UIHint("_ReadOnly")]
        public string AssessmentYear { get; set; }

        [DisplayName("Income From Date")]
        [UIHint("_ReadOnlyDate")]
        [Required]
        public DateTime FromDate { get; set; }

        [DisplayName("Income To Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime ToDate { get; set; }

        [DisplayName("Tax Deducted")]
        [Required]
        [UIHint("_OnlyNumber")]
        [Range(1, 9999999999, ErrorMessage = "Tax deducted amount must be greater than 0.")]
        public Decimal TaxDeducted { get; set; }

        public Decimal TotalAmount { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }

        public IList<SelectListItem> IncomeYearList { get; set; }

        [Required(ErrorMessage = "Please, enter taxable income opening balance in each head.")]
        public virtual ICollection<TaxOpeningDetailModel> TaxOpeningDetailList { get; set; }

        public IList<SelectListItem> EmployeeList { get; set; }

        #endregion
    }

}
