
using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class MonthlyPayrollExpSheetRpt : PGMReportBase
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
                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                var employeeCategory = Convert.ToInt32(ddlEmployeeCategory.SelectedValue);
                var divisionId = Convert.ToInt32(ddlSelectDivision.SelectedValue);
                var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
                var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);


                lblMsg.Text = String.Empty;
                GenerateReport(selectedYear
                    , selectedMonth
                    , divisionId
                    , designationId
                    , empTypeId
                    , employeeCategory);
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        private void GenerateReport(string selectedYear, string selectedMonth, int divisionId, int designationId, int empTypeId, int empCategoryId)
        {
            #region Processing Report Data
            rvMonthlyPayrollExpSheetRpt.Reset();

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = (from s in base._pgmContext.sp_PGM_GetMonthlyPayrollExpSheet(selectedYear, selectedMonth, divisionId, designationId, empTypeId, empCategoryId, numErrorCode, strErrorMsg) select s)
                .OrderBy(x => x.DepartmentName)
                .ToList();

            string searchParameters = "For the month of" + " " + selectedMonth.Substring(0, 3) + "/" + selectedYear.ToString() + "";

            if (data.Count() > 0)
            {
                lblMsg.Text = "";
                rvMonthlyPayrollExpSheetRpt.Reset();
                rvMonthlyPayrollExpSheetRpt.ProcessingMode = ProcessingMode.Local;
                rvMonthlyPayrollExpSheetRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/MonthlyPayrollExpSheetRpt.rdlc");

                rvMonthlyPayrollExpSheetRpt.LocalReport.DataSources.Add(new ReportDataSource("MonthlyPayrollExpSheet", data));

                ReportParameter p2 = new ReportParameter("ReportTitle", "Monthly Payroll Expenditure Sheet");
                rvMonthlyPayrollExpSheetRpt.LocalReport.SetParameters(new ReportParameter[] { p2, new ReportParameter("ParamLbl1","Year"), new ReportParameter("ParamLbl2","Month"), new ReportParameter("ParamLbl3","Department"),
                    new ReportParameter("ParamLbl4","Employee Category"), new ReportParameter("ParamLbl5","Designation"), new ReportParameter("ParamLbl6","Employment Type"),
                new ReportParameter("ParamVal1",selectedYear), new ReportParameter("ParamVal2",selectedMonth), new ReportParameter("ParamVal3",ddlSelectDivision.SelectedItem.Text)
                , new ReportParameter("ParamVal4",ddlEmployeeCategory.SelectedItem.Text) , new ReportParameter("ParamVal5",ddlSelectDesignation.SelectedItem.Text)
                , new ReportParameter("ParamVal6",ddlSelectEmployeeType.SelectedItem.Text)});

                this.rvMonthlyPayrollExpSheetRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvMonthlyPayrollExpSheetRpt.DataBind();

                //ExportToPDF
                String fileName = "PayrollExpenditureSheet_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvMonthlyPayrollExpSheetRpt, fileName);
            }
            else
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);

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

        protected void rvMonthlyPayrollExpSheetRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

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


            ddlSelectDivision.DataSource = _pgmContext.PRM_Division.OrderBy(x => x.Name).ToList();
            ddlSelectDivision.DataValueField = "Id";
            ddlSelectDivision.DataTextField = "Name";
            ddlSelectDivision.DataBind();
            ddlSelectDivision.Items.Insert(0, new ListItem("All", "0"));

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

            ddlEmployeeCategory.DataSource = _pgmContext.PRM_StaffCategory.OrderBy(x => x.SortOrder).ToList();
            ddlEmployeeCategory.DataValueField = "Id";
            ddlEmployeeCategory.DataTextField = "Name";
            ddlEmployeeCategory.DataBind();
            ddlEmployeeCategory.Items.Insert(0, new ListItem("All", "0"));

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

    }
}