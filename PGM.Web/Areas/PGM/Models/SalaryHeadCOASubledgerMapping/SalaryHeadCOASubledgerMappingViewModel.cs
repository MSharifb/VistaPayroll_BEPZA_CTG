using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.SalaryHeadCOASubledgerMapping
{
    public class SalaryHeadCOASubledgerMappingViewModel:BaseViewModel
    {
        public SalaryHeadCOASubledgerMappingViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.COAList = new List<SelectListItem>();
            this.SubledgerList = new List<SelectListItem>();
            this.SalaryHeadList = new List<SelectListItem>();
        }

        #region Standard Properties

        [DisplayName("Salary Head")]
        [Required]
        public int SalaryHeadId { get; set; }
        public int ZoneInfoId { get; set; }
        [DisplayName("Account Head")]
        public int? COAId { get; set; }
        [DisplayName("Sub Ledger")]
        public int? SubledgerId { get; set; }
        [DisplayName("Is Multiple Employee Configuration")]
        public bool IsMultiEmpConfig { get; set; }

        public IList<SelectListItem> COAList { get; set; }
        public IList<SelectListItem> SubledgerList { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        #endregion

        #region Other properties
        public string Mode { get; set; }
        public string SalaryHead { get; set; }
        public string COA { get; set; }
        public string SubLedger { get; set; }
        #endregion

    }
}