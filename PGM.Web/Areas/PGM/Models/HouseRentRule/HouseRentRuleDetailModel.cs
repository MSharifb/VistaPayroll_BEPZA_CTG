using System.ComponentModel.DataAnnotations;

namespace PGM.Web.Areas.PGM.Models.HouseRentRule
{
    public class HouseRentRuleDetailModel : BaseViewModel
    {
        public HouseRentRuleDetailModel()
        {

        }

        #region Standard Property

        public int HouseRentRuleId { get; set; }

        [Required]
        public int SlabNo { get; set; }

        [UIHint("_OnlyNumber")]
        [Required]
        public decimal SalaryFrom { get; set; }

        [UIHint("_OnlyNumber")]
        [Required]
        public decimal SalaryTo { get; set; }

        [UIHint("_OnlyNumber")]
        [Required]
        public decimal PercentOfSalary { get; set; }

        [UIHint("_OnlyNumber")]
        [Required]
        public decimal MinHouseRent { get; set; }

        //[UIHint("_OnlyNumber")]
        //[Required]
        //public decimal MaxHouseRent { get; set; }        

        #endregion
    }
}
