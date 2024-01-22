using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class LeaveEncashmentRpt : PGMReportBase
    {
        #region Properties

        #endregion

        bool status = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedUserZoneInfoId == 0) return;

            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #region Button Event
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> zoneList = new List<int>();
                int[] arrZoneList = new int[] { };
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        zoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                arrZoneList = zoneList.ToArray();

                string strZoneId = ConvertListToString(arrZoneList);

                var fromYear = Convert.ToString(ddlFromYear.SelectedValue);
                var fromMonth = Convert.ToString(ddlFromMonth.SelectedItem);
                var toYear = Convert.ToString(ddlToYear.SelectedValue);
                var toMonth = Convert.ToString(ddlToMonth.SelectedItem);
                var employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());


                GenerateReport(fromYear
                    , fromMonth
                    , toYear
                    , toMonth
                    , employeeId
                    , strZoneId);

                lblMsg.Text = default(string);
                if (status == true)
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
                status = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
        #endregion

        [NoCache]
        private void GenerateReport(string fromYear, string fromMonth, string toYear, string toMonth, int employeeId, string zoneList)
        {
            try
            {
                var data = (from s in base._pgmContext.PGM_SP_LeaveEncashmentRpt(fromYear, fromMonth, toYear, toMonth, employeeId, zoneList) select s).ToList();

                if (data.Count() > 0)
                {
                    lblMsg.Text = "";

                    rvMyPaySlipRpt.Reset();
                    rvMyPaySlipRpt.ProcessingMode = ProcessingMode.Local;
                    rvMyPaySlipRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/LeaveEncashmentRpt.rdlc");

                    rvMyPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsLeaveEncashment", data));

                    string parameterLine1 = "";
                    string parameterLine2 = "";

                    if (!string.IsNullOrEmpty(fromYear) && fromMonth != "All" && !string.IsNullOrEmpty(toYear) && toMonth != "All")
                    {
                        parameterLine1 = String.Concat("From the month of ", fromMonth, "/", fromYear, " to ", toMonth, "/", toYear);
                    }
                    else if (!string.IsNullOrEmpty(fromYear) && !string.IsNullOrEmpty(fromMonth))
                    {
                        parameterLine1 = String.Concat("From the month of ", fromMonth, "/", fromYear);
                    }
                    else if (employeeId != 0)
                    {
                        parameterLine1 = String.Concat("All Leave Encashment for ", ddlEmployee.SelectedItem.Text);
                    }
                    else
                    {
                        parameterLine1 = "Leave Encashment of All Time";
                    }

                    ReportParameter p1 = new ReportParameter("Param1", parameterLine1);
                    ReportParameter p2 = new ReportParameter("Param2", parameterLine2);

                    rvMyPaySlipRpt.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                }
                else
                {
                    status = true;
                    rvMyPaySlipRpt.Reset();
                }

                rvMyPaySlipRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvMyPaySlipRpt.DataBind();

                //ExportToPDF
                String fileName = "LeaveEncashment_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvMyPaySlipRpt, fileName);
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }

        }

        [NoCache]
        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                dynamic data = null;
                var dsName = string.Empty;
                switch (e.ReportPath)
                {
                    case "_ReportHeader4AdviceLetter":
                        data = base.GetZoneInfoForReportHeader();
                        dsName = "DSCompanyInfo";
                        break;

                    default:
                        break;
                }
                e.DataSources.Add(new ReportDataSource(dsName, data));
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvMyPaySlipRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Others


        [NoCache]
        private void PopulateDropdownList()
        {
            int j = 0;
            foreach (var year in Common.PopulateYearList().ToList())
            {
                ddlFromYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });
                ddlToYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });

                j++;
            }
            ddlFromYear.Items.Insert(0, new ListItem("All", ""));
            ddlToYear.Items.Insert(0, new ListItem("All", ""));


            int k = 0;
            foreach (var month in Common.PopulateMonthListForReport().ToList())
            {
                ddlFromMonth.Items.Insert(k, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });
                ddlToMonth.Items.Insert(k, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });

                k++;
            }
            ddlFromMonth.Items.Insert(0, new ListItem("All", ""));
            ddlToMonth.Items.Insert(0, new ListItem("All", ""));

            //ddlFromMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;
            //ddlToMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;


            HashSet<int> zoneIDs = new HashSet<int>(MyAppSession.SelectedZoneList.Select(s => s.Id));

            ddlEmployee.DataSource = _pgmExecuteFunctions.GetEmployeeList().Select(q => new
            {
                ZoneInfoId = q.SalaryWithdrawFromZoneId,
                EmpID = q.EmpID,
                Id = q.Id,
                DisplayText = q.FullName + " [" + q.EmpID + " ]"
            })
              .Where(x => zoneIDs.Contains(Common.GetInteger(x.ZoneInfoId)))
              .OrderBy(x => x.DisplayText)
              .ToList();
            ddlEmployee.DataValueField = "Id";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));
        }

        #endregion

    }
}