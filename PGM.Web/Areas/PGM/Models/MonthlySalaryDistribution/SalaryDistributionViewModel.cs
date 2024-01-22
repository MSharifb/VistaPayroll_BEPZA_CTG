using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PGM.Web.Utility;


namespace PGM.Web.Areas.PGM.Models.MonthlySalaryDistribution
{
    public class SalaryDistributionViewModel
    {

        public SalaryDistributionViewModel() 
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
         
            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.DivisionList = new List<SelectListItem>();
            this.ProjectNoList = new List<SelectListItem>();

            this.Mode = CrudeAction.Create;
        }


        public Int64 SalaryId { get; set; }
       
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public int DivisionId { get; set; }
        public string EmployeeInitial { get; set; }

        [DisplayName("Employee ID")]       
        public string EmpID { get; set; }

        [DisplayName("Month")]
        [Required]
        public string SalaryMonth { get; set; }

        [DisplayName("Year")]
        [Required]
        public string SalaryYear { get; set; }

        public Decimal GrossSalary { get; set; }
        public Decimal TotalHours { get; set; }
        public Decimal RatePerHour { get; set; }

        #region Other properties


        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string Message { get; set; }
        


        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> DivisionList { get; set; }
        public IList<SelectListItem> ProjectNoList { get; set; } 
     

        #endregion
    }
}