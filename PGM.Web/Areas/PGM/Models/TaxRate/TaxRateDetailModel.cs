using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.TaxRate
{
    public class TaxRateDetailModel
    {
        #region Standard Property

        public int Id { get; set; }
        public int TaxRateId { get; set; }     
       
        public int SlabNo { get; set; }

        [UIHint("_OnlyNumber")]
        public decimal LowerLimit { get; set; }

        [UIHint("_OnlyNumber")]
        public decimal UpperLimit { get; set; }

        [UIHint("_OnlyNumber")]
        public decimal TRate { get; set; }

        //public string IUser { get; set; }
        //public string EUser { get; set; }
        //public DateTime IDate { get; set; }
        //public DateTime? EDate { get; set; }

        #endregion

        
    }

}
