using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Utility;


namespace PGM.Web.Areas.PGM.Models.GratuitySettlement
{
    public class GratuitySettlementViewModel : BaseViewModel
    {
        public GratuitySettlementViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.PaymentStatusList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();

            this.Mode = CrudeAction.Create;
        }

        #region Standard Properties


        public int EmployeeId { get; set; }

        [DisplayName("Designation")]
        [UIHint("_ReadOnly")]
        public string Designation { get; set; }

        [DisplayName("Date of Joining")]
        [UIHint("_ReadOnlyDate")]
        [Required]
        public DateTime DateofJoining { get; set; }

        [DisplayName("Date of Confirmation")]
        [UIHint("_ReadOnlyDate")]
        [Required]
        public DateTime? DateofConfirmation { get; set; }

        [DisplayName("Date of Seperation")]
        [UIHint("_ReadOnlyDate")]
        [Required]
        public DateTime? DateofSeperation { get; set; }
        
        [DisplayName("Service Length")]
        [UIHint("_ReadOnly")]
        public String ServiceLength { get; set; }
        

        [DisplayName("Date of Payment")]
        [UIHint("_Date")]
        [Required]
        public DateTime DateofPayment { get; set; }
        
        [DisplayName("Gratuity Gross Salary")]
        [Required]
        [UIHint("_OnlyCurrency")]
        [Range(1, 99999999, ErrorMessage = "Basic Salary must be greater than zero.")]
        public decimal GratuityGrossSalary { get; set; }

        [DisplayName("Payable Amount")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public decimal PayableAmount { get; set; }

        [DisplayName("Paid")]
        public bool isPaid { get; set; }

        [DisplayName("Paid With Final Settlement")]
        public bool isPaidWithFinalSettlement { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        #endregion


        #region Other properties

        public string Mode { get; set; }

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> PaymentStatusList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }

        #endregion
    }
}