using System;
using System.Collections.Generic;

using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.WithheldSalaryPayment
{
    public class WithheldSalaryPaymentViewModel : BaseViewModel
    {
        public WithheldSalaryPaymentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.PaymentStatusList = new List<SelectListItem>();
            this.Mode = CrudeAction.Create;
        }

        #region Standard Properties

        [DataColumn(true)]
        public Int64 SalaryId { get; set; }

        [DataColumn(true)]
        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [DataColumn(true)]
        public string EmpID { get; set; }

        [DisplayName("Month")]
        [Required, DataColumn(true)]
        public string SalaryMonth { get; set; }

        [DisplayName("Year")]
        [Required, DataColumn(true)]
        public string SalaryYear { get; set; }

        [DisplayName("Payment Date")]
        [Required, DataColumn(true)]
        [UIHint("_Date")]
        public DateTime? PaymentDate { get; set; }

        [DisplayName("Amount")]
        [Required, DataColumn(true)]
        public decimal Payable { get; set; }

        [DisplayName("Employee Name")]
        [Required, DataColumn(true)]
        public string FullName { get; set; }

        [DisplayName("Initial")]
        [DataColumn(true)]
        public string EmployeeInitial { get; set; }

        [DisplayName("Account No.")]
        public string AccountNo { get; set; }

        public int BankId { get; set; }
        public int BranchId { get; set; }

        public bool IsPaid { get; set; }

        [DisplayName("Bank and Branch")]
        public string BankNBranch { get; set; }

        [DataColumn(true)]
        public string Remarks { get; set; }

        #endregion

        #region Other properties

        public string Mode { get; set; }

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> PaymentStatusList { get; set; }
        public IList<WithheldSalaryPaymentViewModel> PaymentList { get; set; }


        #endregion
    }
}