using System;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.ArrearAdjustment
{
    public class ArrearAdjustmentListViewModel : BaseViewModel
    {
        public ArrearAdjustmentListViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.Now;
        }

        public DateTime EffectiveDate { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime ArrearFromDate { get; set; }

        public DateTime ArrearToDate { get; set; }

        public string ArrearType { get; set; }

        public bool IsSelected { get; set; }

        public string PaymentStatus { get; set; }

        public bool IsApproved { get; set; }

        public string ApprovalStatus { get; set; }
    }
}