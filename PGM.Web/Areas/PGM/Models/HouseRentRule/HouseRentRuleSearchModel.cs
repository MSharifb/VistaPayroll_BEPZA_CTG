using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;
using System.Text;
using System.ComponentModel;

namespace PGM.Web.Areas.PGM.Models.HouseRentRule
{
    public class HouseRentRuleSearchModel
    {
        #region Properties
        public int? ID { get; set; }
        public int SalaryScaleId { get; set; }
        public int RegionId { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));

            if (SalaryScaleId != 0)
                filterExpressionBuilder.Append(String.Format("SalaryScaleId like {0} AND ", SalaryScaleId));

            if (RegionId != 0)
                filterExpressionBuilder.Append(String.Format("RegionId like {0} AND ", RegionId));
           
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);

            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}