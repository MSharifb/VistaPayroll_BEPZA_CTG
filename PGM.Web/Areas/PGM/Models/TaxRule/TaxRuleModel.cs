using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DAL.PGM;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.TaxRule
{
    public class TaxRuleModel : BaseViewModel
    {
        #region Ctor

        public TaxRuleModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = CrudeAction.Create;

            this.IncomeYearList = new List<SelectListItem>();
            this.AssessmentYearList = new List<SelectListItem>();
            this.SalaryHeadList = new List<SelectListItem>();
            this.TaxBasedOnList = new List<SelectListItem>();

            this.TaxRuleList = new List<TaxRuleModel>();
        }

        #endregion

        #region Standard Property

        [DisplayName("Income Year")]
        [Required]
        [StringLength(9)]        
        public string IncomeYear { get; set; }

        [DisplayName("Assessment Year")]
        [Required]
        [StringLength(9)]
        [UIHint("_ReadOnly")]
        public string AssessmentYear { get; set; }

        [DisplayName("Investment Rate")]
        [Required]
        [UIHint("_OnlyNumber")]
        public decimal InvestmentRate { get; set; }

        [DisplayName("Maximum Investment")]
        [Required]
        [UIHint("_OnlyNumber")]
        public decimal MaximumInvestment { get; set; }
        
        [DisplayName("Excess Income Percent")]
        [Required]
        [UIHint("_OnlyNumber")]
        public decimal ExcessIncomePercent { get; set; }

        [DisplayName("Special Rebate Rate")]
        [Required]
        [UIHint("_OnlyNumber")]
        public decimal SpecialRebateRate { get; set; }


        [DisplayName("Yearly Advance Tax Percentage")]
        [Required]
        [UIHint("_OnlyNumber")]
        public decimal TaxablePercentage { get; set; }

        [DisplayName("Tax Free Amount For Having Child With Disability")]
        [Required]
        [UIHint("_OnlyNumber")]
        public decimal TaxFreeAmountForHavingChildWithDisability { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }

        public IList<SelectListItem> IncomeYearList { get; set; }
        public IList<SelectListItem> AssessmentYearList { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        public IList<SelectListItem> TaxBasedOnList { get; set; }

        public IList<TaxRuleModel> TaxRuleList { get; set; }

        #endregion
    }

}
