using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxInvestmentRebateRule
{
    public class TaxInvestmentRebateRuleViewModel : BaseViewModel
    {
        public TaxInvestmentRebateRuleViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.IncomeYearList = new List<SelectListItem>();
            this.MasterList = new List<TaxInvestmentRebateRuleMasterViewModel>();
        }

        #region Standard
        [Required]
        [DisplayName("Income Year")]
        public string IncomeYear { get; set; }
        [DisplayName("Assessment Year")]
        public string AssessmentYear { get; set; }

        #endregion

        #region other
        public IList<SelectListItem> IncomeYearList { get; set; }
        public IList<TaxInvestmentRebateRuleMasterViewModel> MasterList { get; set; }

        #endregion
    }


}