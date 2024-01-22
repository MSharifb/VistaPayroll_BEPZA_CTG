using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.NightBill
{
    public class NightBillPaymentViewModel : BaseViewModel
    {
        public NightBillPaymentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
            this.Mode = CrudeAction.Create;
            this.NightBillPaymentDetail = new List<NightBillPaymentDetailViewModel>();
        }

        #region Standard Property

        [Required]
        [DisplayName("Month")]
        public string BillMonth { get; set; }
        [Required]
        [DisplayName("Year")]
        public Int32 BillYear { get; set; }
        [Required]
        [UIHint("_Date")]
        [DisplayName("Payment Date")]
        public DateTime PaymentDate { get; set; }
        [Required]
        [DisplayName("Department")]
        public Int32 DepartmentId { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        public List<NightBillPaymentDetailViewModel> NightBillPaymentDetail { get; set; }
        #endregion 

        #region Others Property
        public string Mode { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }
        public IList<SelectListItem> YearList { get; set; }
        public string DepartmentName { get; set; }

        #endregion 
    }

    public class NightBillPaymentDetailViewModel : BaseViewModel
    {
        public NightBillPaymentDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }

        #region Standard Property 
        public Int32 BillId { get; set; }
        public Int32 EmployeeId { get; set; }
        public decimal TotalDays { get; set; }
        [UIHint("_OnlyCurrency")]
        public decimal PerDayRate { get; set; }
        [UIHint("_OnlyCurrency")]
        public decimal RevenueStamp { get; set; }
        [UIHint("_OnlyCurrency")]
        public decimal TotalAmount { get; set; }
        [UIHint("_OnlyCurrency")]
        public decimal NetAmount { get; set; }
        
        #endregion 

        #region Others
        public string ICNo { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        #endregion 
    }

}