using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.OtherAdjustmentDeductionAll
{
    public class OtherAdjustmentDeductAll : BaseViewModel
    {
        public OtherAdjustmentDeductAll()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.SalaryHeadList = new List<SelectListItem>();
            this.SalaryMonthList = new List<SelectListItem>();
            this.SalaryYearList = new List<SelectListItem>();
            this.EmploymentTypeList = new List<SelectListItem>();
            this.AmountTypeList = new List<SelectListItem>();


            this.StaffCategoryList = new List<SelectListItem>();
            this.DepartmentList = new List<SelectListItem>();
            
            this.JobGradeList = new List<SelectListItem>();
            this.DesignationList = new List<SelectListItem>();
        }

        #region Standard Property
        
        [DisplayName("Salary Year")]
        [StringLength(4)]
        public string SalaryYear { get; set; }

        [DisplayName("Salary Month")]
        [StringLength(50)]
        public string SalaryMonth { get; set; }
        
        [Required]
        public string HeadType { get; set; }
        public string SelectedType { get; set; }

        [Required]
        public int SalaryHeadId { get; set; }

        [DisplayName("Salary Head")]
        public String SalaryHead { get; set; }

        public int ZoneInfoId { get; set; }

        [DisplayName("Designation")]
        public int? DesignationId { get; set; }
        public string Designation { get; set; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; }

        [DisplayName("Employment Type")]
        public int? EmploymentTypeId { get; set; }
        public string EmploymentType { get; set; }

        [DisplayName("From Job Grade")]
        public int? FromJobGradeId { get; set; }

        [DisplayName("To Job Grade")]
        public int? ToJobGradeId { get; set; }

        [DisplayName("Staff Category")]
        public int? StaffCategoryId { get; set; }

        [DisplayName("Override Structure Amount?")]
        public bool IsOverrideStructureAmount { get; set; }

        [DisplayName("Amount Type")]
        [Required]
        public string AmountType { get; set; }

        public bool IsDayWise { get; set; }
        
        [DisplayName("Amount")]
        [Required]
        [UIHint("_OnlyNumber")]
        [Range(1, 99999999, ErrorMessage = "Amount must be greater than zero.")]
        public Decimal Amount { get; set; }
        
        [DisplayName("Remarks")]
        [StringLength(200)]
        public string Remarks { get; set; }
        

        #endregion

        #region Others
        public string Mode { get; set; }
        public string ErrClss { set; get; }
        public IList<SelectListItem> SalaryHeadList { get; set; }
        public IList<SelectListItem> SalaryMonthList { get; set; }
        public IList<SelectListItem> SalaryYearList { get; set; }
        public IList<SelectListItem> EmploymentTypeList { get; set; }
        public IList<SelectListItem> AmountTypeList { get; set; }

        public IList<SelectListItem> StaffCategoryList { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }
        public IList<SelectListItem> JobGradeList { get; set; }
        public IList<SelectListItem> DesignationList { get; set; }

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

        public string StaffCategory { get; set; }
        public string Department { get; set; }
        public string JobGrade { get; set; }

        #endregion
    }
}