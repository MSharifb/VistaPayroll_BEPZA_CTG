using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.BankAccount
{

    public class BankAccountModel : BaseViewModel
    {
        #region Ctor

        public BankAccountModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.Mode = CrudeAction.Create;

            this.BankAccountNoList = new List<SelectListItem>();
            this.BankNameList = new List<SelectListItem>();
            this.AccountForList = new List<SelectListItem>();

            this.BankAccountList = new List<BankAccountModel>();
        }

        #endregion

        #region Standard Property

        [DisplayName("Bank Name")]
        [Required]
        public int BankId { get; set; }

        [DisplayName("Account No.")]
        [Required]
        public int BankAccountId { get; set; }

        [DisplayName("Branch Name")]
        [UIHint("_ReadOnly")]
        public string BranchName { get; set; }

        [DisplayName("Bank Address")]
        [UIHint("_ReadOnly")]
        public string BankAddress { get; set; }

        [DisplayName("Account For")]
        [Required]
        public string AccountFor { get; set; }

        public int ZoneInfoId { get; set; }

        #endregion

        #region Others

        public string Mode { get; set; }

        public string BankName { get; set; }
        public string BankAccountNo { get; set; }

        public IList<SelectListItem> BankNameList { get; set; }
        public IList<SelectListItem> BankAccountNoList { get; set; }
        public IList<SelectListItem> AccountForList { get; set; }

        public IList<BankAccountModel> BankAccountList { get; set; }

        #endregion
    }
}