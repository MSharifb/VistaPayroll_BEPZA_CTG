using System;
using System.Collections.Generic;
using System.Linq; 
using System.Web;
using System.Text;
using System.ComponentModel;

namespace PGM.Web.Areas.PGM.Models.TaxRate
{
    public class TaxRateSearchModel
    {
        #region Properties
        public int? ID { get; set; }
        public string IncomeYear { get; set; }
        public string AssessmentYear { get; set; }
        public string ApplicableFor { get; set; }
        public int NumberOfSlab { get; set; }

        #endregion

        #region Methods
        public string GetFilterExpression()
        {
            StringBuilder filterExpressionBuilder = new StringBuilder();

            if (AssessmentYear=="0")
            {
                AssessmentYear = "";
            }

            if (IncomeYear == "0")
            {
                IncomeYear = "";
            }

            if (ApplicableFor == "0")
            {
                ApplicableFor = "";
            }

            if (ID.HasValue)
                filterExpressionBuilder.Append(String.Format("id = {0} AND ", ID));
            if (!String.IsNullOrWhiteSpace(IncomeYear))
                filterExpressionBuilder.Append(String.Format("IncomeYear like {0} AND ", IncomeYear));
            if (!String.IsNullOrWhiteSpace(AssessmentYear)) 
                filterExpressionBuilder.Append(String.Format("AssessmentYear like {0} AND ", AssessmentYear));
            if (!String.IsNullOrWhiteSpace(ApplicableFor))
                filterExpressionBuilder.Append(String.Format("ApplicableFor like {0} AND ", ApplicableFor));
            if ( NumberOfSlab > 0)
                filterExpressionBuilder.Append(String.Format("NumberOfSlab = {0} AND ", NumberOfSlab));
           
            if (filterExpressionBuilder.Length > 0)
                filterExpressionBuilder.Remove(filterExpressionBuilder.Length - 5, 5);
            return filterExpressionBuilder.ToString();
        }
        #endregion
    }
}