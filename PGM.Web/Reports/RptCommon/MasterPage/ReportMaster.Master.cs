using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PRM.viewers
{
    public partial class ReportMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterStartupManager();
        }

        private void RegisterStartupManager()
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DateImageShow", "DateImageShow();", true); 
        }
    }
}