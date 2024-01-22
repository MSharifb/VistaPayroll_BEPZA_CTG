using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using MFS_IWM.Web.Utility;

namespace MFS_IWM.Web.Areas.PGM.Models.BankAdviceLetterBodySetup
{
    public class BankAdviceLetterBodyViewModel
    {
        public BankAdviceLetterBodyViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.Mode = "Create";
        }

        #region Standard Properties
       
        public Int32 Id { get; set; }

        [DisplayName("Letter Type")]
        [Required]
        public int LetterTypeId { get; set; }

        [DisplayName("Subject")]
        [Required]
        public string Subject { get; set; }

        [DisplayName("Body1")]
        public string Body1 { get; set; }

        [DisplayName("Body2")]
        public string Body2 { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }

        #endregion 

        #region Other properties

        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }


        #endregion
    }
}