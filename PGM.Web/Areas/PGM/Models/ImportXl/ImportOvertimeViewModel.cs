using System;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.ImportXl
{
    public class ImportOvertimeViewModel : LongBaseViewModel
    {
        public ImportOvertimeViewModel()
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
        public String Account_Number { get; set; }
        public String OT_Month { get; set; }
        public String OT_Year { get; set; }
        public decimal Basic_Salary { get; set; }
        public decimal OT_Rate { get; set; }
        public decimal Revenue_Stamp { get; set; }
        public decimal Actual_Hour { get; set; }
        public decimal Approved_Hour { get; set; }
        public decimal Deduction_Percentage { get; set; }
        public decimal Net_Payable { get; set; }
    }
}