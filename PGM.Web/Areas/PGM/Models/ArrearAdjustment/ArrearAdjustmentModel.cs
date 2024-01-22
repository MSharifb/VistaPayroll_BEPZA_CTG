using PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Areas.PGM.Models.SalaryStructure;


namespace PGM.Web.Areas.PGM.Models.ArrearAdjustment
{
    public class ArrearAdjustmentModel : BaseViewModel
    {
        public ArrearAdjustmentModel()
        {
            YearList = new List<SelectListItem>();
            MonthList = new List<SelectListItem>();

            ArrearAdjustmentDetailModelList = new List<ArrearAdjustmentDetailModel>();
            ArrearAdjustmentList = new List<SelectListItem>();
            ArrearAdjustmentViewModelList = new List<ArrearAdjustmentListViewModel>();
        }

        public int EmpStatusChangeId { get; set; }

        [DisplayName("Employee ID")]
        public string EmpID { get; set; }

        //
        [Required]
        public int EmployeeId { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }
        //

        [Required]
        public int DesignationId { get; set; }

        //
        [DisplayName("Designation Name")]
        public string DesignationName { get; set; }

        //
        [DisplayName("Order Date")]
        [UIHint("_Date")]
        public DateTime? OrderDate { get; set; }

        [DisplayName("Adjustment Type")]
        public string AdjustmentType { get; set; }

        //
        [DisplayName("Payment Date")]
        [UIHint("_Date")]
        public DateTime? PaymentDate { get; set; }

        //
        [DisplayName("Arrear From Date")]
        [UIHint("_Date")]
        public DateTime? ArrearFromDate { get; set; }

        //
        [DisplayName("Arrear To Date")]
        [UIHint("_Date")]
        public DateTime? ArrearToDate { get; set; }

        //
        [DisplayName("Adjustment Year")]
        public string AdjustmentYear { get; set; }

        //
        [DisplayName("Adjustment Month")]
        public string AdjustmentMonth { get; set; }

        //
        [DisplayName("Is Adjust with Salary")]
        public bool IsAdjustWithSalary { get; set; }

        //
        [DisplayName("Is Adjustment Paid")]
        public bool IsAdjustmentPaid { get; set; }

        //
        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        public int ZoneInfoId { get; set; }

        [DisplayName("Addition Arrear")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? TotalAddition { get; set; }

        [DisplayName("Deduction Arrear")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? TotalDeduction { get; set; }

        [DisplayName("Net Arrear")]
        [UIHint("_ReadOnlyAmount")]
        public decimal? NetArrear { get; set; }

        public DateTime? EffectiveDate { get; set; }

        #region Others

        public bool Select { get; set; }

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }

        public List<ArrearAdjustmentDetailModel> ArrearAdjustmentDetailModelList { get; set; }
        public virtual ICollection<SalaryStructureDetailsModel> SalaryStructureDetail { get; set; }

        public int ArrearAdjustmentId { get; set; }
        public IList<SelectListItem> ArrearAdjustmentList { get; set; }

        public List<ArrearAdjustmentListViewModel> ArrearAdjustmentViewModelList { get; set; }

        #endregion
    }
}