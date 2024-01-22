using System;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.ImportXl
{
    public class ImportAttendanceViewModel : BaseViewModel
    {
        public ImportAttendanceViewModel()
        {
            this.IUser = HttpContext.Current.User.Identity.Name;
            this.EUser = this.IUser;
            this.IDate = DateTime.Now;
            this.EDate = this.IDate;
        }

        public int Sl_No { get; set; }
        public String Employee_Id { get; set; }
        public String Employee_Name { get; set; }
        public String Designation { get; set; }
        public String Department { get; set; }
        public String Att_Month { get; set; }
        public String Att_Year { get; set; }
        public int Calender_Days { get; set; }
        public String Att_From_Date { get; set; }
        public String Att_To_Date { get; set; }
        
        public decimal? Total_Present { get; set; }
        public decimal? Total_Casual_Leave { get; set; }
        public decimal? Total_Earned_Leave { get; set; }
        public decimal? Total_Others_Leave { get; set; }
        public decimal? Total_Attendance { get; set; }

        public String Remark { get; set; }
    }
}