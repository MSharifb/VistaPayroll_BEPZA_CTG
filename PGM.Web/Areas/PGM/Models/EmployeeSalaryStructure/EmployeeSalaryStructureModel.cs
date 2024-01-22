using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PGM.Web.Areas.PGM.Models.SalaryStructure;

namespace PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure
{
    public class EmployeeSalaryStructureModel : BaseViewModel
    {
        #region Ctor

        public EmployeeSalaryStructureModel()
        {
            this.SalaryStructureDetail = new List<SalaryStructureDetailsModel>();
            this.SalaryScaleList = new List<SelectListItem>();
            this.GradeList = new List<SelectListItem>();
            this.StepList = new List<SelectListItem>();
        }
        #endregion

        #region Standard Property

        [DisplayName("Employee ID")]
        [UIHint("_ReadOnly")]
        public string EmpId { get; set; }

        [Required]
        [MaxLength(200)]
        [UIHint("_ReadOnly")]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [UIHint("_ReadOnly")]
        public String Designation { get; set; }

        [DisplayName("PF Membership Status")]
        [UIHint("_ReadOnly")]
        public String PFMembershipStatus { get; set; }

        public DateTime? DateofInactive { get; set; }

        public int EmployeeId { get; set; }

        [DisplayName("Job Grade")]
        [Required]
        public int GradeId { get; set; }

        [Required]
        //[UIHint("_ReadOnly")]
        public string JobGrade { get; set; }

        public IEnumerable<SelectListItem> GradeList { get; set; }

        [DisplayName("Salary Scale")]
        [Required]
        public int SalaryScaleId { get; set; }

        [Required]
        [UIHint("_ReadOnly")]
        public string SalaryScale { get; set; }

        public IEnumerable<SelectListItem> SalaryScaleList { get; set; }

        [DisplayName("Step Number")]
        [Required]
        public int StepId { get; set; }

        public IEnumerable<SelectListItem> StepList { get; set; }

        [DisplayName("Gross Salary")]
        [Required]
        public decimal GrossSalary { get; set; }

        [DisplayName("Is Consolidated Structure?")]
        public bool isConsolidated { get; set; }

        [DisplayName("Total Addition")]
        [UIHint("_ReadOnlyAmount")]
        public decimal TotalAddition { get; set; }

        [DisplayName("Total Deduction")]
        [UIHint("_ReadOnlyAmount")]
        public decimal TotalDeduction { get; set; }

        [DisplayName("Net Pay")]
        [UIHint("_ReadOnlyAmount")]
        public decimal NetPay { get; set; }

        public int SalaryStructureId { get; set; }

        [DisplayName("Original Gross Salary")]
        public decimal OrgGrossSalary { get; set; }

        #endregion

        #region Association
        public ICollection<SalaryStructureDetailsModel> SalaryStructureDetail { get; set; }
        #endregion

        #region Others

        public string SelectedClass { get; set; }
        public bool IsSalaryProcess { get; set; }

        public decimal? HouseRentByRule { get; set; }
        public int? HouseRentSalaryHeadId { get; set; }

        [DisplayName("House rent region and probable amount by rule")]
        [UIHint("_ReadOnly")]
        public String HouseRentRegionAndAmount { get; set; }

        #endregion
    }
}