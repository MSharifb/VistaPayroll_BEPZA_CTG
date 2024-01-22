using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using PGM.Web.Areas.PGM.Models.Document;

namespace PGM.Web.Areas.PGM.Models.TaxAdvancedPaid
{
    public class AdvancedTaxPaidDetailViewModel : BaseViewModel
    {
        public AdvancedTaxPaidDetailViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
            this.AdvanceDetailDocumentList = new List<DocumentViewModel>();
            //this.DetailList = new List<AdvancedTaxPaidDetailViewModel>();
        }
        
        public int EitityId { get; set; }
        
        [DisplayName("Payment Description")]
        public string EntityDescription { get; set; }
        
        [DisplayName("Paid Amount")]
        [Required]
        [Range(typeof(decimal),"1", "9999999999999999.99", ErrorMessage = "Amount cannot be zero.")]
        public decimal EntityAmount { get; set; }
        
        [UIHint("_Date")]
        [DisplayName("Payment Date")]
        [Required]
        public DateTime? EntityEntryDate { get; set; }

        #region other

        


        public string IncomeYear { get; set; }

        public string AssessmentYear { get; set; }

        public int EmployeeId { get; set; }

        [DisplayName("Employee ID")]
        public string EmpId { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }


        public IList<DocumentViewModel> AdvanceDetailDocumentList { get; set; }

        //public IList<AdvancedTaxPaidDetailViewModel> DetailList { get; set; }

        #endregion

        //#region Attachment

        //public HttpPostedFileBase File { get; set; }

        //[DisplayName("Attachment")]
        //public byte[] Attachment { get; set; }
        
        //public string DocumentName { get; set; }
        
        //public int DocumentTypeId { get; set; }
        
        //public DateTime DocumentUploadDate { get; set; }

        //#endregion

    }
}