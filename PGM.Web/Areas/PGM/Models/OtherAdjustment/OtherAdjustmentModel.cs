using System;
using System.Collections.Generic;

using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.OtherAdjustment
{
    public class OtherAdjustmentModel : BaseViewModel
    {
        public OtherAdjustmentModel()
        {
            
            this.SalaryMonthList = new List<SelectListItem>();
            this.SalaryYearList = new List<SelectListItem>();
            this.AdjustmentTypeList = new List<SelectListItem>();
            this.SalaryHeadList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
            this.OtherEmployeeModel = new List<OtherAdjustDeductEmployeesModel>();

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
        
        [DisplayName("Adjustment Type")]
        public string HeadType { get; set; }
        public IList<SelectListItem> AdjustmentTypeList { get; set; }
        
        public int SalaryHeadId { get; set; }
        [DisplayName("Salary Head")]
        public String SalaryHead { get; set; }

        [DisplayName("Amount")]
        [UIHint("_OnlyNumber")]
        [Range(-99999999, 99999999, ErrorMessage = "Amount must be in between -99999999 and 99999999.")]
        public Decimal Amount { get; set; }

        public String AmountType { get; set; }

        [DisplayName("Remarks")]
        [StringLength(200)]
        public string Remarks { get; set; }

        [DisplayName("Override Structure Amount?")]
        public bool IsOverrideStructureAmount { get; set; }

        #endregion

        #region Others
        
        public IList<SelectListItem> SalaryHeadList { get; set; }
        public IList<SelectListItem> SalaryYearList { get; set; }
        public IList<SelectListItem> SalaryMonthList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }
        public IList<OtherAdjustDeductEmployeesModel> OtherEmployeeModel { get; set; } // this variable name is using in _PartialDetail.cshtml as a magic string.

        #region These are supporting properties. All selecting employees are stored at OtherAdjustDeductEmployeesModel collection
        public string EmpID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesignation { get; set; } 
        #endregion

        [Required]
        public string FromYear { get; set; }

        [Required]
        public string ToYear { get; set; }

        [Required]
        [DisplayName("From Month")]
        public string FromMonth { get; set; }

        [Required]
        [DisplayName("To Month")]
        public string ToMonth { get; set; }

        public DateTime FromDate
        {
            get
            {
                int intYear = Convert.ToInt32(FromYear);
                int intMonth = Convert.ToInt32(FromMonth);
                if (intYear != 0 && intMonth != 0)
                    return new DateTime(intYear, intMonth, 1);
                else
                    return DateTime.MinValue;
            }
        }

        public DateTime ToDate
        {
            get
            {
                int intYear = Convert.ToInt32(ToYear);
                int intMonth = Convert.ToInt32(ToMonth);
                if (intYear != 0 && intMonth != 0)
                    return new DateTime(intYear, intMonth, 1);
                else
                    return DateTime.MinValue;
            }
        }

        #endregion
    }
}
