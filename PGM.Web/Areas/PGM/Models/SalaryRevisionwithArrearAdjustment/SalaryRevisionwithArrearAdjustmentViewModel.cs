using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.SalaryRevisionwithArrearAdjustment
{
    public class SalaryRevisionwithArrearAdjustmentViewModel : BaseViewModel
    {
        #region Ctor
        public SalaryRevisionwithArrearAdjustmentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.strMode = "Create";
            this.MonthList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            //this.SuspensionOfEmployeeDetailList = new List<SuspensionOfEmployeeViewModel>();
            //this.SuspensionOfEmployeeDeductionList = new List<SuspensionOfEmployeeDetailViewModel>();
        }
        #endregion

        #region Standard

        public int EmployeeId { get; set; }
        public int SalaryStructureId { get; set; }
        public int SalaryScaleId { get; set; }
        public int GradeId { get; set; }
        public int StepId { get; set; }
        public decimal GrossSalary { get; set; }
        public bool isConsolidated { get; set; }
        [Display(Name = "Order Date")]
        [UIHint("_RequiredDate")]
        public DateTime? OrderDate { get; set; }

        public decimal TotalAddition { get; set; }
        [Display(Name = "Total Deduction")]
        public decimal TotalDeduction { get; set; }
        [Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }
        public string Status { get; set; }

        [Display(Name = "Adjustment Month")]
        public string Month { get; set; }
        [Display(Name = "Adjustment Year")]
        public string Year { get; set; }
        public string Remarks { get; set; }
        public int ZoneInfoId { get; set; }
        #endregion

        #region Other

        [Display(Name = "Employee ID")]
        public string EmpId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }

        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> YearList { get; set; }

        //public List<SuspensionOfEmployeeViewModel> SuspensionOfEmployeeDetailList { get; set; }
        //public ICollection<SuspensionOfEmployeeDetailViewModel> SuspensionOfEmployeeDeductionList { get; set; }

        #endregion

        #region Salary Payments
        public int HeadId { get; set; }
        public string HeadType { get; set; }
        public string SalaryHead { get; set; }
        public decimal Amount { get; set; }
        public decimal ActualAmount { get; set; }
        public string AmountType { get; set; }
        public bool IsTaxable { get; set; }
        #endregion

    }
}