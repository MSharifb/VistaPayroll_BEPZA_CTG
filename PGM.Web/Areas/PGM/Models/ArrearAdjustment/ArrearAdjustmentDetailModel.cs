using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace PGM.Web.Areas.PGM.Models.ArrearAdjustment
{
    public class ArrearAdjustmentDetailModel : BaseViewModel
    {
        #region Standard Property


        [Required]
        public int ArrearAdjustmentId { get; set; }

        //
        [Required]
        public int HeadId { get; set; }        

        //
        [DisplayName("Head Name")]
        public string HeadName { get; set; }

        //
        [DisplayName("Head Type")]
        public string HeadType { get; set; }

        //
        [DisplayName("Amount Type")]
        public string AmountType { get; set; }

        //
        [DisplayName("Based On")]
        public string BasedOn { get; set; }

        //
        [DisplayName("Amount")]
        public Decimal Amount { get; set; }

        #endregion
    }
}