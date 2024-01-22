using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxInvestmentRebateRule
{
    public class TaxInvestmentRebateRuleMasterViewModel : BaseViewModel
    {
        public TaxInvestmentRebateRuleMasterViewModel()
        {
            this.RuleDetail = new List<TaxInvestmentRebateRuleDetailViewModel>();
            this.IncomeYearList = new List<SelectListItem>();
        }

        #region standard
        [Required]
        public int RebateRuleId { get; set; }
        [Required]
        [DisplayName("Slab No.")]
        public int SlabNo { get; set; }
        [DisplayName("Total Income Lower Limit")]
        public decimal LowerLimit { get; set; }
        [DisplayName("Total Income Upper Limit")]
        public decimal UpperLimit { get; set; }

        #endregion

        #region other
        [Required]
        [DisplayName("Income Year")]
        public string IncomeYear { get; set; }
        [DisplayName("Assessment Year")]
        public string AssessmentYear { get; set; }
        public IList<SelectListItem> IncomeYearList { get; set; }
        public IList<TaxInvestmentRebateRuleDetailViewModel> RuleDetail { get; set; }

        #endregion
    }
}