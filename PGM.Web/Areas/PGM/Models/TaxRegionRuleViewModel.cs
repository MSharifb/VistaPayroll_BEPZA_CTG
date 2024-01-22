using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERP_BEPZA.Web.Areas.PGM.Models
{
    public class TaxRegionRuleViewModel : BaseViewModel
    {
        [DisplayName("Region Name")]
        [Required]
        public string RegionName { get; set; }
        public bool? IsActive { get; set; }
    }
}