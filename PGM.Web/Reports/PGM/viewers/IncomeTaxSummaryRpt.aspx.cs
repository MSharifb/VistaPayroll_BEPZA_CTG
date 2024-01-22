using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Web.UI.WebControls;
using PGM.Web.Resources;
using ObjectParameter = System.Data.Objects.ObjectParameter;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class IncomeTaxSummaryRpt : PGMReportBase
    {
        bool status = false;
        bool incomeStatus = false;
        bool selectionStatus = false;
        bool checkStatus = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    if (LoggedUserZoneInfoId == 0) return;

                    PopulateDropdownList();
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedIncomeYear = Convert.ToString(ddlSelectIncomeYear.SelectedValue);

                if (ddlSelectIncomeYear.SelectedIndex > 0)
                {
                    var zoneList = new List<FilteredZoneList>();
                    foreach (ListItem item in ddlZone.Items)
                    {
                        if (item.Selected)
                        {
                            var obj = new FilteredZoneList(Convert.ToInt32(item.Value), item.Text);
                            zoneList.Add(obj);
                        }
                    }

                    if (zoneList == null || zoneList.Count == 0)
                    {
                        lblMsg.Text = ErrorMessages.ZoneRequired;
                    }
                    else
                    {
                        int? employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());

                        GenerateReport(selectedIncomeYear, zoneList, employeeId);
                    }

                    lblMsg.Text = "";
                    lblErrmsg.Text = "";
                    if (checkStatus == true)
                    {
                        lblErrmsg.Text = " 'From Date' must be greater than 'To Date'.Please try again.";
                    }
                    if (status == true)
                    {
                        lblErrmsg.Text = "Date range must be within selected Income Year correctly.";
                    }
                    status = false;

                    if (incomeStatus == true)
                    {
                        lblErrmsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    }
                    incomeStatus = false;

                    if (selectionStatus == true)
                    {
                        lblErrmsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    }
                    selectionStatus = false;
                }
                else
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.MandatoryInputFailed);
                    lblErrmsg.Text = "";
                    rvIncomeTaxSummaryRt.Reset();
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        private void GenerateReport(string selectedIncomeYear, List<FilteredZoneList> zoneList, int? employeeId)
        {
            string[] fYear = selectedIncomeYear.Split('-');

            var fromDateIncomeYear = Convert.ToDateTime("1" + "-" + "July-" + fYear[0]);
            var toDateIncomeYear = Convert.ToDateTime("30" + "-" + "June-" + fYear[1]);

            #region Processing Report Data

            var reportHeader = base.GetZoneInfoForReportHeader();

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var summaries = (from s in base._pgmContext.sp_PGM_GetIncomeTaxSummary(numErrorCode, strErrorMsg)
                             select s).ToList();

            // Filter by SalaryWithdrawFromZoneId
            summaries = summaries.Where(x => zoneList.Select(n => n.Id).Contains((int)x.SalaryWithdrawFromZoneId)).ToList();


            if (employeeId != null && employeeId > 0)
                summaries = summaries.Where(s => s.EmployeeId == employeeId).ToList();


            if (summaries != null)
            {
                try
                {
                    var data = (from s in summaries
                                where s.SalaryDate >= fromDateIncomeYear && s.SalaryDate <= toDateIncomeYear
                                select s);

                    if (data != null)
                    {
                        lblMsg.Text = "";
                        rvIncomeTaxSummaryRt.Reset();
                        rvIncomeTaxSummaryRt.ProcessingMode = ProcessingMode.Local;

                        if (employeeId != null && employeeId > 0)
                        {
                            rvIncomeTaxSummaryRt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/IncomeTaxSummaryRptByEmployee.rdlc");
                        }
                        else
                        {
                            rvIncomeTaxSummaryRt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/IncomeTaxSummaryRpt.rdlc");
                        }

                        rvIncomeTaxSummaryRt.LocalReport.DataSources.Add(new ReportDataSource("dsIncomeTaxSummaryRpt", data));
                        rvIncomeTaxSummaryRt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));

                        string searchParameters = "For the Financial Year " + " " + selectedIncomeYear.ToString();
                        ReportParameter p1 = new ReportParameter("param", searchParameters);

                        rvIncomeTaxSummaryRt.LocalReport.SetParameters(new ReportParameter[] { p1 });
                    }
                    else
                    {
                        selectionStatus = true;
                        rvIncomeTaxSummaryRt.Reset();
                    }
                }
                catch (NullReferenceException ex)
                {
                    selectionStatus = true;
                    lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                }
            }

            rvIncomeTaxSummaryRt.DataBind();

            //ExportToPDF
            String fileName = "IncomeTaxSummary_" + Guid.NewGuid() + ".pdf";
            ExportToPDF(rvIncomeTaxSummaryRt, fileName);

            #endregion
        }

        protected void rvIncomeTaxSummaryRt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        private void PopulateDropdownList()
        {
            ddlSelectIncomeYear.Items.Insert(0, new ListItem() { Text = "[Select Income Year]", Value = "0" });
            int j = 1;
            for (int i = DateTime.Now.Year + 1; i >= 2000; i--)
            {
                var iFormatYear = (i - 1) + "-" + i;
                ddlSelectIncomeYear.Items.Insert(j, new ListItem() { Text = iFormatYear.ToString(), Value = iFormatYear.ToString() });
                j++;
            }


            // Populate zone form session. It is not possible to do this with other thread except UI thread.
            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            var empList = GetEmployeeList();
            if (empList != null)
            {
                ddlEmployee.DataSource = empList;
                ddlEmployee.DataValueField = "Id";
                ddlEmployee.DataTextField = "DisplayText";
                ddlEmployee.DataBind();
                ddlEmployee.Items.Insert(0, new ListItem("", "0"));
            }
        }

    }
}