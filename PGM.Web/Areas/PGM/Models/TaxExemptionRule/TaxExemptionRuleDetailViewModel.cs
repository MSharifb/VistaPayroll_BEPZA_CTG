using System;
using System.ComponentModel;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.TaxExemptionRule
{
    public class TaxExemptionRuleDetailViewModel : BaseViewModel
    {
        public TaxExemptionRuleDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
          
        }
        #region Standard
        public int ExemptionId { get; set; }
        public int HeadId { get; set; }
        
        public string HeadName { get; set; }

        public byte? TaxExemptionBasisOn { get; set; }
        
        public string TaxExemptionBasis { get; set; }
        
        public bool? IsPercentageOnBasis { get; set; }
        
        public decimal? TaxableInPercentage { get; set; }
        
        public bool? HaveYearlyExemptionLimit { get; set; }
        
        public decimal? YearlyExemptionLimitAmount { get; set; } 
        #endregion


    }
}