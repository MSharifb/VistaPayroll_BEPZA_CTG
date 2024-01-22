using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.OtherAdjustment
{
    public class OtherAdjustDeductEmployeesModel : BaseViewModel
    {
        public OtherAdjustDeductEmployeesModel()
        {
            this.AdjustmentTypeList = new List<SelectListItem>();
            this.SalaryHeadList = new List<SelectListItem>();
        }

        #region standard

        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        [Required]
        public string EmpID { get; set; }

        [DisplayName("Employee Name")]
        [UIHint("_ReadOnly")]
        [Required]
        public string EmployeeName { get; set; }

        [DisplayName("Employee Designation")]
        [UIHint("_ReadOnly")]
        [Required]
        public string EmployeeDesignation { get; set; }
        
        [Required]
        public string Type { get; set; }
        
        [Required]
        public int SalaryHeadId { get; set; }
        public String SalaryHeadName { get; set; }

        [Required]
        [UIHint("_OnlyNumber")]
        [Range(0, 99999999, ErrorMessage = "Amount must be greater or equal zero.")]
        public decimal Amount { get; set; }
        
        public bool IsOverrideStructureAmount { get; set; }

        #endregion

        #region Others
        public IList<SelectListItem> AdjustmentTypeList { get; set; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        #endregion
    }
}