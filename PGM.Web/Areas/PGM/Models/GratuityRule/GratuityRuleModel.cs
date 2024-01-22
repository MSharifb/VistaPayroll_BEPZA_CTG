using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.GratuityRule
{
    public class GratuityGrossSalaryHeadModel : BaseViewModel
    {
        public GratuityGrossSalaryHeadModel() { }

        [Required]
        public int HeadId { get; set; }
    }

    public class GratuityRuleModel : BaseViewModel
    {
        #region Ctor

        public GratuityRuleModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = CrudeAction.Create;

            //this.GratuityGrossSalaryHeadList = new List<GratuityGrossSalaryHeadModel>();
            this.SalaryHeadList = new List<SelectListItem>();
            this.GratuityEligibleFromList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

        [DisplayName("Effective Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime EffectiveDate { get; set; }

        [DisplayName("Eligible From")]
        [Required]
        public int GratuityEligibleFromId { get; set; }
        public bool IsEligibleFromJoiningDate { get; set; }

        [DisplayName("Eligible After Month of")]
        [UIHint("_OnlyInteger")]
        [Required]
        public int EligibleAfterMonthof { get; set; }

        [DisplayName("Gross Salary Head(s)")]
        [Required(ErrorMessage = "Atlease one salary head is required.")]
        public int[] SalaryHeadIds { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }

        //public ICollection<GratuityGrossSalaryHeadModel> GratuityGrossSalaryHeadList { get; set; }
        public IList<SelectListItem> SalaryHeadList{ get; set; }
        public IList<SelectListItem> GratuityEligibleFromList { get; set; }
        
        #endregion
    }
}