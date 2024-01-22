using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.ExcludeEmpFromReport
{
    public class ExcludeEmpFromReportModel : BaseViewModel
    {
        public ExcludeEmpFromReportModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.EmployeeList = new List<SelectListItem>();
            this.SalaryHeadList = new List<SelectListItem>();
        }

        #region Standard Properties

        [DisplayName("Salary Head")]
        [Required]
        public int SalaryHeadId { get; set; }
        public int ZoneInfoId { get; set; }
        [DisplayName("Employee")]
        public string Employee { get; set; }

        public IList<SelectListItem> EmployeeList { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        public IEnumerable<string> SelectedEmployees { get; set; }
        #endregion

        #region Other properties
        public string Mode { get; set; }
        public string SalaryHead { get; set; }
        public string COA { get; set; }
        public string SubLedger { get; set; }
        #endregion


    }
}