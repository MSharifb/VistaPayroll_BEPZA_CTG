using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.GroupInsurancePayment
{
    public class GroupInsurancePaymentViewModel : BaseViewModel
    {
        public GroupInsurancePaymentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.Mode = CrudeAction.Create;
        }

        #region Standard Properties
        [Required]
        [DisplayName("Payment Type")]
        public int PaymentTypeId { get; set; }
        [Required]
        [DisplayName("Employee Id")]
        public int EmployeeId { get; set; }
        [Required]
        [DisplayName("Order No.")]
        public string OrderNo { get; set; }
        [Required]
        [DisplayName("Payment Date")]
        [UIHint("_Date")]
        public DateTime PaymentDate { get; set; }
        [DisplayName("Pay To")]
        public string PayTo { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Relation")]
        public string Relation { get; set; }
        #endregion 

        #region Others
        public string Mode { get; set; }
        public string EmployeeName { get; set; }
        public string ICNo { get; set; }
        public string Designation { get; set; }
        public string PaymentType { get; set; }
        public decimal PaymentAmount { get; set; }
        public IEnumerable<SelectListItem> PaymentTypeList { get; set; }
        #endregion 
    }
}