using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class YearlySalaryStatementRpt : PGMReportBase
    {
        bool status = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (LoggedUserZoneInfoId == 0) return;

                if (!this.IsPostBack)
                {
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
                lblMsg.Text = String.Empty;
                Errmsg.Text = String.Empty;

                var selectedFinancialYear = Convert.ToString(ddlSelectFinancialYear.SelectedValue);
                var jobGrade = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
                var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
                var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);

                int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());

                if (employeeId == 0)
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.MandatoryInputFailed);
                    rvYearlySalaryStatementRpt.Reset();
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedFinancialYear) && ddlSelectFinancialYear.SelectedIndex > 0)
                    {
                        GenerateReport(selectedFinancialYear
                            , 0
                            , designationId
                            , empTypeId
                            , employeeId);

                        if (status == true)
                        {
                            lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                        }
                        status = false;
                    }
                    else
                    {
                        lblMsg.Text = Common.GetCommomMessage(CommonMessage.MandatoryInputFailed);
                        Errmsg.Text = "";
                        rvYearlySalaryStatementRpt.Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void GenerateReport(string selectedFinancialYear, int divisionId, int designationId, int empTypeId, int employeeId)
        {
            #region Processing Report Data
            rvYearlySalaryStatementRpt.Reset();

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = _pgmContext.GetSalaryPaySheetMaster(selectedFinancialYear, divisionId, designationId, empTypeId, employeeId, numErrorCode, strErrorMsg)
                .DistinctBy(x => x.EmpID)
                .ToList();

            var reportHeader = base.GetZoneInfoForReportHeader();

            if (data.Count() > 0 && selectedFinancialYear != null)
            {
                lblMsg.Text = "";
                rvYearlySalaryStatementRpt.ProcessingMode = ProcessingMode.Local;
                rvYearlySalaryStatementRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/YearlySalaryStatementRpt.rdlc");

                rvYearlySalaryStatementRpt.LocalReport.DataSources.Add(new ReportDataSource("dsSalaryPaySheetMaster", data));
                rvYearlySalaryStatementRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));

                string searchParameters = "For the Financial Year " + " " + selectedFinancialYear.ToString();
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvYearlySalaryStatementRpt.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }
            else
            {
                status = true;
                rvYearlySalaryStatementRpt.Reset();
            }

            this.rvYearlySalaryStatementRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvYearlySalaryStatementRpt.DataBind();

            //ExportToPDF
            String fileName = "YearlySalaryStatement_" + Guid.NewGuid() + ".pdf";
            ExportToPDF(rvYearlySalaryStatementRpt, fileName);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                dynamic data = null;
                var dsName = string.Empty;
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                var selectedFinancialYear = Convert.ToString(ddlSelectFinancialYear.SelectedValue);
                var jobGrade = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
                var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
                var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);

                int EmployeeID = Convert.ToInt32(e.Parameters[0].Values[0]);

                switch (e.ReportPath)
                {
                    case "_SalaryPaySheet":

                        e.DataSources.Clear();
                        data = _pgmContext.GetSalaryPaySheetDetail(selectedFinancialYear, 0, designationId, empTypeId, EmployeeID, numErrorCode, strErrorMsg).ToList();
                        dsName = "dsSalaryPaySheetDetail";
                        e.DataSources.Add(new ReportDataSource(dsName, data));
                        break;

                    case "_SalaryPaySheetTaxable":

                        e.DataSources.Clear();
                        data = _pgmContext.sp_PGM_GetTaxableSalaryPaySheetDetail(selectedFinancialYear, EmployeeID, numErrorCode, strErrorMsg).ToList();
                        dsName = "dsSalaryPaySheetTaxable";
                        e.DataSources.Add(new ReportDataSource(dsName, data));
                        break;

                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvYearlySalaryStatementRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        private void PopulateDropdownList()
        {
            ddlSelectFinancialYear.Items.Insert(0, new ListItem() { Text = "Select Financial Year", Value = "0" });
            int j = 1;
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                var iFormatYear = i + "-" + (i + 1);
                ddlSelectFinancialYear.Items.Insert(j, new ListItem() { Text = iFormatYear.ToString(), Value = iFormatYear.ToString() });
                j++;
            }

            ddlSelectEmployeeType.DataSource = _pgmContext.PRM_EmploymentType.OrderBy(x => x.Name).ToList();
            ddlSelectEmployeeType.DataValueField = "Id";
            ddlSelectEmployeeType.DataTextField = "Name";
            ddlSelectEmployeeType.DataBind();
            ddlSelectEmployeeType.Items.Insert(0, new ListItem("All", "0"));

            ddlSelectDesignation.DataSource = _pgmContext.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlSelectDesignation.DataValueField = "Id";
            ddlSelectDesignation.DataTextField = "Name";
            ddlSelectDesignation.DataBind();
            ddlSelectDesignation.Items.Insert(0, new ListItem("All", "0"));

            DateTime cDate = DateTime.Now;
            var prm_SalaryScaleEntity = _pgmContext.PRM_SalaryScale.Where(t => t.DateOfEffective <= cDate).OrderByDescending(t => t.DateOfEffective).First();

            ddlSelectJobGrade.DataSource = _pgmContext.PRM_JobGrade.Where(t => t.SalaryScaleId == prm_SalaryScaleEntity.Id).ToList();
            ddlSelectJobGrade.DataValueField = "Id";
            ddlSelectJobGrade.DataTextField = "GradeName";
            ddlSelectJobGrade.DataBind();
            ddlSelectJobGrade.Items.Insert(0, new ListItem("All", "0"));

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