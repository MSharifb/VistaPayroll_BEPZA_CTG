using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxExemptionRule
{
    public class TaxExemptionRuleViewModel : BaseViewModel
    {
        public TaxExemptionRuleViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.IncomeYearList = new List<SelectListItem>();
            this.DetailList = new List<TaxExemptionRuleDetailViewModel>();
            this.HeadList = new List<SelectListItem>();
            this.ExemptionBasisList = new List<SelectListItem>();
        }
        #region Standard
        
        [Required]
        [DisplayName("Income Year")]
        public string IncomeYear { get; set; }

        [DisplayName("Assessment Year")]
        public string AssessmentYear { get; set; }
        
        public bool IsActive { get; set; } 
        
        #endregion

        #region Other
        [DisplayName("Salary Head")]
        public int? HeadId { get; set; }

        [DisplayName("Basis of Calculation")]
        public byte? TaxExemptionBasisOn { get; set; }

        public bool IsPercentageOnBasis { get; set; }

        [DisplayName("Percentage On Basis")]
        [Range(typeof(decimal), "-1", "999",ErrorMessage = "Range exceeded for Percentage On Basis.")]
        public decimal? TaxableInPercentage { get; set; }
        
        public bool HaveYearlyExemptionLimit { get; set; }
        
        [DisplayName("Yearly Exemption Limit")]
        public decimal? YearlyExemptionLimitAmount { get; set; }
        
        [DisplayName("Monthly Exemption Limit")]
        public decimal? MonthlyExemptionLimitAmount { get; set; }
        
        public IList<SelectListItem> IncomeYearList { get; set; }
        
        public IList<TaxExemptionRuleDetailViewModel> DetailList { get; set; }
        
        public IList<SelectListItem> HeadList { get; set; }
        
        public IList<SelectListItem> ExemptionBasisList { get; set; } 


        #endregion
    }
}