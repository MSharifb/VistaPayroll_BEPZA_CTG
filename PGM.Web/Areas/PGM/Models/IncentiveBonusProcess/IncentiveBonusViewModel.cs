using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.IncentiveBonusProcess
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
            CalenderYearList = new List<SelectListItem>();
            SalaryHeadList = new List<SelectListItem>();

            AmountTypeList = new List<SelectListItem>();
            DepartmentList = new List<SelectListItem>();
            StuffCategoryList = new List<SelectListItem>();
            EmploymentTypeList = new List<SelectListItem>();

            BonusTypeList = new List<SelectListItem>();

            InfoTypeList = new List<string>();

            IncentiveBonusDetailViewModelList = new List<IncentiveBonusDetailViewModel>();

            this.Mode = CrudeAction.Create;
        }

        #region Standard properties

        public int Id { get; set; }

        [DisplayName("Financial Year")]        
        public string FinancialYear { get; set; }

        [DisplayName("Incentive Bonus Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime IncentiveBonusDate { get; set; }

        [DisplayName("Bonus Type")]
        [Required]
        public int BonusTypeId { get; set; }

        [DisplayName("Order Date")]
        [UIHint("_Date")]
        public DateTime? OrderDate { get; set; }

        [DisplayName("Order Ref. No.")]
        public String OrderRefNo { get; set; }

        [DisplayName("Remark")]
        public String Remark { get; set; }

        [DisplayName("Formula Select")]
        [Required]
        public String FormulaSelect { get; set; }

        [DisplayName("Financial Year")]
        public String FinancialYearForFormula { get; set; }

        [DisplayName("Basic Salary Calulation From")]
        [UIHint("_Date")]
        public DateTime? BasicSalaryCalFromFinancialYear { get; set; }

        [DisplayName("Incentive Bonus Day")]
        [UIHint("_OnlyCurrency")]
        [Required]
        public int IncentiveBonusDay { get; set; }

        [DisplayName("Incentive Bonus Day")]
        public int ?IncentiveBonusDayF1 { get; set; }
        public int ?IncentiveBonusDayF2 { get; set; }
        public int ?IncentiveBonusDayF3 { get; set; }

        //--
        [DisplayName("Day of Month")]
        [UIHint("_OnlyCurrency")]
        public int? DayOfMonth { get; set; }
        public int? DayOfMonthF1 { get; set; }
        public int? DayOfMonthF3 { get; set; }
        //

        [DisplayName("Total Num of Month")]
        [UIHint("_OnlyCurrency")]
        public int? TotalNumOfMonth { get; set; }

        //
        [DisplayName("Day of Year")]
        [UIHint("_OnlyCurrency")]
        public int? DayOfYear { get; set; }
        public int? DayOfYearF2 { get; set; }
        public int? DayOfYearF3 { get; set; }
        //

        [DisplayName("Calender Year")]
        [UIHint("_OnlyCurrency")]
        public int ? CalenderYearForFormula { get; set; }

        [DisplayName("Basic Salary Calulation From")]
        [UIHint("_Date")]
        public DateTime? BasicSalaryCalFromCalenderYear { get; set; }

        [DisplayName("Less Entitled Earn Leave Days of the Year")]
        [Description("Less Entitled Earn Leave Days of the Year")]
        [UIHint("_OnlyCurrency")]
        public int? LEELDOY { get; set; }

        [DisplayName("Total Days of the Year without Entitled Leave Days")]
        [Description("Total Days of the Year without Entitled Leave Days")]
        [UIHint("_OnlyCurrency")]
        public int? TDoYWELD { get; set; }

        [DisplayName("Formula Factor")]
        public Decimal FormulaFactor { get; set; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; }

        [DisplayName("Staff Category")]
        public int? StaffCategoryId { get; set; }

        [DisplayName("Employment Type")]
        public int? EmploymentTypeId { get; set; }

        [DisplayName("Is All")]
        [Required]
        public bool IsAll { get; set; }

        [DisplayName("Employee Id")]
        public int EmployeeId { get; set; }

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

        [DisplayName("Employee ID")]
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
        public IList<SelectListItem> CalenderYearList { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        public IList<SelectListItem> AmountTypeList { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }
        public IList<SelectListItem> StuffCategoryList { get; set; }
        public IList<SelectListItem> EmploymentTypeList { get; set; }
        public IList<string> InfoTypeList { get; set; }
        public IList<SelectListItem> BonusTypeList { get; set; }

        #endregion

        public IList<IncentiveBonusDetailViewModel> IncentiveBonusDetailViewModelList { get; set; }
    }

    #region Bonus Details Properties

    public class IncentiveBonusDetailViewModel
    {
        public IncentiveBonusDetailViewModel()
        {

        }

        #region Standard Details Properties

        public Int64 Id { get; set; }

        public int IncentiveBonusId { get; set; }

        public int EmployeeId { get; set; }

        [DisplayName("Basic Salary")]
        [UIHint("_OnlyCurrency")]
        public Decimal BasicSalary { get; set; }

        [DisplayName("Incentive Bonus Amount")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal IncentiveBonusAmount { get; set; }

        [DisplayName("R/S")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal RevenueStamp { get; set; }

        [DisplayName("Net Payable")]
        [UIHint("_OnlyCurrency")]
        public Decimal NetPayable { get; set; }
        
        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        public string FullName { get; set; }

        #endregion

        #region Other Details Properties
        public int IsError { set; get; }
        public string ErrMsg { set; get; }

        #endregion
    }

    #endregion
}