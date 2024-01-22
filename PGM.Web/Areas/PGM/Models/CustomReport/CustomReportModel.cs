using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.CustomReport
{
    public class CustomReportModel:BaseViewModel
    {
        public CustomReportModel()
        {

        }

        [Required]
        [DisplayName("Report Name")]
        public String ReportName { get; set; }

        [Required]
        [DisplayName("Parameter Hints")]
        public String ReportParameterNameHints { get; set; }

        [Required]
        [DisplayName("Default Value")]
        public String ReportParameterDefaultValue { get; set; }

        public DataTable ReportDataTable { get; set; }
    }
}