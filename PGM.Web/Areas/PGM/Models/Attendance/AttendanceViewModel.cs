using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Models.Attendance
{
    public class AttendanceViewModel : BaseViewModel
    {
        public AttendanceViewModel()
        {
            IUser = HttpContext.Current.User.Identity.Name;
            IDate = DateTime.UtcNow;

            this.MonthList = new List<SelectListItem>();
            this.YearList = new List<SelectListItem>();
            this.EmployeeList = new List<SelectListItem>();
        }

        #region Standard Property

        [Required]
        [DisplayName("Employee ID")]
        public int EmployeeId { get; set; }

        public string EmpID { get; set; }
        
        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Employee Designation")]
        [UIHint("_ReadOnly")]
        public string EmployeeDesignation { get; set; }

        [DisplayName("Account No")]
        public string AccountNo { get; set; }
        
        [Required]
        [DisplayName("Month")]
        public string AttMonth { get; set; }
        
        [Required]
        [DisplayName("Year")]
        public string AttYear { get; set; }
        
        [DisplayName("Calender Days")]
        [Range(1, 999)]
        public int CalenderDays { get; set; }

        [UIHint("_Date")]
        [DisplayName("Attendance From Date")]
        public DateTime? AttFromDate { get; set; }

        [UIHint("_Date")]
        [DisplayName("Attendance To Date")]
        public DateTime? AttToDate { get; set; }

        [DisplayName("Total Present")]
        [Range(0, 999.99)]
        public decimal TotalPresent { get; set; }

        [DisplayName("Total Casual Leave")]
        [Range(0, 999.99)]
        public decimal TotalCasualLeave { get; set; }

        [DisplayName("Total Earned Leave")]
        [Range(0, 999.99)]
        public decimal TotalEarnedLeave { get; set; }

        [DisplayName("Total Others Leave")]
        [Range(0, 999.99)]
        public decimal TotalOthersLeave { get; set; }

        [DisplayName("Total Attendance")]
        [Range(0, 999.99)]
        public decimal TotalAttendance { get; set; }

        public string Remark { get; set; }

        public int ZoneIdDuringAttendance { get; set; }
        #endregion

        #region Other

        public IList<SelectListItem> YearList { get; set; }
        public IList<SelectListItem> MonthList { get; set; }
        public IList<SelectListItem> EmployeeList { get; set; }

        #endregion
    }
}