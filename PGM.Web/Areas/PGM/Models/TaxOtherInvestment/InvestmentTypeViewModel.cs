using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.TaxOtherInvestment
{
    public class InvestmentTypeViewModel : BaseViewModel
    {
        public InvestmentTypeViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }
        [Required]
        [DisplayName("Investment Type")]
        public string InvestmentTypeName { get; set; }
        public bool IsActive { get; set; }
    }
}