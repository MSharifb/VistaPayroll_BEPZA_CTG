using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.HouseRentWaterBillDeduct
{
    public class HouseRentWaterBillDeductModel:LongBaseViewModel
    {
        public HouseRentWaterBillDeductModel()
        {
            this.SalaryHeadList = new List<SelectListItem>();
        }

        #region Standard Property

        //
        [Required]
        public int EmployeeId { get; set; }

        //
        [DisplayName("Water Bil Salary Head (-)")]
        [Required]
        public int WaterBillSalaryHeadDeductId { get; set; }
        public string WaterBillSalaryHeadDeduct { get; set; }

        //
        [DisplayName("Water Bill Amount")]
        [Required]
        [UIHint("_OnlyNumber")]
        public decimal WaterBillAmount { get; set; }

        //
        [DisplayName("House Rent Deduction Type")]
        [Required]
        [Description("Fixed = As it is House Rent Amount; HRA (House Rent Allowance) = Same as emp. wise house rent addition head ")]
        public string HouseRentDeductionType { get; set; }

        //
        [DisplayName("House Rent Salary Head (-)")]
        [Required]
        public int HouseRentSalaryHeadDeductId { get; set; }
        public string HouseRentSalaryHeadDeduct { get; set; }

        //
        [DisplayName("House Rent Fixed Deduction Amount")]
        [UIHint("_OnlyNumber")]
        public decimal?  HouseRentAmount { get; set; }
        
        //
        [DisplayName("Effective Date From")]
        [Required]
        [UIHint("_Date")]
        public DateTime? EffectiveDateFrom { get; set; }

        //
        [DisplayName("Effective Date To")]
        [Required]
        [UIHint("_Date")]
        public DateTime? EffectiveDateTo { get; set; }

        //
        [DisplayName("Is Conitinuous")]
        [Required]
        public bool IsConitinuous { get; set; }

        //
        [Required]
        [DisplayName("Residence Address")]
        public string ResidenceAddress { get; set; }

        [Required]
        public int ZoneInfoId { get; set; }

        #endregion

        #region Others

        //
        [Required]
        [UIHint("_ReadOnly")]
        [DisplayName("Employee ID")]
        public string EmpID { get; set; }

        //
        [UIHint("_ReadOnly")]
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        //
        [UIHint("_ReadOnly")]
        [DisplayName("Employee Designation")]
        public string EmployeeDesignation { get; set; }

        public int DesignationId { get; set; }

        public string Mode { get; set; }

        public IList<SelectListItem> SalaryHeadList { get; set; }

        #endregion       
    }
}
