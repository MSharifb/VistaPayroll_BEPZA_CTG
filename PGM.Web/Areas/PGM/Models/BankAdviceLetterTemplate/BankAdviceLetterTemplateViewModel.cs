using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Models.BankAdviceLetterTemplate   
{
    public class BankAdviceLetterTemplateViewModel  
    {
        public BankAdviceLetterTemplateViewModel() 
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.LetterTypeList = new List<SelectListItem>();
            this.BankLetterBodyVariableList = new List<BankAdviceLetterBodyVariable>(); 
            this.Mode = CrudeAction.Create;
            this.BankList = new List<SelectListItem>();
        }

        #region Standard Properties
       
        public int Id { get; set; }
        [DisplayName("Letter Type")]
        [Required]
        public string LetterType { get; set; }
        [DisplayName("Recipient's Address")]
        [Required]
        public string Addressee { get; set; }
        [DisplayName("Subject")]
        [Required]
        public string LetterSubject { get; set; }
        [Required]
        public string Salutation { get; set; }
        [DisplayName("Body (Part 1)")]
        [Required]
        public string Body1 { get; set; }
        [DisplayName("Body (Part 2)")]
        public string Body2 { get; set; }
        [Required]
        public string Complementary { get; set; }
        [Required]
        public string Enclosure { get; set; }
        public int Signatory1Id { get; set; }
        [Required]
        public string Signatory1 { get; set; }
        [Required]
        [DisplayName("Designation")]
        public string Signatory1Designation { get; set; }
        public int Signatory2Id { get; set; }
        [Required]
        public string Signatory2 { get; set; }
        [Required]
        [DisplayName("Designation")]
        public string Signatory2Designation { get; set; }
        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }
        public int ZoneInfoId { get; set; }
        [DisplayName("Bank Name")]
        public int? BankId { get; set; }
        #endregion 

        #region Other properties

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public IList<SelectListItem> LetterTypeList { get; set; }
        public IList<BankAdviceLetterBodyVariable> BankLetterBodyVariableList { get; set; }
        public IList<SelectListItem> BankList { get; set; }
        public string BankName { get; set; }
        #endregion

        
    }

    public class BankAdviceLetterBodyVariable
    {
        public int Id { get; set; }
        public string VariableNames { get; set; }
    }
}