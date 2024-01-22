using System;
using PGM.Web.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.ImportXl
{
    public class ImportXlViewModel : BaseViewModel
    {
        #region ctor
        public ImportXlViewModel()
        {
            this.FileTypeList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.MonthList = new List<SelectListItem>();
        }

        #endregion

        #region Standard Property

        [DisplayName("Per-Day Allowance (Refreshment)")]
        [UIHint("_OnlyNumber")]
        [Range(0, 9999.99)]
        public decimal DailyAllowance { get; set; }

        [DisplayName("Revenue Stamp")]
        [UIHint("_OnlyNumber")]
        [Range(0, 999.99)]
        public decimal RevenueStamp { get; set; }

        [DisplayName("Deduction Percentage (Overtime)")]
        [UIHint("_OnlyNumber")]
        [Range(0, 999.99)]
        public decimal DeductionPercentage { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        public string Month { get; set; }


        [DisplayName("File Type")]
        [Required]
        public int? FileTypeId { get; set; }

        [DisplayName("Attachment")]
        [AllowedFileExtension(".xlsx", ".xls", ".xlsm", ".xltx", ".xltm")]
        public HttpPostedFileBase File { get; set; }

        [DisplayName("Attendance From")]
        [UIHint("_Date")]
        public DateTime? AttFromDate { get; set; }

        [DisplayName("Attendance To")]
        [UIHint("_Date")]
        public DateTime? AttToDate { get; set; }

        public IList<SelectListItem> FileTypeList { get; set; }
        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        

        #endregion

    }
}