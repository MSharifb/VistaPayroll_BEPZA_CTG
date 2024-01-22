using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxOpening
{
    public class TaxOpeningDetailModel
    {
        #region Standard Property

        public int Id { get; set; }

        public int TaxOpeningId { get; set; }

        public int IncomeHeadId { get; set; }

        [Required]
        [UIHint("_ReadOnly")]
        public string IncomeHead { get; set; }

        public string HeadSource { get; set; }

        [UIHint("_OnlyNumber")]
        public decimal IncomeAmount { get; set; }

        #endregion

        
    }

}
