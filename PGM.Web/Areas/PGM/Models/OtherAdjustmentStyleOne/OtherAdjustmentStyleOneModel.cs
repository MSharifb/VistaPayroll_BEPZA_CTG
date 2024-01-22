using System;
using System.Collections.Generic;

using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Areas.PGM.Models.SalaryStructure;

namespace PGM.Web.Areas.PGM.Models.OtherAdjustmentStyleOne
{
    public class OtherAdjustmentStyleOneModel : BaseViewModel
    {
        public OtherAdjustmentStyleOneModel()
        {
            
            this.SalaryMonthList = new List<SelectListItem>();
            this.SalaryYearList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
            
            this.SalaryStructureDetail = new List<SalaryStructureDetailsModel>();

            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        #region Standard Property

        [Required]
        [DisplayName("Employee ID")]
        public int EmployeeId { get; set; }
       
        [DisplayName("Salary Month")]
        [StringLength(50)]
        public string SalaryMonth { get; set; }

        [DisplayName("Salary Year")]
        [StringLength(4)]
        public string SalaryYear { get; set; }


        [DisplayName("Remarks")]
        [StringLength(200)]
        public string Remarks { get; set; }

        [DisplayName("Override Structure Amount?")]
        public bool IsOverrideStructureAmount { get; set; }

        public int SalaryStructureId { get; set; }
        
        [DisplayName("Total Addition")]
        [UIHint("_ReadOnlyAmount")]
        public decimal TotalAddition { get; set; }

        [DisplayName("Total Deduction")]
        [UIHint("_ReadOnlyAmount")]
        public decimal TotalDeduction { get; set; }

        [DisplayName("Net Pay")]
        [UIHint("_ReadOnlyAmount")]
        public decimal NetPay { get; set; }


        #endregion

        #region Others
        
        public IList<SelectListItem> SalaryYearList { get; set; }
        public IList<SelectListItem> SalaryMonthList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }
        
        public ICollection<SalaryStructureDetailsModel> SalaryStructureDetail { get; set; }

        #region These are supporting properties. All selecting employees are stored at OtherAdjustDeductEmployeesModel collection
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; } 
        #endregion

        #endregion

        public bool LockEmpDDL { get; set; }
        public bool LockYearMonth { get; set; }
    }
}
