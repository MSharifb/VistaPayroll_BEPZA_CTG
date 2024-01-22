using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using PGM.Web.Resources;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class MISReport : PGMReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedUserZoneInfoId == 0) return;

            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = String.Empty;

                var selectedYear = Convert.ToString(ddlYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlMonth.SelectedItem);

                var selectedZoneLists = new List<int>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneLists.Add(Convert.ToInt32(item.Value));
                    }
                }

                if (selectedZoneLists == null || selectedZoneLists.Count == 0)
                {
                    lblMsg.Text = ErrorMessages.ZoneRequired;
                }
                else
                {
                    GenerateReport(selectedYear
                        , selectedMonth
                        , ConvertListToString(selectedZoneLists.ToArray()));
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvReportViewer_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        public void GenerateReport(string selectedYear, string selectedMonth, string zoneList)
        {
            #region Processing Report Data
            rvReportViewer.Reset();

            var data = _pgmExecuteFunctions.GetMISReport(selectedYear, selectedMonth, zoneList);

            string paramReportTitle = string.Empty;
            string parameter = string.Empty;

            #region Search parameter

            paramReportTitle = "MIS REPORT";
            parameter = "For the month of " + selectedMonth + "/" + selectedYear;

            #endregion

            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                rvReportViewer.ProcessingMode = ProcessingMode.Local;
                rvReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/MISReport.rdlc");

                rvReportViewer.LocalReport.DataSources.Add(new ReportDataSource("DSMISReport", data));

                ReportParameter p1 = new ReportParameter("reportTitle", paramReportTitle);
                ReportParameter p2 = new ReportParameter("Param", parameter);

                rvReportViewer.LocalReport.SetParameters(new ReportParameter[] {p1, p2});
                this.rvReportViewer.LocalReport.SubreportProcessing +=
                    new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvReportViewer.DataBind();

                //ExportToPDF
                String fileName = "MISReport_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvReportViewer, fileName);
            }

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var dsName = string.Empty;
            switch (e.ReportPath)
            {
                case "_PGMZoneWiseReportHeader":
                    dsName = "DSCompanyInfo";
                    var data = base.GetZoneInfoForReportHeader();
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;
            }
        }


        #region User Methods

        private void PopulateDropdownList()
        {

            int j = 0;
            foreach (var year in Common.PopulateYearList().ToList())
            {
                ddlYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });
                j++;
            }

            ddlYear.Items.FindByValue(DateTime.Now.Year.ToString()).Selected = true;

            int k = 0;
            foreach (var month in Common.PopulateMonthListForReport().ToList())
            {
                ddlMonth.Items.Insert(k, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });
                k++;
            }
            ddlMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

        #endregion
    }
}