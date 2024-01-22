using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Utility;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class IncomeTaxComputationRpt : PGMReportBase
    {
        bool checkStatus = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (LoggedUserZoneInfoId == 0) return;

                if (!this.IsPostBack)
                {
                    PopulateDDL();
                }
            }
            catch (Exception ex)
            {
                this.lblErrmsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {

            try
            {
                if (ddlIncomeYear.SelectedIndex > 0)
                {

                    int selectedEmployeeID = default(int);
                    {
                        var empInitial = txtEmployeeInitial.Text.Trim().ToString();
                        selectedEmployeeID = (from p in _pgmExecuteFunctions.GetEmployeeList()
                                              where p.EmpID.Trim().ToLower() == empInitial.ToLower()
                                              select p.Id).FirstOrDefault();
                    }

                    var selectIncomeYear = Convert.ToString(ddlIncomeYear.SelectedValue);
                    GenerateReport(selectedEmployeeID, selectIncomeYear);
                    lblMsg.Text = "";
                    lblErrmsg.Text = "";

                    if (checkStatus == true)
                    {
                        lblErrmsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                    }
                }
                else
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.MandatoryInputFailed);
                    lblErrmsg.Text = "";
                    rvIncomeTaxComputationRpt.Reset();
                }
            }
            catch (Exception ex)
            {
                lblErrmsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
            finally
            {
                lblLoadingMessage.Visible = false;
            }
        }

        private void GenerateReport(int selectedEmployeeID, string selectIncomeYear)
        {
            #region Processing Report Data


            lblMsg.Text = string.Empty;
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

            //ExportToPDF
            String fileName = "IncomeTaxComputation_" + Guid.NewGuid() + ".pdf";
            ExportToPDF(rvIncomeTaxComputationRpt, fileName);

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                int selectedEmployeeID = default(int);
                if (txtEmployeeInitial.Text != string.Empty)
                {
                    var empInitial = txtEmployeeInitial.Text.Trim().ToString();
                    selectedEmployeeID = (from p in _pgmExecuteFunctions.GetEmployeeList()
                                          where p.EmpID.Trim().ToLower() == empInitial.ToLower()
                                          select p.Id).FirstOrDefault();
                }

                var selectIncomeYear = Convert.ToString(ddlIncomeYear.SelectedValue);
                string[] fYear = selectIncomeYear.Split('-');

                var fromDateIncomeYear = Convert.ToDateTime("1" + "-" + "July-" + fYear[0]);
                var toDateIncomeYear = Convert.ToDateTime("30" + "-" + "June-" + fYear[1]);

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
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvIncomeTaxComputationRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                btnViewReport_Click(null, null);
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        private void PopulateDDL()
        {
            ddlIncomeYear.Items.Clear();

            ddlIncomeYear.Items.Insert(0, new ListItem() { Text = "Select Income Year", Value = "0" });
            int k = 1;
            for (int i = DateTime.Now.Year + 1; i >= 2000; i--)
            {
                var iIncomeYear = (i - 1) + "-" + i;

                ddlIncomeYear.Items.Insert(k, new ListItem() { Value = iIncomeYear.ToString(), Text = iIncomeYear.ToString() });
                k++;
            }
        }

    }
}