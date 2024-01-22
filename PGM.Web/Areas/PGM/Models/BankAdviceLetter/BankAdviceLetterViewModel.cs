using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using PGM.Web.Utility;


namespace PGM.Web.Areas.PGM.Models.BankAdviceLetter
{
    #region Bank Advice Letter Master
    public class BankAdviceLetterViewModel
    {
        public BankAdviceLetterViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
            this.BankList = new List<SelectListItem>();
            this.BranchList = new List<SelectListItem>();
            this.AccountNoList = new List<SelectListItem>();
            this.SignatoryNameList = new List<SelectListItem>();
            this.BankLetterDetails = new List<BankAdviceLetterDetailModel>();
            this.BonusTypeList = new List<SelectListItem>();

            this.Mode = CrudeAction.Create;
        }

        #region Standard Properties

        [DataColumn(true)]
        public Int64 Id { get; set; }

        [DataColumn(true)]
        public int EmployeeId { get; set; }
        //[Required]
        [DisplayName("Letter Type")]
        public string LetterType { get; set; }
        public string LetterTypeB { get; set; }
        public string BonusType { get; set; }
        public string ReportPath { get; set; }
        [DisplayName("Date of Letter")]
        [Required]
        [DataColumn(true)]
        [UIHint("_Date")]
        public DateTime DateofLetter { get; set; }

        //[DisplayName("Limit of Amount")]
        //[Required]
        //[DataColumn(true)]
        //[UIHint("_OnlyCurrency")]
        //public decimal LimitAmount { get; set; }
        [DataColumn(true)]
        [DisplayName("Amount")]
        [Required]
        public decimal TotalAmount { get; set; }
        [DisplayName("Month")]
        [Required]
        [DataColumn(true)]
        public string SalaryMonth { get; set; }

        [DisplayName("Year")]
        [Required]
        [DataColumn(true)]
        public string SalaryYear { get; set; }

        //[DisplayName("Initial of Signatory")]
        //[Required]
        //[DataColumn(true)]
        //public int SignatoryId { get; set; }

        //[DisplayName("Name of Signatory")]
        //[Required]
        //[DataColumn(true)]
        //public string SignatoryName { get; set; }

        //[Required]
        //[DataColumn(true)]
        //public string SignatoryInitial { get; set; } 

        //[DisplayName("Designation of Signatory")]
        //[Required]
        //[DataColumn(true)]
        //public string SignatoryDesignation { get; set; }

        [DisplayName("Reference")]
        [Required]
        [DataColumn(true)]
        public string ReferenceNo { get; set; }

        [DisplayName("Account No.")]
        //[Required]
        //[DataColumn(true)]
        public string AccountNo { get; set; }

        [DisplayName("Bank Name")]
        //[Required]
        //[DataColumn(true)]
        public int BankId { get; set; }

        [DisplayName("Branch Name")]
        //[Required]
        [DataColumn(true)]
        public int BranchId { get; set; }

        [DataColumn(true)]
        public string BranchName { get; set; }

        [DataColumn(true)]
        public string BankName { get; set; }

        [DataColumn(true)]
        public string BankAddress { get; set; }

        //[DisplayName("Total Amount")]
        //[Required]
        //[DataColumn(true)]
        //[UIHint("_OnlyCurrency")]
        //public Decimal TotalPayable { get; set; } 
        [DisplayName("Bonus Type")]
        public int? BonusTypeId { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        //[Required]
        public int ZoneInfoId { get; set; }

        #endregion


        #region Other properties

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string errClass { set; get; }
        public IList<BankAdviceLetterDetailModel> BankLetterDetails { get; set; }
        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> BankList { get; set; }
        public IList<SelectListItem> BranchList { get; set; }
        public IList<SelectListItem> AccountNoList { get; set; }
        public IList<SelectListItem> SignatoryNameList { get; set; }
        public IList<SelectListItem> BonusTypeList { get; set; }
        public string SelectedLetterType { get; set; }
        public string LetterTypeClickChange { get; set; }

        [DisplayName("Select All/None")]
        public bool SelectAll { get; set; }


        public IList<LetterTypeDetail> LetterTypeDetails { get; set; }


        #endregion
    }
    #endregion

    public class LetterTypeDetail
    {
        public string Description { get; set; }
        public int Id { get; set; }

    }


    #region Bank Advice Letter Details
    public class BankAdviceLetterDetailModel
    {

        public BankAdviceLetterDetailModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
        }

        #region Standard Properties

        public Int64 Id { get; set; }
        public Int64 BankLetterId { get; set; }
        public int EmployeeId { get; set; }
        public string EmpID { get; set; }
        public string AccountNo { get; set; }
        public decimal NetPayable { get; set; }

        public bool Ischecked { get; set; }


        [DisplayName("Employee Name")]
        //[Required]
        public string FullName { get; set; }

        [DisplayName("Employee Initial")]
        //[Required]
        public string EmployeeInitial { get; set; }

        public string BankName { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }
        #endregion
    }

    #endregion
}