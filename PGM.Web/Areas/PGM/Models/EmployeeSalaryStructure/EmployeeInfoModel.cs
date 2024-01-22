using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure
{
    public class EmployeeInfoModel : BaseViewModel
    {


        public EmployeeInfoModel()
        {
            this.BankList = new List<SelectListItem>();
            this.BankBranchList = new List<SelectListItem>();
            this.JobGradeList = new List<SelectListItem>();
            this.AssesseTypeList = new List<SelectListItem>();
            this.TaxRegionList = new List<SelectListItem>();
            this.SalaryWithdrawFromList = new List<SelectListItem>();
            this.WorkingZoneList = new List<SelectListItem>();
            this.MembershipStatusList = new List<SelectListItem>();

            EmployeeSalary = new EmployeeSalaryStructureModel();
        }

        public String ActionName { get; set; }
        public string SelectedClass { get; set; }

        [DisplayName("Employee ID")]
        public String EmpID { get; set; }

        [DisplayName("Full Name")]
        public virtual String FullName { get; set; }

        [DisplayName("Full Name (Bangla)")]
        public String FullNameBangla { get; set; }

        public String DesignationName { get; set; }
        public int DesigSortOrder { get; set; }
        public int ZoneInfoId { get; set; }
        public bool IsPhotoExist { get; set; }
        public String Gender { get; set; }
        public String Religion { get; set; }

        public String EmialAddress { get; set; }
        public String MobileNo { get; set; }

        [DisplayName("e-TIN")]
        [MaxLength(20)]
        public String ETIN { get; set; }

        [DisplayName("Birth Date")]
        public DateTime? DateofBirth { get; set; }
        [DisplayName("Joining Date")]
        public DateTime DateofJoining { get; set; }
        [DisplayName("Confirmation Date")]
        public DateTime? DateofConfirmation { get; set; }
        [DisplayName("Appointment Date")]
        public DateTime? DateofAppointment { get; set; }
        [DisplayName("Inactive Date")]
        public DateTime? DateofInactive { get; set; }
        [DisplayName("Retirement Date")]
        public DateTime? DateofRetirement { get; set; }
        [DisplayName("Contract End Date")]
        public DateTime? ContractExpireDate { get; set; }

        [Required]
        [DisplayName("Job Grade")]
        public int JobGradeId { get; set; }
        public IList<SelectListItem> JobGradeList { get; set; }

        public String JobGradeName { get; set; }

        public String StaffCategoryName { get; set; }

        [DisplayName("Bank")]
        public int? BankId { get; set; }
        public IList<SelectListItem> BankList { get; set; }

        [DisplayName("Branch")]
        public int? BankBranchId { get; set; }
        public IList<SelectListItem> BankBranchList { get; set; }

        [DisplayName("Account No.")]
        [MaxLength(50)]
        public virtual String BankAccountNo { get; set; }

        [DisplayName("Salary Withdraw From")]
        public int SalaryWithdrawFromZoneId { get; set; }
        public IList<SelectListItem> SalaryWithdrawFromList { get; set; }

        [DisplayName("Working Zone")]
        public int WorkingZoneId { get; set; }
        public IList<SelectListItem> WorkingZoneList { get; set; }

        public string WorkingZoneName { get; set; }

        [DisplayName("Tax Region")]
        public int? TaxRegionId { get; set; }
        public IList<SelectListItem> TaxRegionList { get; set; }

        [DisplayName("Assessee Type")]
        public byte? TaxAssesseeType { get; set; }
        public IList<SelectListItem> AssesseTypeList { get; set; }

        [DisplayName("Have children with disability")]
        public bool HavingChildWithDisability { get; set; }


        [DisplayName("Contract Duration(Month)")]
        public decimal? ContractDuration { get; set; }

        public bool IsContractual { get; set; }
        public bool IsConsultant { get; set; }
        public bool? IsBonusEligible { get; set; }
        public bool IsPensionEligible { get; set; }
        public bool IsLeverageEligible { get; set; }
        public bool? IsOvertimeEligible { get; set; }
        public bool? IsRefreshmentEligible { get; set; }
        public bool? IsGPFEligible { get; set; }

        public int DesignationId { get; set; }
        public int EmploymentTypeId { get; set; }
        public int StaffCategoryId { get; set; }
        public int EmployeeStatusId { get; set; }

        public EmployeeSalaryStructureModel EmployeeSalary { get; set; }
        public bool IsSalaryAlreadyProcessed { get; set; }

        public string OrganogramLevelName { get; set; }
        public string DivisionName { get; set; }
        public string DisciplineName { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public string SalaryScaleName { get; set; }
        public string EmployeeClassName { get; set; }
        [DisplayName("Employment Type")]
        public String EmpTypeName { get; set; }
        [DisplayName("Employee Status")]
        public String EmpStatusName { get; set; }

        [DisplayName("PF Membership Status")]
        [Required]
        public String MembershipStatus { get; set; }
        public IList<SelectListItem> MembershipStatusList { get; set; }

        [DisplayName("PF Membership Date")]
        public DateTime? PFMembershipDate { get; set; }
    }
}