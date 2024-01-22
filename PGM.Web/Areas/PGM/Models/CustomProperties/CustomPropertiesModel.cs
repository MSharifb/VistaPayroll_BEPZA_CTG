using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.CustomProperties
{
    public class CustomPropertiesModel:BaseViewModel
    {
        public CustomPropertiesModel()
        {

        }

        [Required]
        [DisplayName("Fully Qualified Model Name")]
        public String ModelName { get; set; }

        [Required]
        [DisplayName("Property Name")]
        public String PropertyName { get; set; }

        [Required]
        [DisplayName("Display Text")]
        public String DisplayText { get; set; }

        public Boolean IsRequired { get; set; }
    }
}