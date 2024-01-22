using Utility;
using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using PGM.Web.Resources;


namespace PGM.Web.Reports.PGM.viewers
{
    public partial class OvertimeReportStaffRpt : PGMReportBase
    {
        #region Properties

        private int GetDesignationId { get { return Convert.ToInt32(ddlDesignation.SelectedValue); } }
        private string GetDesignationName { get { return ddlDesignation.SelectedItem.Text; } }

        //private string GetEmpID { get { return txtIcNo.Text; } }
        private int GetYear { get { return Convert.ToInt32(ddlYear.SelectedValue); } }
        private int GetMonth { get { return Convert.ToInt32(ddlMonth.SelectedValue); } }

        private string GetMonthName
        {
            get
            {
                int month = Convert.ToInt32(ddlMonth.SelectedValue);
                return UtilCommon.GetMonthName(month);
            }
        }

        #endregion

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
            ddlDesignation.DataSource = _pgmContext.PRM_Designation.OrderBy(x => x.SortingOrder).ToList();
            ddlDesignation.DataValueField = "Id";
            ddlDesignation.DataTextField = "Name";
            ddlDesignation.DataBind();
            ddlDesignation.Items.Insert(0, new ListItem("All", "0"));

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

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                //string empID = GetEmpID;
                int desigId = GetDesignationId;
                int year = GetYear;
                int month = GetMonth;
                lblMsg.Text = String.Empty;

                var zoneList = new List<FilteredZoneList>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        var zone = new FilteredZoneList(Convert.ToInt32(item.Value), item.Text);
                        zoneList.Add(zone);
                    }
                }

                if (zoneList == null || zoneList.Count == 0)
                {
                    lblMsg.Text = ErrorMessages.ZoneRequired;
                }
                else
                {
                    GenerateReport(desigId, year, month, zoneList);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvOvertimeReportStaff_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        public void GenerateReport(int designationId, int year, int month, List<FilteredZoneList> zoneList)
        {
            #region Processing Report Data
            rvOvertimeReportStaff.Reset();
            var data = (from s in base._pgmContext.SP_PGM_RptOvertimeReportStaff(null, designationId, year, month) select s)
                .ToList();

            // Filter by ZoneIdDuringOvertime
            data = data.Where(x => zoneList.Select(n => n.Id).Contains((int)x.ZoneIdDuringOvertime)).ToList();

            string searchParamEmpID = string.Empty;
            string searchParameter = string.Empty;
            string searchParamYear = string.Empty;
            string searchParamMonth = string.Empty;

            #region Search parameter

            searchParamEmpID = "Statement of Overtime Allowance";
            searchParameter = "For the month of " + GetMonthName + "-" + GetYear;
            searchParamYear = ddlYear.SelectedItem.Text;
            searchParamMonth = GetMonthName;

            #endregion

            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                rvOvertimeReportStaff.ProcessingMode = ProcessingMode.Local;
                rvOvertimeReportStaff.LocalReport.ReportPath =
                    Server.MapPath("~/Reports/PGM/rdlc/OvertimeReportStaffRpt.rdlc");

                rvOvertimeReportStaff.LocalReport.DataSources.Add(new ReportDataSource("OvertimeReportStaffRpt", data));

                ReportParameter p1 = new ReportParameter("reportTitle", searchParamEmpID);
                ReportParameter p2 = new ReportParameter("Param", searchParameter);

                rvOvertimeReportStaff.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                this.rvOvertimeReportStaff.LocalReport.SubreportProcessing +=
                    new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvOvertimeReportStaff.DataBind();

                //ExportToPDF
                String fileName = "OvertimeReportForStuff_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvOvertimeReportStaff, fileName);
            }

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