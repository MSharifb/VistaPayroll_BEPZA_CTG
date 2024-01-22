using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.ChargeAllowanceRule
{
    public class ChargeAllowanceRuleModel
    {
        #region Ctor

        public ChargeAllowanceRuleModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.EUser = this.IUser;
            this.EDate = this.IDate;

            this.SalaryHeadList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

        public int Id { get; set; }

        [DisplayName("Salary Head")]
        [Required]
        public int SalaryHeadId { get; set; }

        [DisplayName("Base On")]
        [Required]
        public string BaseOn { get; set; }

        [DisplayName("Charge Allowance Percent")]
        [Required]
        public decimal ChargeAllowancePercent { get; set; }

        [DisplayName("Max Charge Allowance")]
        [Required]
        public decimal MaxChargeAllowance { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public DateTime IDate { get; set; }
        public DateTime EDate { get; set; }

        #endregion

        #region Others
        public string Mode { get; set; }

        public IList<SelectListItem> SalaryHeadList { get; set; }
        public IList<SelectListItem> BaseOnList { get; set; }
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public bool HasData { get; set; }
        #endregion
    }
}