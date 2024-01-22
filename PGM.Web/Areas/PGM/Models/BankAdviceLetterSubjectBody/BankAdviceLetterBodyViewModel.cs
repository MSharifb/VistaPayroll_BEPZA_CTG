using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.BankAdviceLetterSubjectBody
{
    public class BankAdviceLetterBodyViewModel
    {
        public BankAdviceLetterBodyViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.LetterTypeList = new List<SelectListItem>();

            this.Mode = CrudeAction.Create;
        }

        #region Standard Properties

        public Int64 Id { get; set; }
        [Required]
        public Int64 BankLetterId { get; set; }
        [DisplayName("Letter Type")]
        [Required]
        public string LetterType { get; set; }
        [DisplayName("Reference No.")]
        [Required]
        public string ReferenceNo { get; set; }
        [DisplayName("Subject")]
        [Required]
        public string LetterSubject { get; set; }
        [Required]
        [DisplayName("Recipient's Address")]
        public string Adressee { get; set; }
        [Required]
        public string Salutation { get; set; }
        [DisplayName("Body 1")]
        [Required]
        public string Body1 { get; set; }
        [DisplayName("Body 2")]
        public string Body2 { get; set; }
        [Required]
        public string Complementary { get; set; }
        [Required]
        public string Enclosure { get; set; }
        public int Signatory1Id { get; set; }
        [Required]
        public string Signatory1Designation { get; set; }
        public int Signatory2Id { get; set; }
        [Required]
        public string Signatory2Designation { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion

        #region Other properties

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public IList<SelectListItem> LetterTypeList { get; set; }


        #endregion
    }
}