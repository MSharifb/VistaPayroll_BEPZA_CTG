using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;

namespace BOM_MPA.Web.Areas.PGM.Models.IncentiveBonusProcess
{
    public class IncentiveBonusViewModel
    {
        public IncentiveBonusViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            EUser = this.IUser;

            IDate = DateTime.UtcNow;
            EDate = this.IDate;

            FinancialYearList = new List<SelectListItem>();
            SalaryHeadList = new List<SelectListItem>();

            AmountTypeList = new List<SelectListItem>();
            DepartmentList = new List<SelectListItem>();
            StuffCategoryList = new List<SelectListItem>();
            EmploymentTypeList = new List<SelectListItem>();

            InfoTypeList = new List<string>();

            this.Mode = "Create";
        }

        #region Standard properties

        public int Id { get; set; }

        [DisplayName("Financial Year")]
        [Required]
        public int FinancialYearId { get; set; }

        [DisplayName("Incentive Bonus Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime IncentiveBonusDate { get; set; }

        [DisplayName("Salary Head (+)")]
        [Required]
        public int SalaryHeadId { get; set; }

        [DisplayName("Order Date")]
        [UIHint("_Date")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Order Ref. No.")]
        public String OrderRefNo { get; set; }

        [DisplayName("Remark")]
        public String Remark { get; set; }

        [DisplayName("Formula Select")]
        public String FormulaSelect { get; set; }

        [DisplayName("Financial Year For F1")]
        public String FinancialYearForF1 { get; set; }

        [DisplayName("Basic Salary Calulation From")]
        [UIHint("_Date")]
        public DateTime BasicSalaryCalulationFrom { get; set; }

        [DisplayName("Incentive Bonus Day")]
        [Required]
        public int IncentiveBonusDay { get; set; }

        [DisplayName("Day of Month")]
        public int DayOfMonth { get; set; }

        [DisplayName("Total Num of Month")]
        public int TotalNumOfMonth { get; set; }

        [DisplayName("Day of Year")]
        public int DayOfYear { get; set; }

        [DisplayName("Calender Year")]
        public int CalenderYear { get; set; }

        [DisplayName("Less Entitled Earn Leave Days of the Year")]
        [Description("Less Entitled Earn Leave Days of the Year")]
        public int LEELDOY { get; set; }

        [DisplayName("Total Days of the Year without Entitled Leave Days")]
        [Description("Total Days of the Year without Entitled Leave Days")]
        public int TDoYWELD { get; set; }

        [DisplayName("Formula Factor")]
        public Decimal FormulaFactor { get; set; }

        [DisplayName("Department")]
        public int DepartmentId { get; set; }

        [DisplayName("Staff Category")]
        public int StaffCategoryId { get; set; }

        [DisplayName("Employment Type")]
        public int EmploymentTypeId { get; set; }

        [DisplayName("Is All")]
        public int IsAll { get; set; }

        [DisplayName("Employee Id")]
        public int EmployeeId { get; set; }

        [DisplayName("Amount Type")]
        [Required]
        public string AmountType { get; set; }

        [DisplayName("Revenue Stamp(R/S)")]
        [UIHint("_OnlyCurrency")]
        public Decimal RevenueStamp { get; set; }

        [DisplayName("Information Type")]
        [Description("All, Individual")]
        public string InfoType { get; set; }

        #endregion

        #region Other Properties

        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        public string EmployeeName { get; set; }

        [DisplayName("I.C. No.")]
        [UIHint("_ReadOnly")]
        public string EmpID { get; set; }

        [DisplayName("Designation")]
        [UIHint("_ReadOnly")]
        public string DesignationName { get; set; }

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        public IList<SelectListItem> FinancialYearList { get; set; }
        public List<SelectListItem> SalaryHeadList { get; set; }
        public IList<SelectListItem> AmountTypeList { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }
        public IList<SelectListItem> StuffCategoryList { get; set; }
        public IList<SelectListItem> EmploymentTypeList { get; set; }
        public IList<string> InfoTypeList { get; set; }

        #endregion

       
    }

    #region Bonus Details Properties

    public class IncentiveBonusDetailViewModel
    {
        public IncentiveBonusDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.DivisionList = new List<SelectListItem>();
        }

        #region Standard Details Properties

        public Int64 Id { get; set; }

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

        //[Required]
        //public int EmpProjectId { get; set; }

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
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal EmpRevenueStamp { get; set; }

        [DisplayName("Net Payable")]
        [UIHint("_OnlyCurrency")]
        public Decimal EmpNetPayable { get; set; }

        public int EmployeeId { get; set; }

        public string EmpID { get; set; }

        [DisplayName("Name")]
        public string FullName { get; set; }

        //[DisplayName("Initial")]
        //public string EmployeeInitial { get; set; }

        #endregion

        #region Other Details Properties
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }
        public IList<SelectListItem> DivisionList { get; set; }
        #endregion
    }
    #endregion
}
