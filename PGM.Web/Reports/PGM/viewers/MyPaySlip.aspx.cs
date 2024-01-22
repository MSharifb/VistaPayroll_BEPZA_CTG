using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utility;

/*
		Author	: Mostafizur Rahaman
		Date	: 2017-May-31
		---------
*/

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class MyPaySlip : PGMReportBase
    {
        #region Properties

        #endregion

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
                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                string userId = User.Identity.Name;
                lblMsg.Text = String.Empty;

                var loginUser = _pgmExecuteFunctions.GetEmployeeList().Where(emp => emp.EmpID == userId).ToList().FirstOrDefault();

                GenerateReport(selectedYear, selectedMonth, loginUser.Id);
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
        #endregion

        [NoCache]
        private void GenerateReport(string selectedYear, string selectedMonth, int emplyeeId)
        {
            try
            {
                rvMyPaySlipRpt.Reset();

                var data = _pgmExecuteFunctions.GetMonthlySalaryPayslip(selectedYear, selectedMonth, null, emplyeeId);
                var reportHeader = base.GetZoneInfoForReportHeader();
                
                if (!data.Any())
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
                else
                {
                    rvMyPaySlipRpt.ProcessingMode = ProcessingMode.Local;
                    rvMyPaySlipRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/SalaryPaySlipDetailRpt.rdlc");

                    rvMyPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsSalaryPaySlip", data));
                    rvMyPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));

                    string parameterLine2 = "";

                    if (data[0].WithheldPaymentDate == null)
                    {
                        parameterLine2 = String.Concat("For the month of ", selectedMonth, "/", selectedYear);
                    }
                    else
                    {
                        DateTime WithheldPaymentDate = Convert.ToDateTime(data[0].WithheldPaymentDate);
                        parameterLine2 = String.Concat("Withheld salary for the month of "
                                            , selectedMonth
                                            , "/"
                                            , selectedYear
                                            , ","
                                            , " paid in "
                                            , UtilCommon.GetMonthName(WithheldPaymentDate.Month)
                                            , "/"
                                            , WithheldPaymentDate.Year);
                    }

                    ReportParameter p2 = new ReportParameter("param", parameterLine2);
                    rvMyPaySlipRpt.LocalReport.SetParameters(new ReportParameter[] { p2 });
                    
                    rvMyPaySlipRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                    rvMyPaySlipRpt.DataBind();

                    //ExportToPDF
                    String fileName = "MyPaySlip_" + Guid.NewGuid() + ".pdf";
                    ExportToPDF(rvMyPaySlipRpt, fileName);
                }
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
                var dsName = string.Empty;
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);

                switch (e.ReportPath)
                {
                    case "_PGMZoneWiseReportHeader":

                        dsName = "DSCompanyInfo";
                        var data = base.GetZoneInfoForReportHeader();
                        e.DataSources.Add(new ReportDataSource(dsName, data));
                        break;

                    case "_SalaryPaySlipDetailAddition":

                        int EmpID = Convert.ToInt32(e.Parameters[0].Values[0]);
                        var dataAddition = (from ad in base._pgmContext.sp_PGM_GetSalaryPaySlipDetailAdditionExtended(EmpID, selectedYear, selectedMonth, numErrorCode, strErrorMsg) select ad).OrderBy(x => x.SortPriority).ToList();

                        dsName = "dsSalaryPaySlipDetailAddition";
                        e.DataSources.Add(new ReportDataSource(dsName, dataAddition));

                        break;

                    case "_SalaryPaySlipDetailDeduction":

                        EmpID = Convert.ToInt32(e.Parameters[0].Values[0]);
                        var dataDeduction = (from ad in base._pgmContext.sp_PGM_GetSalaryPaySlipDetailDeductionExtended(EmpID, selectedYear, selectedMonth, numErrorCode, strErrorMsg) select ad).OrderBy(x => x.SortPriority).ToList();

                        dsName = "dsSalaryPaySlipDetailDeduction";
                        e.DataSources.Add(new ReportDataSource(dsName, dataDeduction));
                        break;
                }

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

        }

        #endregion

    }


}