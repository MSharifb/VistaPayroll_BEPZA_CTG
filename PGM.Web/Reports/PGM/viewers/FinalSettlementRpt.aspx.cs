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
    public partial class FinalSettlementRpt : PGMReportBase
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
                var employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());

                GenerateReport(employeeId);

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
        private void GenerateReport(int employeeId)
        {

            try
            {
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                var data = (from s in base._pgmContext.PGM_SP_GetFinalSettlementRpt(employeeId, numErrorCode, strErrorMsg) select s).ToList();

                if (data.Count() > 0)
                {
                    lblMsg.Text = "";

                    rvMyPaySlipRpt.Reset();
                    rvMyPaySlipRpt.ProcessingMode = ProcessingMode.Local;
                    rvMyPaySlipRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/FinalSettlementRpt.rdlc");

                    rvMyPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsFinalSettlement", data));
                }
                else
                {
                    status = true;
                    rvMyPaySlipRpt.Reset();
                }

                rvMyPaySlipRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvMyPaySlipRpt.DataBind();

                //ExportToPDF
                String fileName = "FinalSettlement_" + Guid.NewGuid() + ".pdf";
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
                    case "_PGMZoneWiseReportHeader":
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
            var empList = GetEmployeeList();
            if (empList != null)
            {
                ddlEmployee.DataSource = empList;
                ddlEmployee.DataValueField = "Id";
                ddlEmployee.DataTextField = "DisplayText";
                ddlEmployee.DataBind();
                ddlEmployee.Items.Insert(0, new ListItem("All", "0"));
            }
        }

        #endregion


    }
}