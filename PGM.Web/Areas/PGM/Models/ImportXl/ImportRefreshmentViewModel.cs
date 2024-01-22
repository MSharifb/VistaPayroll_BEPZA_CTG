using System;
using System.Web;

namespace PGM.Web.Areas.PGM.Models.ImportXl
{
    public class ImportRefreshmentViewModel : BaseViewModel
    {
        public ImportRefreshmentViewModel()
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

        public String R_Month { get; set; }
        public String R_Year { get; set; }
        public Decimal Per_Day_Amount { get; set; }
        public Decimal Total_Days { get; set; }
        public Decimal Revenue_Stamp { get; set; }
        public Decimal Net_Payable { get; set; }
    }
}