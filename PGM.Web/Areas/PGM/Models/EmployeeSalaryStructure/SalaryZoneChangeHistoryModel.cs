using System;

namespace PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure
{
    public class SalaryZoneChangeHistoryModel: BaseViewModel
    {
        public String SalaryZoneName { get; set; }
        public DateTime ChangeDate { get; set; }
        public bool IsInactive { get; set; }
    }
}