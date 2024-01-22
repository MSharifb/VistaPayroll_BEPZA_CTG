using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGM.Web.Utility;


namespace PGM.Web.Areas.PGM.Models.HouseMaintenanceCharge
{
    public class HouseMaintenanceChargeViewModel : BaseViewModel
    {
        public HouseMaintenanceChargeViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = CrudeAction.Create;
            this.HouseMaintenanceDetail = new List<HouseMaintenanceChargeDetailViewModel>();
        }

        #region Standard Properties

        [Required]
        [UIHint("_Date")] 
        [DisplayName("Efective Date")]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [DisplayName("Salary Head")]
        public int SalaryHeadId { get; set; }
        public List<HouseMaintenanceChargeDetailViewModel> HouseMaintenanceDetail { get; set; }

        #endregion 

        #region Others
        public string SalaryHeadName { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        public string Mode { get; set; }
        #endregion 
    }

    public class HouseMaintenanceChargeDetailViewModel : BaseViewModel
    {
        public HouseMaintenanceChargeDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }

        #region Standard Property
        [Required]
        [DisplayName("House Maintenance")]
        public int HouseMaintenanceId { get; set; }
        [Required]
        [DisplayName("From Grade")]
        public int FromGradeId { get; set; }
        [Required]
        [DisplayName("To Grade")]
        public int ToGradeId { get; set; }
        [Required]
        [UIHint("_OnlyCurrency")]
        [DisplayName("% of Basic Salary")]
        public Decimal AmountPercent { get; set; }

        #endregion 

        #region Others
        public IList<SelectListItem> GradeList { get; set; }

        #endregion 
    }

}