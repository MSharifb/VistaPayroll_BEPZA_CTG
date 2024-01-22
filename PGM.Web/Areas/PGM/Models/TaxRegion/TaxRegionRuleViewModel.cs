using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.TaxRegion
{
    public class TaxRegionRuleViewModel : BaseViewModel
    {
        public TaxRegionRuleViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }
        [DisplayName("Region Name")]
        [Required]
        public string RegionName { get; set; }
        public bool? IsActive { get; set; }
    }
}