using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Objects;
using System.Linq;
using Utility;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class IncomeTaxComputationSheetViewer : PGMReportBase
    {
        bool checkStatus = false;
        static int id=default(int);
        static string financialYear = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (LoggedUserZoneInfoId == 0) return;

                if (!this.IsPostBack)
                {
                    string idFinancialYear = Request.QueryString["idYearMonth"];
                    if (!string.IsNullOrEmpty(idFinancialYear))
                    {
                        string[] idFinancialYearArrary = idFinancialYear.Split(',');
                        id = Convert.ToInt32(idFinancialYearArrary[0]);
                        financialYear = idFinancialYearArrary[1];

                        GenerateReport(id, financialYear);

                        lblMsg.Text = "";

                        if (checkStatus == true)
                        {
                            lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        private void GenerateReport(int selectedEmployeeID, string selectIncomeYear)
        {
            #region Processing Report Data
            lblMsg.Text = "";
            rvIncomeTaxComputationRpt.Reset();
            rvIncomeTaxComputationRpt.ProcessingMode = ProcessingMode.Local;
            rvIncomeTaxComputationRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/IncomeTaxComputationRpt.rdlc");

            var reportHeader = base.GetZoneInfoForReportHeader();

            var data = _pgmContext.vwPGMIncomeTaxComputations.Where(x => x.EmployeeId == selectedEmployeeID && x.IncomeYear == selectIncomeYear).OrderBy(x => x.EmpID).ToList();

            if (data.Count() > 0)
            {
                rvIncomeTaxComputationRpt.LocalReport.DataSources.Add(new ReportDataSource("dsIncomeTaxComputation", data));
                rvIncomeTaxComputationRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));
                string searchParameters = "For the Income Year " + " " + selectIncomeYear.ToString();

                ReportParameter p1 = new ReportParameter("param", searchParameters);

                rvIncomeTaxComputationRpt.LocalReport.SetParameters(new ReportParameter[] { p1 });

                this.rvIncomeTaxComputationRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);

            }
            else
            {
                checkStatus = true;
                rvIncomeTaxComputationRpt.Reset();

            }

            rvIncomeTaxComputationRpt.DataBind();

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var selectedEmployeeID =id;
            var selectIncomeYear = Convert.ToString(financialYear);

            dynamic data = null;
            var dsName = string.Empty;


            switch (e.ReportPath)
            {
                case "_IncomeTaxableSalary":
                    data = (from c in _pgmContext.GetTaxableIncomeSalary(selectIncomeYear, selectedEmployeeID, numErrorCode, strErrorMsg)
                            select c).ToList();

                    dsName = "DSIncomeTaxableSalary";
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;

                case "_IncomeTaxationSlabs":
                    data = (from c in _pgmContext.vwPGMIncomeTaxationSlabs.Where(x => x.EmployeeId == selectedEmployeeID && x.IncomeYear == selectIncomeYear)
                            select c).ToList();

                    dsName = "dsTaxationSlab";
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;

                case "_IncomeTaxUptoPreviousMonth":
                    data = (from c in _pgmContext.sp_PGM_GetPreviousMonthIncomeTax(selectedEmployeeID, selectIncomeYear)
                            select c).ToList();

                    dsName = "dsPreviousMonthTax";
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;

                case "_IncomeAdvanceTaxPaid":
                    var advanceTaxPaidEntityType = Convert.ToByte(PGMEnum.TaxEntityType.AdvanceTaxPaid);
                    data = (from investDetail in _pgmContext.PGM_TaxOtherInvestAndAdvPaidDetail
                            join invest in _pgmContext.PGM_TaxOtherInvestAndAdvPaid on investDetail.EntityId equals invest.Id
                            where invest.EntityType == advanceTaxPaidEntityType
                                    && invest.EmployeeId == selectedEmployeeID
                                    && invest.IncomeYear == selectIncomeYear
                            select new { investDetail.EntityDescription, investDetail.EntityAmount }).ToList();

                    dsName = "DSAdvanceTaxPaid";
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;

                case "_IncomeOtherTaxInvestment":
                    data = (from c in _pgmContext.PGM_FN_GetActualTaxInvestments(selectedEmployeeID, selectIncomeYear)
                            select c).ToList();

                    dsName = "DSAdvanceTaxPaid";
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;
            }

        }
        
        protected void rvIncomeTaxComputationRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Page_Load(null, null);
        }

        protected void btnBack_Click(object sender, System.EventArgs e)
        {
            string[] years = financialYear.Split('-');
            string assessmentYear = (Convert.ToInt32(years[0]) + 1) + "-" + (Convert.ToInt32(years[1]) + 1);
            string idYearMonth = financialYear + "," + assessmentYear;
            Response.Redirect("~/PGM/IncomeTaxComputation/IncomeTaxDetail?idYearMonth=" + idYearMonth);
        }
    }
}