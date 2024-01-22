using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.GroupInsurancePaymentType
{
    public class GroupInsurancePaymentTypeViewModel : BaseViewModel
    {
        public GroupInsurancePaymentTypeViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.Mode = CrudeAction.Create;
        }
        [Required]
        [DisplayName("Group Insurance Payment Type")]
        public string PaymentType { get; set; }
        [Required]
        [DisplayName("Amount")]
        [UIHint("_OnlyCurrency")]
        public decimal PaymentAmount { get; set; }
        [Required]
        [DisplayName("Order No.")]
        public string OrderNo { get; set; }
        [Required]
        [DisplayName("Effective Date")]
        [UIHint("_Date")]
        public DateTime EffectiveDate { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        #region Others
        public string Mode { get; set; }
        #endregion 
    }
}