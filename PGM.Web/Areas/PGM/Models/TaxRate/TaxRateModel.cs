using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace PGM.Web.Areas.PGM.Models.TaxRate
{
    public class TaxRateModel : BaseViewModel
    {
        public TaxRateModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.IncomeYearList = new List<SelectListItem>();
            this.ApplicableForList = new List<SelectListItem>();
        }

        #region Standard Property

        [DisplayName("Income Year")]
        [Required]
        [StringLength(9)]
        public string IncomeYear { get; set; }

        [DisplayName("Assessment Year")]
        [Required]
        [StringLength(9)]
        [UIHint("_ReadOnly")]
        public string AssessmentYear { get; set; }

        [DisplayName("Applicable For")]
        [Required]
        public byte AssesseeTypeId { get; set; }

        [DisplayName("Number Of Slab")]
        [Required]
        [UIHint("_OnlyInteger")]
        [Range(1, 10, ErrorMessage = "Number of Slab must be between 1 and 10.")]
        public int NumberOfSlab { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }

        public IList<SelectListItem> IncomeYearList { get; set; }

        public IList<SelectListItem> ApplicableForList { get; set; }

        [Required(ErrorMessage = "Provide at least one step name.")]
        public virtual ICollection<TaxRateDetailModel> TaxRateDetail { get; set; }


        #endregion
    }
}
