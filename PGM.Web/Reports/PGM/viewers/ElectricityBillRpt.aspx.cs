using DAL.PGM;
using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class ElectricityBillRpt : PGMReportBase
    {
        #region Fields

        private readonly PGM_GenericRepository<PGM_ElectricBill> _ElectricBillRepository;
        #endregion

        #region Ctor

        public ElectricityBillRpt()
        {
            _ElectricBillRepository = new PGM_GenericRepository<PGM_ElectricBill>(new PGMEntities());
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId == 0) return;

                PopulateDropdownList();
            }
        }

        private void PopulateDropdownList()
        {

            ddlDepartment.DataSource = GetDepartment();
            ddlDepartment.DataValueField = "Id";
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("All", "0"));

            ddlMonth.DataSource = Common.PopulateMonthList();
            ddlMonth.DataValueField = "Text";
            ddlMonth.DataTextField = "Value";
            ddlMonth.DataBind();

            ddlYear.DataSource = Common.PopulateYearList();
            ddlYear.DataValueField = "Text";
            ddlYear.DataTextField = "Value";
            ddlYear.DataBind();

            ddlZone.DataSource = GetZoneDDL();
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

        public List<PRM_Division> GetDepartment()
        {
            var list = _pgmContext.PRM_Division.OrderBy(x => x.Name).ToList();
            return list;
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(Convert.ToInt32(ddlDepartment.SelectedValue)
                    , ddlYear.SelectedValue.ToString()
                    , ddlMonth.SelectedValue.ToString()
                    , txtIcNo.Text.ToString()
                    , txtMeterNo.Text.ToString());
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvElectricBill_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        public void GenerateReport(int departmentId, string year, string month, string icNo, string meterNo)
        {
            #region Processing Report Data


            var data = (from s in base._pgmContext.sp_PGM_RptElectrictBill(departmentId, month, year, icNo, meterNo) select s).OrderBy(x => x.EmployeeName).ToList();

            string searchParameters1 = string.Empty;
            string searchParameters2 = string.Empty;
            string searchParameters3 = string.Empty;
            string searchParameters4 = string.Empty;
            string searchParameters5 = string.Empty;
            string searchParameters6 = string.Empty;

            #region Search parameter


            if (departmentId > 0)
            {
                string department = ddlDepartment.SelectedItem.Text;
                searchParameters1 = department;
            }
            else
            {
                searchParameters1 = "All";
            }
            searchParameters2 = ddlMonth.SelectedItem.Text;
            searchParameters3 = ddlYear.SelectedItem.Text;

            if (icNo != "")
            {
                searchParameters4 = icNo;
            }
            searchParameters5 = meterNo;

            #endregion

            lblMsg.Text = "";
            rvElectricBill.Reset();
            rvElectricBill.ProcessingMode = ProcessingMode.Local;
            rvElectricBill.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/ElectricBillRpt.rdlc");

            rvElectricBill.LocalReport.DataSources.Add(new ReportDataSource("ElectricBillRpt", data));

            ReportParameter p1 = new ReportParameter("department", searchParameters1);
            ReportParameter p2 = new ReportParameter("Month", searchParameters2);
            ReportParameter p3 = new ReportParameter("Year", searchParameters3);
            ReportParameter p4 = new ReportParameter("ICNo", searchParameters4);
            ReportParameter p5 = new ReportParameter("MeterNo", searchParameters5);


            rvElectricBill.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5 });


            this.rvElectricBill.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvElectricBill.DataBind();

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