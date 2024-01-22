using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class NightBillRpt : PGMReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedUserZoneInfoId == 0) return;
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #region User Methods
        private void PopulateDropdownList()
        {
            ddlDepartment.DataSource = _pgmContext.PRM_Division.OrderBy(x => x.SortOrder).ToList();
            ddlDepartment.DataValueField = "Id";
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("All", "0"));

            ddlDesignation.DataSource = _pgmContext.PRM_Designation.OrderBy(x => x.SortingOrder).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

            ddlMonth.DataSource = Common.PopulateMonthList();
            ddlMonth.DataValueField = "Text";
            ddlMonth.DataTextField = "Value";
            ddlMonth.DataBind();
            ddlMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;

            ddlYear.DataSource = Common.PopulateYearList();
            ddlYear.DataValueField = "Text";
            ddlYear.DataTextField = "Value";
            ddlYear.DataBind();

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

        #endregion

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(Convert.ToInt32(ddlDepartment.SelectedValue)
                    , Convert.ToInt32(ddlDesignation.SelectedValue)
                    , ddlMonth.SelectedValue.ToString()
                    , ddlYear.SelectedValue.ToString()
                    , txtEmpName.Text.ToString()
                    , txtIcNo.Text.ToString());
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvNightBill_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        public void GenerateReport(int departmentId, int designationId, string month, string year, string empName, string icNo)
        {
            #region Processing Report Data

            var data = (from s in base._pgmContext.SP_PGM_RptNightBill(departmentId, designationId, month, year, empName, icNo) select s).OrderBy(x => x.FullName).ToList();

            string searchParameters1 = string.Empty;
            string searchParameters2 = string.Empty;
            string searchParameters3 = string.Empty;
            string searchParameters4 = string.Empty;
            string searchParameters5 = string.Empty;
            string searchParameters6 = string.Empty;

            #region Search parameter

            if (departmentId > 0)
            {
                searchParameters1 = ddlDepartment.SelectedItem.Text;
            }
            else
            {
                searchParameters1 = "All";
            }

            if (designationId > 0)
            {
                searchParameters2 = ddlDesignation.SelectedItem.Text;
            }
            else
            {
                searchParameters2 = "All";
            }
            searchParameters3 = ddlMonth.SelectedItem.Text;
            searchParameters4 = ddlYear.SelectedItem.Text;

            if (empName != "")
            {
                searchParameters5 = empName;
            }

            if (icNo != "")
            {
                searchParameters6 = icNo;
            }

            #endregion

            lblMsg.Text = "";
            rvNightBill.Reset();
            rvNightBill.ProcessingMode = ProcessingMode.Local;
            rvNightBill.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/NightBillRpt.rdlc");

            rvNightBill.LocalReport.DataSources.Add(new ReportDataSource("NightBillRpt", data));

            ReportParameter p1 = new ReportParameter("Department", searchParameters1);
            ReportParameter p2 = new ReportParameter("Desingnation", searchParameters2);
            ReportParameter p4 = new ReportParameter("Month", searchParameters3);
            ReportParameter p5 = new ReportParameter("Year", searchParameters4);
            ReportParameter p3 = new ReportParameter("EmpName", searchParameters5);
            ReportParameter p6 = new ReportParameter("EmpID", searchParameters6);

            rvNightBill.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });
            this.rvNightBill.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvNightBill.DataBind();

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = base.GetZoneInfoForReportHeader();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }
    }
}