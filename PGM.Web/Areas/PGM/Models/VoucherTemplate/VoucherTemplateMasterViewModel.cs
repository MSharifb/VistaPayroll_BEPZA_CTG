using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.VoucherTemplate
{
    public class VoucherTemplateMasterViewModel
    {
        public VoucherTemplateMasterViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.UtcNow;
            this.EDate = this.IDate;
            this.ApproverList = new List<SelectListItem>();
        }

        #region Standard Properties

        public int Id { get; set; }

        [DisplayName("Recommender/Approver")]
        [Required]
        public int ApproverId { get; set; }

        [DisplayName("Transaction Mode")]
        [Required]
        public string TransactionMode { get; set; }

        [DisplayName("Voucher Template For")]
        [Required]
        public string VoucherTemplateFor { get; set; }

        [DisplayName("Pay To")]
        [Required]
        public string PayTo { get; set; }

        [DisplayName("Narration")]
        [Required]
        public string Narration { get; set; }

        public string IUser { get; set; }
        public string EUser { get; set; }
        public System.DateTime IDate { get; set; }
        public System.DateTime EDate { get; set; }
        public int ZoneInfoId { get; set; }
        #endregion

        #region Other properties
        public string Year { get; set; }
        public string Month { get; set; }
        public string Mode { get; set; }
        public int IsError { set; get; }
        public string ErrMsg { set; get; }
        public string errClass { get; set; }
        public IList<SelectListItem> ApproverList { get; set; }
        #endregion
    }
}