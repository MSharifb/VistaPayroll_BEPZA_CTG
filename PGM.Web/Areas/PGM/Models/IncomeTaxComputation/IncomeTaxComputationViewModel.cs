using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.IncomeTaxComputation
{
    public class IncomeTaxComputationViewModel
    {

        public IncomeTaxComputationViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;

            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.IncomeYearList = new List<SelectListItem>();
           
            this.Mode = CrudeAction.Create;
        }

        #region  Standard Properties

        public Int64 Id { get; set; }
        public int TaxRuleId { get; set; }

        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string EmployeeInitial { get; set; }
        public string FullName { get; set; }

        public int DivisionId { get; set; }
        public int DesignationId { get; set; }
        public int EmploymentTypeId { get; set; }
       
        public string Gender { get; set; }
        public int AgeInYear { get; set; }
        public int ActualMonth { get; set; }
        public int RemainingMonth { get; set; }

        public Decimal TaxPayable { get; set; }
        public Decimal TaxDeducted { get; set; }
        public Decimal TaxDue { get; set; }
        public Decimal TaxPerMonth { get; set; }
        public Decimal InvestmentRebate { get; set; }
        public Decimal SpecialRebate { get; set; }
        public Decimal TaxableIncome { get; set; }
        public Decimal PFCompany { get; set; }
        public Decimal TaxLiability { get; set; }

        [DisplayName("Income Year")]
        [Required]
        public string IncomeYear { get; set; }

        [DisplayName("Assessment Year")]
        [Required]
        public string AssessmentYear { get; set; }

        [DisplayName("Salary Month")]
        [Required]
        public string SalaryMonth { get; set; }

        [DisplayName("Salary Year")]
        [Required]
        public string SalaryYear { get; set; }

        public string ReportPath { get; set; }


        #endregion

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
        public IList<SelectListItem> IncomeYearList { get; set; } 
       
        #endregion


    }
}