using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.SalaryAutoIncrement
{
    public class SalaryAutoIncrementViewModel
    {
        public SalaryAutoIncrementViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
        }

        public int Id { get; set; }
        public int FinancialYearId { get; set; }
        public int EmployeeId { get; set; }
        public int SalaryHeadId { get; set; }

        [UIHint("_Date")]
        [DisplayName("Process Date")]
        public DateTime? ProcessDate { get; set; }
        #region Other properties

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string Message { get; set; }

        #endregion
    }
}