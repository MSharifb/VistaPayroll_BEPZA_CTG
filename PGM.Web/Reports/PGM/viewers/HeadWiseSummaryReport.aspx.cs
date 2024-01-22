using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using PGM.Web.Resources;


namespace PGM.Web.Reports.PGM.viewers
{
    public partial class HeadWiseSummaryReport : PGMReportBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId == 0) return;

                PopulateDropdownList();

                // Populate zone from session. It is not possible to do this with other thread except UI thread.
                ddlZone.DataSource = ZoneListCached;
                ddlZone.DataValueField = "Value";
                ddlZone.DataTextField = "Text";
                ddlZone.DataBind();
                ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedZoneList = new List<int>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                if (selectedZoneList == null || selectedZoneList.Count == 0)
                {
                    lblMsg.Text = ErrorMessages.ZoneRequired;
                }

                var fromYear = Convert.ToString(ddlFromYear.SelectedValue);
                var fromMonth = Convert.ToString(ddlFromMonth.SelectedValue);
                var toYear = Convert.ToString(ddlToYear.SelectedValue);
                var toMonth = Convert.ToString(ddlToMonth.SelectedValue);
                var headId = Convert.ToInt32(ddlSingleHead.SelectedValue);


                if (headId > 0)
                {
                    var strZoneIds = ConvertListToString(selectedZoneList.ToArray());
                    GenerateReport(strZoneIds, headId, fromYear, fromMonth, toYear, toMonth);

                    lblMsg.Text = "";
                }
                else
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.MandatoryInputFailed);
                    rvHeadWiseSummaryReport.Reset();
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        private void GenerateReport(String zoneList, int headId, String fromYear, String fromMonth, String toYear, String toMonth)
        {

            #region Processing Report Data
            rvHeadWiseSummaryReport.Reset();

            var dsReportHeader = base.GetZoneInfoForReportHeader();
            var data = (from s in base._pgmContext.sp_PGM_GetHeadWiseSummary(zoneList, headId, fromYear, fromMonth, toYear, toMonth)
                        select s).ToList();
            
            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                lblMsg.Text = "";

                rvHeadWiseSummaryReport.ProcessingMode = ProcessingMode.Local;
                rvHeadWiseSummaryReport.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/HeadWiseSummaryRpt.rdlc");

                rvHeadWiseSummaryReport.LocalReport.DataSources.Add(new ReportDataSource("dsHeadWiseSummary", data));
                rvHeadWiseSummaryReport.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", dsReportHeader));

                var fromDate = Convert.ToDateTime("1" + "-" + fromMonth + "-" + fromYear);
                var toDate = Convert.ToDateTime(toMonth + "-" + toYear);
                string searchParameters = "For the Period " + " " + fromDate.ToString("MMM/yyyy") + " to " + toDate.ToString("MMM/yyyy");

                ReportParameter p1 = new ReportParameter("param", searchParameters);

                rvHeadWiseSummaryReport.LocalReport.SetParameters(new ReportParameter[] { p1 });
                rvHeadWiseSummaryReport.DataBind();

                //ExportToPDF
                String fileName = "HeadWiseSummaryReport_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvHeadWiseSummaryReport, fileName);
            }

            #endregion
        }

        protected void rvHeadWiseReportByMonthRange_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        private void PopulateDropdownList()
        {
            ddlFromMonth.DataSource = Common.PopulateMonthList();
            ddlFromMonth.DataValueField = "Text";
            ddlFromMonth.DataTextField = "Value";
            ddlFromMonth.DataBind();
            ddlFromMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;

            ddlFromYear.DataSource = Common.PopulateYearList();
            ddlFromYear.DataValueField = "Text";
            ddlFromYear.DataTextField = "Value";
            ddlFromYear.DataBind();

            ddlToMonth.DataSource = Common.PopulateMonthList();
            ddlToMonth.DataValueField = "Text";
            ddlToMonth.DataTextField = "Value";
            ddlToMonth.DataBind();
            ddlToMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;

            ddlToYear.DataSource = Common.PopulateYearList();
            ddlToYear.DataValueField = "Text";
            ddlToYear.DataTextField = "Value";
            ddlToYear.DataBind();

            var ddlHeadList = _pgmContext.PRM_SalaryHead.OrderBy(x => x.HeadName).ToList();
            ddlSingleHead.DataSource = Common.PopulateSalaryHeadDDL(ddlHeadList);
            ddlSingleHead.DataValueField = "Value";
            ddlSingleHead.DataTextField = "Text";
            ddlSingleHead.DataBind();
            ddlSingleHead.Items[0].Selected = true;
        }

    }
}