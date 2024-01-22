using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;


namespace PGM.Web.Reports.PGM.viewers
{
    public partial class BankWiseMonthlyElectricityBillRpt : PGMReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #region User Methods
        private void PopulateDropdownList()
        {

            ddlBank.DataSource = _pgmContext.acc_BankInformation.OrderBy(x => x.bankName).ToList();
            ddlBank.DataValueField = "id";
            ddlBank.DataTextField = "bankName";
            ddlBank.DataBind();
            ddlBank.Items.Insert(0, new ListItem("All", "0"));

            ddlBranch.DataSource = _pgmContext.acc_BankBranchInformation.OrderBy(x => x.branchName).ToList();
            ddlBranch.DataValueField = "id";
            ddlBranch.DataTextField = "branchName";
            ddlBranch.DataBind();
            ddlBranch.Items.Insert(0, new ListItem("All", "0"));

            ddlDepartment.DataSource = _pgmContext.PRM_Division.OrderBy(x => x.SortOrder).ToList();
            ddlDepartment.DataValueField = "Id";
            ddlDepartment.DataTextField = "Name";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("All", "0"));

            ddlMonth.DataSource = Common.PopulateMonthList();
            ddlMonth.DataValueField = "Text";
            ddlMonth.DataTextField = "Value";
            ddlMonth.DataBind();
            ddlMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;


            ddlYear.DataSource = Common.PopulateYearList();
            ddlYear.DataValueField = "Text";
            ddlYear.DataTextField = "Value";
            ddlYear.DataBind();


            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }

        #endregion

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateReport(Convert.ToInt32(ddlBank.SelectedValue)
                    , Convert.ToInt32(ddlBranch.SelectedValue)
                    , Convert.ToInt32(ddlDepartment.SelectedValue)
                    , ddlYear.SelectedValue.ToString()
                    , ddlMonth.SelectedValue.ToString()
                    , txtIcNo.Text.ToString());
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvBankWiseMonthlyElectricityBill_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        public void GenerateReport(int bankId, int branchId, int departmentId, string year, string month, string icNo)
        {
            #region Processing Report Data

            var data = (from s in base._pgmContext.SP_PGM_RptBankWiseMonthlyElectricityBill(bankId, branchId, departmentId, month, year, icNo) select s).OrderBy(x => x.FullName).ToList();

            string searchParameters1 = string.Empty;
            string searchParameters2 = string.Empty;
            string searchParameters3 = string.Empty;
            string searchParameters4 = string.Empty;
            string searchParameters5 = string.Empty;
            string searchParameters6 = string.Empty;

            #region Search parameter

            if (bankId > 0)
            {
                searchParameters1 = ddlBank.SelectedItem.Text;
            }
            else
            {
                searchParameters1 = "All";
            }

            if (branchId > 0)
            {
                searchParameters2 = ddlBranch.SelectedItem.Text;
            }
            else
            {
                searchParameters2 = "All";
            }

            if (departmentId > 0)
            {
                searchParameters3 = ddlDepartment.SelectedItem.Text;
            }
            else
            {
                searchParameters3 = "All";
            }
            searchParameters4 = ddlMonth.SelectedItem.Text;
            searchParameters5 = ddlYear.SelectedItem.Text;

            if (icNo != "")
            {
                searchParameters6 = icNo;
            }

            #endregion

            lblMsg.Text = "";
            rvBankWiseMonthlyElectricityBill.Reset();
            rvBankWiseMonthlyElectricityBill.ProcessingMode = ProcessingMode.Local;
            rvBankWiseMonthlyElectricityBill.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/BankWiseMonthlyElectricityBillRpt.rdlc");

            rvBankWiseMonthlyElectricityBill.LocalReport.DataSources.Add(new ReportDataSource("BankWiseMonthlyElectricityBillRpt", data));

            ReportParameter p1 = new ReportParameter("Bank", searchParameters1);
            ReportParameter p2 = new ReportParameter("Branch", searchParameters2);
            ReportParameter p3 = new ReportParameter("Department", searchParameters3);
            ReportParameter p4 = new ReportParameter("Month", searchParameters4);
            ReportParameter p5 = new ReportParameter("Year", searchParameters5);
            ReportParameter p6 = new ReportParameter("EmpID", searchParameters6);

            rvBankWiseMonthlyElectricityBill.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });
            this.rvBankWiseMonthlyElectricityBill.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvBankWiseMonthlyElectricityBill.DataBind();

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = base.GetZoneInfoForReportHeader();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        [WebMethod]
        public  ArrayList GetBranchByBankId(int Id)
        {
            ArrayList list = new ArrayList();
            if (Id > 0)
            {
                var items = (from bBranch in base._pgmContext.acc_BankBranchInformation
                             where bBranch.bankId == Id
                             select new
                             {
                                 Id = bBranch.id,
                                 Name = bBranch.branchName
                             }).ToList();
                foreach (var item in items)
                {
                    list.Add(item);
                }

            }
            else
            {
                var items = (from bBranch in base._pgmContext.acc_BankBranchInformation
                             select new
                             {
                                 Id = bBranch.id,
                                 Name = bBranch.branchName
                             }).ToList();
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
            return list;
        }

    }
}