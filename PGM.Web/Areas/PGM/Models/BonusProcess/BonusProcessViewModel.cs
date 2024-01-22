using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.BonusProcess
{
    public class BonusProcessViewModel : BaseViewModel
    {

        public BonusProcessViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            EUser = this.IUser;
            IDate = DateTime.UtcNow;
            EDate = this.IDate;

            YearList = new List<SelectListItem>();
            MonthList = new List<SelectListItem>();
            BonusTypeList = new List<SelectListItem>();
            ReligionList = new List<SelectListItem>();
            AmountTypeList = new List<SelectListItem>();

            DepartmentList = new List<SelectListItem>();
            SectionList = new List<SelectListItem>();
            StuffCategoryList = new List<SelectListItem>();
            JobGradeList = new List<SelectListItem>();

            InfoTypeList = new List<string>();
            BasicCalculationMonthList = new List<SelectListItem>();
            EmployeeList = new List<SelectListItem>();

            this.Mode = CrudeAction.Create;
        }

        #region Standard properties
        
        [DisplayName("Month")]
        [Required]
        public string BonusMonth { get; set; }

        [DisplayName("Year")]
        [Required]
        public string BonusYear { get; set; }

        [DisplayName("Payment Date")]
        [UIHint("_Date")]
        [Required]
        public DateTime EffectiveDate { get; set; }

        [DisplayName("Bonus Type")]
        public string BonusType { get; set; }

        public int BonusTypeId { get; set; }

        [DisplayName("Religion")]
        public int? ReligionId { get; set; }

        public string Religion { get; set; }

        [DisplayName("Amount Type")]
        [Required]
        public string AmountType { get; set; }

        [DisplayName("Bonus Amount")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal BonusAmount { get; set; }

        [DisplayName("Revenue Stamp(R/S)")]
        [Required]
        [UIHint("_OnlyCurrency")]
        public Decimal RevenueStamp { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        public string SecurityType { get; set; }


        //
        [DisplayName("Department")]
        public int? DepartmentId { get; set; }

        [DisplayName("Section")]
        public int? SectionId { get; set; }

        [DisplayName("Officer / Staff Category")]
        public int? StaffCategoryId { get; set; }

        [DisplayName("Job Grade")]
        public int? JobGradeId { get; set; }

        [DisplayName("Employee Id")]
        public int EmployeeId { get; set; }

        [DisplayName("Is All")]
        public bool IsAll { get; set; }

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
        
        [DisplayName("Get Basic From")]
        [Required]
        public byte? BasicCalculationMonth { get; set; }

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> BonusTypeList { get; set; }
        public IList<SelectListItem> ReligionList { get; set; }
        public IList<SelectListItem> AmountTypeList { get; set; }
        public IList<SelectListItem> DepartmentList { get; set; }
        public IList<SelectListItem> SectionList { get; set; }
        public IList<SelectListItem> StuffCategoryList { get; set; }
        public IList<SelectListItem> JobGradeList { get; set; }
        public IList<string> InfoTypeList { get; set; }
        public IList<SelectListItem> BasicCalculationMonthList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }

        #endregion
    }
}
