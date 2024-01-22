using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxAdvancedPaid
{
    public class AdvancedTaxPaidViewModel : BaseViewModel
    {
        public AdvancedTaxPaidViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.AssessmentYearList = new List<SelectListItem>();
            this.IncomeYearList = new List<SelectListItem>();
            this.AdvanceDetailList = new List<AdvancedTaxPaidDetailViewModel>();
        }
        
        [Required]
        [DisplayName("Assessment Year")]
        public string AssessmentYear { get; set; }
        
        [Required]
        [DisplayName("Income Year")]
        public string IncomeYear { get; set; }

        [Required]
        [DisplayName("Employee")]
        public int EmployeeId { get; set; }
        
        [DisplayName("Employee ID")]
        public string EmpId { get; set; }
        
        public string EmployeeName { get; set; }

        #region other

        
        public IList<SelectListItem> AssessmentYearList { get; set; }
        public IList<SelectListItem> IncomeYearList { get; set; }
        public IList<AdvancedTaxPaidDetailViewModel> AdvanceDetailList { get; set; }

        #endregion

    }
}