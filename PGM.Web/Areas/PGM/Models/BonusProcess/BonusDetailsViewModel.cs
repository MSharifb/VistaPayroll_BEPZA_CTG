using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.BonusProcess
{
    public class BonusDetailsViewModel:BaseViewModel
    {
        public BonusDetailsViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.DivisionList = new List<SelectListItem>();
        }

        #region Standard Details Properties

        public int BonusId { get; set; }

        public int DivisionId { get; set; }

        public int DesignationId { get; set; }

        public int GradeId { get; set; }

        public int BankId { get; set; }

        public int BranchId { get; set; }

        public int BonusTypeId { get; set; }

        [DisplayName("Month")]
        [Required]
        public string BonusMonth { get; set; }

        [DisplayName("Year")]
        [Required]
        public string BonusYear { get; set; }

        [DisplayName("Bonus Type")]
        public string BonusType { get; set; }

        [DisplayName("Account No.")]
        public string AccountNo { get; set; }

        [DisplayName("Basic")]
        [UIHint("_OnlyCurrency")]
        public Decimal EmpBasicSalary { get; set; }

        [DisplayName("Bonus Amount")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal EmpBonusAmount { get; set; }

        [DisplayName("R/S")]
        [UIHint("_OnlyCurrency")]
        public Decimal EmpRevenueStamp { get; set; }

        [DisplayName("Net Payable")]
        [UIHint("_OnlyCurrency")]
        public Decimal EmpNetPayable { get; set; }

        public int EmployeeId { get; set; }

        public string EmpID { get; set; }

        [DisplayName("Name")]
        public string FullName { get; set; }

        #endregion

        #region Other Details Properties
        public IList<SelectListItem> DivisionList { get; set; }
        #endregion
    }
}