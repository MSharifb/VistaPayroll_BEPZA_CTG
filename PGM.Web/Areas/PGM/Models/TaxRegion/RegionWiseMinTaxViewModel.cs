using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxRegion
{
    public class RegionWiseMinTaxViewModel : BaseViewModel
    {
        public RegionWiseMinTaxViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.AssessmentYearList = new List<SelectListItem>();
            this.IncomeYearList = new List<SelectListItem>();
            this.RegionList = new List<SelectListItem>();
        }
        [DisplayName("Region")]
        public int RegionId { get; set; }
        public string Region { get; set; }
        [Required]
        [DisplayName("Income Year")]
        public string IncomeYear { get; set; }
        [Required]
        [DisplayName("Assessment Year")]
        public string AssessmentYear { get; set; }
        [Required]
        [Range(typeof(decimal), "1", "79228162514264337593543950335")]
        [DisplayName("Minimum Amount")]
        public decimal MinimumAmount { get; set; }
        public IList<SelectListItem> RegionList { get; set; }
        public IList<SelectListItem> IncomeYearList { get; set; }
        public IList<SelectListItem> AssessmentYearList { get; set; }

    }
}