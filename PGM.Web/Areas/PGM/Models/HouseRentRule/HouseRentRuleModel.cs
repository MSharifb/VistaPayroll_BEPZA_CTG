using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.HouseRentRule
{
    public class HouseRentRuleModel : BaseViewModel
    {
        public HouseRentRuleModel()
        {
            this.SalaryScaleList = new List<SelectListItem>();
            this.RegionList = new List<SelectListItem>();
            this.SalaryHeadList = new List<SelectListItem>();
            this.BaseOnList = new List<SelectListItem>();
        }

        #region Standard Property

        [DisplayName("Salary Scale")]
        [Required]
        public int SalaryScaleId { get; set; }
        public string SalaryScale { get; set; }

        [DisplayName("Region")]
        [Required]
        public int RegionId { get; set; }
        public string Region { get; set; }

        [DisplayName("Salary Head (+)")]
        [Required]
        public int SalaryHeadId { get; set; }
        public string SalaryHead { get; set; }

        [DisplayName("Base On")]
        [Required]
        public string BaseOn { get; set; }
        public IList<SelectListItem> BaseOnList { get; set; }

        [DisplayName("Effective Date From")]
        [Required]
        [UIHint("_Date")]
        public DateTime? EffectiveDateFrom { get; set; }

        [DisplayName("Effective Date To")]
        [Required]
        [UIHint("_Date")]
        public DateTime? EffectiveDateTo { get; set; }

        #endregion

        #region Others

        [DisplayName("Number Of Slab")]
        [UIHint("_OnlyInteger")]
        [Range(1, 10, ErrorMessage = "Number of Slab must be between 1 and 10.")]
        public int NumberOfSlab { get; set; }

        public string Mode { get; set; }

        public IList<SelectListItem> SalaryScaleList { get; set; }
        public IList<SelectListItem> RegionList { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }

        [Required(ErrorMessage = "Provide at least one rule")]
        public virtual ICollection<HouseRentRuleDetailModel> HouseRentRuleDetail { get; set; }

        #endregion


    }
}
