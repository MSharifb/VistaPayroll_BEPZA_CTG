
using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using PGM.Web.Resources;

//*********************
// Repeating TABLIX header on every page -
// https://stackoverflow.com/questions/20699635/rdlc-tablix-column-header-not-repeating-on-every-page-repeat-column-header-on-e
//*********************

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class MonthlySalaryRpt : PGMReportBase
    {

        [NoCache]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId == 0) return;

                PopulateDropdownList();
            }
        }

        #region Button Event

        [NoCache]
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = String.Empty;

                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                var jobGrade = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
                //var divisionId = Convert.ToInt32(ddlSelectDivision.SelectedValue);
                var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
                var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);
                var bankId = Convert.ToInt32(ddlBankName.SelectedValue);
                var branchId = Convert.ToInt32(ddlBranchName.SelectedValue);
                var categoryId = Convert.ToInt32(ddlEmpCategory.SelectedValue);
                var reportId = Convert.ToInt32(DdlReportType.SelectedValue);

                var IsWithheldStatmentChecked = chkWithheldStatment.Checked;

                var selectedZoneList = new List<int>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                var strZoneIds = ConvertListToString(selectedZoneList.ToArray());

                if (selectedZoneList == null || selectedZoneList.Count == 0)
                {
                    lblMsg.Text = ErrorMessages.ZoneRequired;
                }
                else
                {
                    GenerateReport(
                        selectedYear
                        , selectedMonth
                        , designationId
                        , empTypeId
                        , jobGrade
                        , IsWithheldStatmentChecked
                        , bankId
                        , branchId
                        , categoryId
                        , strZoneIds
                        , reportId);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        #endregion

        private void GenerateReport(string selectedYear, string selectedMonth, int designationId, int empTypeId, int jobGrade, bool IsWithheldStatmentChecked, int bankId, int branchId, int categoryId, String zoneList, int reportId)
        {
            #region Processing Report Data
            rvMonthlySalaryRpt.Reset();
            rvMonthlySalaryRpt.ProcessingMode = ProcessingMode.Local;

            var dsReportHeader = base.GetZoneInfoForReportHeader();
            var data = _pgmExecuteFunctions.GetMonthlySalaryStatement(selectedYear, selectedMonth, designationId, jobGrade, empTypeId, bankId, branchId, categoryId, zoneList);

            //if (designationId > 0) data = data.Where(q => q.DesignationId == designationId).ToList();
            //if (jobGrade > 0) data = data.Where(q => q.GradeId == jobGrade).ToList();
            //if (empTypeId > 0) data = data.Where(q => q.EmploymentId == empTypeId).OrderBy(o => o.SortPriority).ToList();
            //if (bankId > 0) data = data.Where(q => q.BankId == bankId).ToList();
            //if (branchId > 0) data = data.Where(q => q.BranchId == branchId).ToList();
            //if (categoryId > 0) data = data.Where(q => q.StaffCategoryId == categoryId).ToList();
            string searchParameters = string.Empty;

            if (IsWithheldStatmentChecked)
            {
                data = data.Where(q => q.IsOnceWithheld == 1).ToList();
                searchParameters = "Withheld salary for the month of" + " " + selectedMonth.Substring(0, 3) + "/" + selectedYear.ToString() + "";
            }
            else
            {
                data = data.Where(q => q.IsOnceWithheld == 0).ToList();
                searchParameters = "For the month of" + " " + selectedMonth.Substring(0, 3) + "-" + selectedYear.ToString() + "";
            }

            String fileName = string.Empty;

            if (reportId == 1)
            {
                data = data.Where(x => (x.IsOtherAddition == false && x.IsOtherDeduction == false) || (x.IsGrossSalaryHead || x.IsNetPayableHead || x.IsTotalDeductionHead || x.IsOtherAdditionTotalHead || x.IsOtherDeductionTotalHead)).ToList();

                if (data.Count() > 0)
                {
                    rvMonthlySalaryRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/MonthlySalaryRpt.rdlc");

                    rvMonthlySalaryRpt.LocalReport.DataSources.Add(new ReportDataSource("DSMonthlySalary", data));
                    rvMonthlySalaryRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", dsReportHeader));

                    ReportParameter p1 = new ReportParameter("param", searchParameters);
                    ReportParameter p2 = new ReportParameter("ReportParameterList", searchParameters);
                    rvMonthlySalaryRpt.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                    fileName = "MonthlySalaryStatementRpt_" + Guid.NewGuid() + ".pdf";
                }
                else
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    return;
                }
            }
            else if (reportId == 2)
            {
                data = data.Where(x => x.IsOtherAddition == true || x.IsOtherAdditionTotalHead == true).ToList();
                if (data.Count() > 0)
                {
                    rvMonthlySalaryRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/OtherAllowanceDetailsRpt.rdlc");

                    rvMonthlySalaryRpt.LocalReport.DataSources.Add(new ReportDataSource("DSMonthlySalary", data));
                    rvMonthlySalaryRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", dsReportHeader));

                    ReportParameter p1 = new ReportParameter("param", searchParameters);
                    ReportParameter p2 = new ReportParameter("ReportParameterList", searchParameters);
                    rvMonthlySalaryRpt.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                    fileName = "OtherAllowanceDetailRpt_" + Guid.NewGuid() + ".pdf";
                }
                else
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    return;
                }
            }
            else if (reportId == 3)
            {
                data = data.Where(x => x.IsOtherDeduction == true || x.IsOtherDeductionTotalHead == true).ToList();
                if (data.Count() > 0)
                {
                    rvMonthlySalaryRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/OtherDeductionDetailsRpt.rdlc");

                    rvMonthlySalaryRpt.LocalReport.DataSources.Add(new ReportDataSource("DSMonthlySalary", data));
                    rvMonthlySalaryRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", dsReportHeader));

                    ReportParameter p1 = new ReportParameter("param", searchParameters);
                    ReportParameter p2 = new ReportParameter("ReportParameterList", searchParameters);
                    rvMonthlySalaryRpt.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
                    fileName = "OtherDeductionDetailRpt_" + Guid.NewGuid() + ".pdf";
                }
                else
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    return;
                }
            }


            this.rvMonthlySalaryRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvMonthlySalaryRpt.DataBind();

            //ExportToPDF
            if (data.Count() > 0 && !String.IsNullOrEmpty(fileName))
            {
                ExportToPDF(rvMonthlySalaryRpt, fileName);
            }

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //dynamic data = null;
            //var dsName = "DSCompanyInfo";
            //data = base.GetZoneInfoForReportHeader();
            //e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        protected void rvMonthlySalaryRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Method

        [NoCache]
        private void PopulateDropdownList()
        {
            int j = 0;
            foreach (var year in Common.PopulateYearList().ToList())
            {
                ddlSelectYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });
                j++;
            }

            int k = 0;
            foreach (var month in Common.PopulateMonthListForReport().ToList())
            {
                ddlSelectMonth.Items.Insert(k, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });
                k++;
            }
            ddlSelectMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;


            ddlSelectEmployeeType.DataSource = _pgmContext.PRM_EmploymentType
                .OrderBy(x => x.Name).ToList();
            ddlSelectEmployeeType.DataValueField = "Id";
            ddlSelectEmployeeType.DataTextField = "Name";
            ddlSelectEmployeeType.DataBind();
            ddlSelectEmployeeType.Items.Insert(0, new ListItem("All", "0"));

            ddlSelectDesignation.DataSource = _pgmContext.PRM_Designation
                .OrderBy(x => x.Name).ToList();
            ddlSelectDesignation.DataValueField = "Id";
            ddlSelectDesignation.DataTextField = "Name";
            ddlSelectDesignation.DataBind();
            ddlSelectDesignation.Items.Insert(0, new ListItem("All", "0"));

            ddlEmpCategory.DataSource = _pgmContext.PRM_StaffCategory.OrderBy(x => x.Name).ToList();
            ddlEmpCategory.DataValueField = "Id";
            ddlEmpCategory.DataTextField = "Name";
            ddlEmpCategory.DataBind();
            ddlEmpCategory.Items.Insert(0, new ListItem("All", "0"));

            DateTime cDate = DateTime.Now;
            var prm_SalaryScaleEntity = _pgmContext.PRM_SalaryScale
                .Where(t => t.DateOfEffective <= cDate)
                .OrderByDescending(t => t.DateOfEffective).First();

            ddlSelectJobGrade.DataSource = _pgmContext.PRM_JobGrade
                .Where(t => t.SalaryScaleId == prm_SalaryScaleEntity.Id)
                .ToList();
            ddlSelectJobGrade.DataValueField = "Id";
            ddlSelectJobGrade.DataTextField = "GradeName";
            ddlSelectJobGrade.DataBind();
            ddlSelectJobGrade.Items.Insert(0, new ListItem("All", "0"));

            ddlBankName.DataSource = _pgmContext.acc_BankInformation
                .OrderBy(x => x.bankName).ToList();
            ddlBankName.DataValueField = "id";
            ddlBankName.DataTextField = "bankName";
            ddlBankName.DataBind();
            ddlBankName.Items.Insert(0, new ListItem("All", "0"));

            ddlBranchName.DataSource = _pgmContext.acc_BankBranchInformation
                .OrderBy(x => x.branchName).ToList();
            ddlBranchName.DataValueField = "id";
            ddlBranchName.DataTextField = "branchName";
            ddlBranchName.DataBind();
            ddlBranchName.Items.Insert(0, new ListItem("All", "0"));


            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

        [NoCache]
        protected void ddlBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlBranchName.Items.Clear();
            var id = Convert.ToInt32(ddlBankName.SelectedValue);
            if (id > 0)
            {
                ddlBranchName.DataSource = _pgmContext.acc_BankBranchInformation
                    .Where(x => x.bankId == id).OrderBy(x => x.branchName).ToList();
                ddlBranchName.DataValueField = "id";
                ddlBranchName.DataTextField = "branchName";
                ddlBranchName.DataBind();
                ddlBranchName.Items.Insert(0, new ListItem("All", "0"));
            }
            else
            {
                ddlBranchName.DataSource = _pgmContext.acc_BankBranchInformation
                    .OrderBy(x => x.branchName).ToList();
                ddlBranchName.DataValueField = "id";
                ddlBranchName.DataTextField = "branchName";
                ddlBranchName.DataBind();
                ddlBranchName.Items.Insert(0, new ListItem("All", "0"));
            }
        }

        #endregion
    }

}