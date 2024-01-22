using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.MonthlySalaryInfo
{
    public class MonthlySalaryViewModel
    {
        public MonthlySalaryViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
         
            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.DivisionList = new List<SelectListItem>();
            this.EmployeeTypeList = new List<SelectListItem>();
            this.WithheldStatusList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
            this.Mode = CrudeAction.Create;
        }
       
        public Int64 SalaryId { get; set; }

        [DisplayName("Employee ID")]     
        public int EmployeeId { get; set; }
        
        public string EmpID { get; set; }

        public int DivisionId { get; set; }
        public int EmploymentTypeId { get; set; }

        [DisplayName("Employee Initial")]   
        public string EmployeeInitial { get; set; }

        public bool? IsWithheld { get; set; }

        [DisplayName("Month")]
        [Required]
        public string SalaryMonth { get; set; }

        [DisplayName("Year")]
        [Required]
        public string SalaryYear { get; set; }


        #region Other properties


        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string Message { get; set; }
        
        public Boolean IsConfirmed { get; set; }

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> DivisionList { get; set; }
        public IList<SelectListItem> EmployeeTypeList { get; set; }
        public IList<SelectListItem> WithheldStatusList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }
        public Boolean ProcessTaxWithSalary { get; set; }

        [DisplayName("Recommender/Approver")]
        public int? ApproverId { get; set; }
        public IList<SelectListItem> ApproverList { get; set; }

        [DisplayName("Recommender/Approver for CPF")]
        public int? CPFApproverId { get; set; }
        public IList<SelectListItem> CPFApproverList { get; set; }

        #endregion
    }
}