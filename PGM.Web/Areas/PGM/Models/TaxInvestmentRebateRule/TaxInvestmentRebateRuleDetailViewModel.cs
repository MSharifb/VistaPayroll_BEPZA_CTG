using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.TaxInvestmentRebateRule
{
    public class TaxInvestmentRebateRuleDetailViewModel : BaseViewModel
    {
        public TaxInvestmentRebateRuleDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #region standard
        [Required]
        public int RebateRuleMasterId { get; set; }
        
        [DisplayName("Slab #")]
        [Required]
        public int SlabNo { get; set; }

        [Required]
        [DisplayName("Eligible Amount From")]
        public decimal LowerLimit { get; set; }
        
        [Required]
        [DisplayName("Eligible Amount To")]
        public decimal UpperLimit { get; set; }
        
        [Required]
        [DisplayName("Rebate Rate")]
        public decimal RebateRate { get; set; }
        #endregion
    }
}