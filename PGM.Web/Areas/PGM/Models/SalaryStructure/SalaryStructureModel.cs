﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.SalaryStructure
{
    public class SalaryStructureModel : BaseViewModel
    {
        public SalaryStructureModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;

            this.GradeList = new List<SelectListItem>();
            this.StepList = new List<SelectListItem>();
            this.SalaryScaleList = new List<SelectListItem>();
            this.SalaryStructureDetail = new List<SalaryStructureDetailsModel>();
        }


        [Required]
        [DisplayName("Salary Scale")]
        public int SalaryScaleId { get; set; }

        [Required]
        [DisplayName("Grade")]
        public int GradeId { get; set; }
        
        [Required]
        [DisplayName("Step")]
        public int StepId { get; set; }

        
        public bool IsConsolidated { get; set; }
        public decimal InitialBasic { get; set; }
        public decimal YearlyIncrement { get; set; }


        public IList<SelectListItem> GradeList { get; set; }
        public IList<SelectListItem> StepList { get; set; }
        public IList<SelectListItem> SalaryScaleList { get; set; }
        public virtual ICollection<SalaryStructureDetailsModel> SalaryStructureDetail { get; set; }

    }
}