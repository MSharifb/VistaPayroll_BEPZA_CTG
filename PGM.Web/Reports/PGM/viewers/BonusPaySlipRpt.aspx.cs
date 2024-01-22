using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class BonusPaySlipRpt : PGMReportBase
    {
        bool status = false;
        [NoCache]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedUserZoneInfoId == 0) return;

            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }
        [NoCache]
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                var selectedBonusType = Convert.ToInt32(ddlSelectBonusType.SelectedValue);
                int selectedEmployeeID = default(int);
                if (txtEmployeeInitial.Text != string.Empty)
                {
                    var empInitial = txtEmployeeInitial.Text.Trim().ToString();
                    selectedEmployeeID = (from p in _pgmExecuteFunctions.GetEmployeeList() where p.EmployeeInitial.Trim().ToLower() == empInitial.ToLower() select p.Id).FirstOrDefault();
                }

                GenerateReport(selectedYear
                    , selectedMonth
                    , selectedEmployeeID
                    , selectedBonusType);

                lblMsg.Text = default(string);
                lblErrmsg.Text = default(string);

                if (status == true)
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
                status = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
        [NoCache]
        private void GenerateReport(string selectedYear, string selectedMonth, int selectEmployeeID, int selectedBonusType)
        {
            #region Processing Report Data

            var data = (from s in base._pgmContext.vwPGMBonusPaySlips.Where(y => y.BonusYear == selectedYear && y.BonusMonth == selectedMonth) select s).OrderBy(x => x.EmpID).ToList();

            if (selectedBonusType > 0) data = data.Where(q => q.BonusTypeId == selectedBonusType).ToList();

            if (txtEmployeeInitial.Text != string.Empty)
            {
                if (selectEmployeeID > 0) data = data.Where(q => q.EmployeeId == selectEmployeeID).ToList();
            }

            var reportHeader = base.GetZoneInfoForReportHeader();

            if (data.Count > 0 && !string.IsNullOrEmpty(selectedYear) && (!string.IsNullOrEmpty(selectedMonth)) && reportHeader != null)
            {

                lblMsg.Text = default(string);
                rvBonusPaySlipRpt.Reset();
                rvBonusPaySlipRpt.ProcessingMode = ProcessingMode.Local;
                rvBonusPaySlipRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/BonusPaySlipRpt.rdlc");

                rvBonusPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsBonusPaySlip", data));
                rvBonusPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));
                string searchParameters = "For the Month of " + " " + selectedMonth.ToString().Substring(0, 3) + "/" + selectedYear.ToString();

                ReportParameter p1 = new ReportParameter("param", searchParameters);

                rvBonusPaySlipRpt.LocalReport.SetParameters(new ReportParameter[] { p1 });

            }
            else
            {
                status = true;
                rvBonusPaySlipRpt.Reset();
            }
            rvBonusPaySlipRpt.DataBind();

            //ExportToPDF
            String fileName = "BonusPaySlip_" + Guid.NewGuid() + ".pdf";
            ExportToPDF(rvBonusPaySlipRpt, fileName);

            #endregion
        }
        [NoCache]
        protected void rvBonusPaySlipRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
        [NoCache]
        private void PopulateDropdownList()
        {
            int j = 0;
            foreach (var year in Common.PopulateYearList().ToList())
            {
                ddlSelectYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });
                j++;
            }

            int i = 0;
            foreach (var month in Common.PopulateMonthListForReport().ToList())
            {
                ddlSelectMonth.Items.Insert(i, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });
                i++;
            }
            ddlSelectMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;

            ddlSelectBonusType.Items.Insert(0, new ListItem() { Text = "Select Bonus Type", Value = "0" });
            var bonusTypeList = (from p in _pgmContext.PGM_BonusType.OrderBy(x => x.BonusType).DistinctBy(d => d.BonusType) select p).ToList();
            int k = 1;
            foreach (var bonusType in bonusTypeList)
            {
                ddlSelectBonusType.Items.Insert(k, new ListItem() { Text = bonusType.BonusType.ToString(), Value = bonusType.Id.ToString() });
                k++;
            }

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }


    }
}