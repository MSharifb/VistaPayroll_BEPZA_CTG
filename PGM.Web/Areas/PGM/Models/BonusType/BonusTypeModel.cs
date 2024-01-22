using System;
using System.Collections.Generic;

using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.BonusType
{
    public class BonusTypeViewModel
    {

        #region Ctor

        public BonusTypeViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.ReligionList = new List<SelectListItem>();
            this.BonusTypeList = new List<BonusTypeViewModel>();
        }

        #endregion

        #region Standard Property

        public int Id { get; set; }

        [DisplayName("Bonus Type")]
        [Required]
        [StringLength(50)]
        public string BonusType { get; set; }
        public string BonusTypeB { get; set; }
        [DisplayName("Religion")]
        public int? ReligionId { get; set; }

        [DisplayName("Is Taxable")]
       // [Required]
        public Boolean IsTaxable { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        #endregion

        #region Others
        public string Mode { get; set; }

        public string Religion { get; set; }

        public IList<SelectListItem> ReligionList { get; set; }
        public IList<BonusTypeViewModel> BonusTypeList { get; set; }

        //public string ActionType { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }

        #endregion
    }
}