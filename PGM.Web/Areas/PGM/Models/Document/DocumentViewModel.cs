using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.Document
{
    public class DocumentViewModel : BaseViewModel
    {
        public DocumentViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.IDate = DateTime.Now;
        }

        public string DocumentName { get; set; }
        
        public int DocumentTypeId { get; set; }
        
        public DateTime DocumentUploadDate { get; set; }
        
        public decimal DocumentSizeInMB { get; set; }
        
        public string DocumentExtension { get; set; }
        
        public int DetailId { get; set; }

        #region other

        [DisplayName("Attachment")]
        public byte[] Attachment { get; set; }

        public HttpPostedFileBase File { get; set; }

        #endregion

    }
}