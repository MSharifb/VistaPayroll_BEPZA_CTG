using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class JournalVoucherRpt : PGMReportBase
    {
        string strZoneId = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId == 0) return;
                PopulateDropdownList();
            }
        }

        #region Button Event
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = String.Empty;
                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem.Text);

                GenerateReport(selectedYear, selectedMonth);
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }
        #endregion

        private void GenerateReport(string selectedYear, string selectedMonth)
        {
            #region Processing Report Data
            rvJournalVoucherRpt.Reset();

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var reportHeader = base.GetZoneInfoForReportHeader();

            var data = (from s in base._pgmContext.sp_PGM_GenerateJournalVoucher(selectedYear, selectedMonth, numErrorCode, strErrorMsg) select s).ToList();

            // Filter by SalaryWithdrawFromZoneId
            data = data.Where(x => x.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId).ToList();


            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                rvJournalVoucherRpt.ProcessingMode = ProcessingMode.Local;
                rvJournalVoucherRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/JournalVoucherRpt.rdlc");
                rvJournalVoucherRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));
                rvJournalVoucherRpt.LocalReport.DataSources.Add(new ReportDataSource("dsJournalVoucher", data));

                int selectedMonthValue = Convert.ToInt32(ddlSelectMonth.SelectedValue);

                int days = DateTime.DaysInMonth(Convert.ToInt32(selectedYear), selectedMonthValue);
                var firstDate = Convert.ToDateTime("01-" + selectedMonth + "-" + selectedYear);
                var lastDate = Convert.ToDateTime(days + "-" + selectedMonth + "-" + selectedYear);

                string searchParameters = String.Concat("Statement for the period of ", firstDate.ToString("dd-MMM-yyyy"), " to ", lastDate.ToString("dd-MMM-yyyy"));

                ReportParameter p1 = new ReportParameter("param1", firstDate.ToString("dd-MMM-yyyy"));
                ReportParameter p2 = new ReportParameter("param2", lastDate.ToString("dd-MMM-yyyy"));
                ReportParameter p3 = new ReportParameter("param3", searchParameters);

                rvJournalVoucherRpt.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3 });

                rvJournalVoucherRpt.LocalReport.SubreportProcessing += LocalReport_SubreportProcessing;
                rvJournalVoucherRpt.DataBind();

                //ExportToPDF
                String fileName = "JournalVoucher_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvJournalVoucherRpt, fileName);
            }


            #endregion
        }

        void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
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
            catch (Exception ex)
            {

                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvJournalVoucherRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Others

        private void PopulateDropdownList()
        {
            ddlSelectMonth.DataSource = Common.PopulateMonthList2();
            ddlSelectMonth.DataValueField = "Value";
            ddlSelectMonth.DataTextField = "Text";
            ddlSelectMonth.DataBind();
            ddlSelectMonth.Items.FindByText(DateTime.Now.ToString("MMMM")).Selected = true;

            ddlSelectYear.DataSource = Common.PopulateYearList();
            ddlSelectYear.DataValueField = "Text";
            ddlSelectYear.DataTextField = "Value";
            ddlSelectYear.DataBind();
        }

        #endregion


    }
}