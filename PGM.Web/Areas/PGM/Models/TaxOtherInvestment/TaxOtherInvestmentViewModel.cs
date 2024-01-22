using PGM.Web.Areas.PGM.Models.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxOtherInvestment
{
    public class TaxOtherInvestmentViewModel : BaseViewModel
    {
        public TaxOtherInvestmentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.AssessmentYearList = new List<SelectListItem>();
            this.IncomeYearList = new List<SelectListItem>();
            this.OtherInvestmentDetailList = new List<TaxOtherInvestmentDetailViewModel>();
        }
        
        [Required]
        [DisplayName("Assessment Year")]
        public string AssessmentYear { get; set; }
        
        [Required]
        [DisplayName("Income Year")]
        public string IncomeYear { get; set; }

        [Required]
        [DisplayName("Employee")]
        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        [Required]
        public string EmpId { get; set; }
        
        public string EmployeeName { get; set; }

        #region other
        public bool EditAddNew { get; set; }
        public IList<SelectListItem> AssessmentYearList { get; set; }
        public IList<SelectListItem> IncomeYearList { get; set; }
        public IList<TaxOtherInvestmentDetailViewModel> OtherInvestmentDetailList { get; set; }

        #endregion

    }

    public class TaxOtherInvestmentDetailViewModel : BaseViewModel
    {
        public TaxOtherInvestmentDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.OtherInvestmentDocumentList = new List<DocumentViewModel>();
            this.DetailList = new List<TaxOtherInvestmentDetailViewModel>();
            this.InvestmentTypeList = new List<SelectListItem>();
        }
        
        public int EntityId { get; set; }
        
        [DisplayName("Investment Description")]
        public string EntityDescription { get; set; }
        
        [DisplayName("Investment Amount")]
        [Required]
        [Range(1, 9999999999999999.99, ErrorMessage = "Amount cannot be zero.")]
        public decimal EntityAmount { get; set; }

        [UIHint("_Date")]
        [DisplayName("Investment Date")]
        public DateTime? EntityEntryDate { get; set; }
        
        public string IncomeYear { get; set; }
        
        public string AssessmentYear { get; set; }
        
        public int EmployeeId { get; set; }
        [DisplayName("Employee ID")]
        
        public string EmpId { get; set; }
        [DisplayName("Employee Name")]
        
        public string EmployeeName { get; set; }
        [Required]
        [DisplayName("Investment Type")]
        
        public int InvestmentTypeId { get; set; }

        #region other
        public bool EditAddNew { get; set; }
        public IList<DocumentViewModel> OtherInvestmentDocumentList { get; set; }
        public IList<TaxOtherInvestmentDetailViewModel> DetailList { get; set; }
        public IList<SelectListItem> InvestmentTypeList { get; set; }

        #endregion

        #region Attachment
        public HttpPostedFileBase File { get; set; }
        [DisplayName("Attachment")]
        public byte[] Attachment { get; set; }
        public string DocumentName { get; set; }
        public int DocumentTypeId { get; set; }
        public DateTime DocumentUploadDate { get; set; }

        #endregion

    }
}