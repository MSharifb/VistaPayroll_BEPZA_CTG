using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class BankWiseIncentiveBonus : PGMReportBase
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
            ddlReportType.Items.Insert(0, new ListItem("Select", "0"));
            ddlReportType.Items.Insert(1, new ListItem("Bank", "Bank"));
            ddlReportType.Items.Insert(2, new ListItem("Department", "Department"));
            ddlReportType.Items.Insert(3, new ListItem("Employee", "Employee"));

            ddlFinancialYear.DataSource = _pgmContext.PGM_IncentiveBonus.DistinctBy(x => x.FinancialYear).ToList();
            ddlFinancialYear.DataValueField = "FinancialYear";
            ddlFinancialYear.DataTextField = "FinancialYear";
            ddlFinancialYear.DataBind();
            ddlFinancialYear.Items.Insert(0, new ListItem("Select", "0"));

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

            ddlEmployeeStaff.DataSource = _pgmContext.PRM_StaffCategory.OrderBy(x => x.Name).ToList();
            ddlEmployeeStaff.DataValueField = "Id";
            ddlEmployeeStaff.DataTextField = "Name";
            ddlEmployeeStaff.DataBind();
            ddlEmployeeStaff.Items.Insert(0, new ListItem("All", "0"));

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
                GenerateReport(ddlReportType.SelectedValue.ToString()
                    , ddlFinancialYear.SelectedValue.ToString()
                    , Convert.ToInt32(ddlBank.SelectedValue)
                    , Convert.ToInt32(ddlBranch.SelectedValue)
                    , Convert.ToInt32(ddlDepartment.SelectedValue)
                    , Convert.ToInt32(ddlEmployeeStaff.SelectedValue));
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvBankWiseIncentiveBonus_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }


        public void GenerateReport(string retportType, string financialYear, int bankId, int branchId, int departmentId, int employeeStaffId)
        {
            #region Processing Report Data

            string searchParameters1 = string.Empty;
            string searchParameters2 = string.Empty;
            string searchParameters3 = string.Empty;
            string searchParameters4 = string.Empty;
            string searchParameters5 = string.Empty;
            string searchParameters6 = string.Empty;

            #region Search parameter

            if (retportType != "")
            {
                searchParameters1 = retportType;
            }

            if (financialYear != "")
            {
                searchParameters2 = financialYear;
            }

            if (bankId > 0)
            {
                searchParameters3 = ddlBank.SelectedItem.Text;
            }
            else
            {
                searchParameters3 = "All";
            }

            if (branchId > 0)
            {
                searchParameters4 = ddlBranch.SelectedItem.Text;
            }
            else
            {
                searchParameters4 = "All";
            }

            if (departmentId > 0)
            {
                searchParameters5 = ddlDepartment.SelectedItem.Text;
            }
            else
            {
                searchParameters5 = "All";
            }

            if (employeeStaffId > 0)
            {
                searchParameters6 = ddlEmployeeStaff.SelectedItem.Text;
            }
            else
            {
                searchParameters6 = "All";
            }



            #endregion

            lblMsg.Text = "";
            rvBankWiseIncentiveBonus.Reset();
            rvBankWiseIncentiveBonus.ProcessingMode = ProcessingMode.Local;
            if (retportType == "Bank")
            {
                var data = (from s in base._pgmContext.PGM_SP_BankWiseIncentiveBonus(retportType, financialYear, bankId, branchId, departmentId, employeeStaffId) select s).ToList();
                rvBankWiseIncentiveBonus.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/BankWiseIncentiveBonus.rdlc");
                rvBankWiseIncentiveBonus.LocalReport.DataSources.Add(new ReportDataSource("BankWiseIncentiveBonus", data));
            }
            else if (retportType == "Department")
            {
                var data2 = (from s in base._pgmContext.PGM_SP_DepartmentWiseIncentiveBonus(retportType, financialYear, bankId, branchId, departmentId, employeeStaffId) select s).ToList();
                rvBankWiseIncentiveBonus.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/DepartmentWiseIncentiveBonus.rdlc");
                rvBankWiseIncentiveBonus.LocalReport.DataSources.Add(new ReportDataSource("DepartmentWiseIncentiveBonus", data2));
            }
            else
            {
                var data3 = (from s in base._pgmContext.PGM_SP_EmployeeWiseIncentiveBonus(retportType, financialYear, bankId, branchId, departmentId, employeeStaffId) select s).ToList();
                rvBankWiseIncentiveBonus.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/EmployeeWiseIncentiveBonus.rdlc");
                rvBankWiseIncentiveBonus.LocalReport.DataSources.Add(new ReportDataSource("EmployeeWiseIncentiveBonus", data3));
            }


            ReportParameter p1 = new ReportParameter("ReportType", searchParameters1);
            ReportParameter p2 = new ReportParameter("FinancialYear", searchParameters2);
            ReportParameter p4 = new ReportParameter("Bank", searchParameters3);
            ReportParameter p5 = new ReportParameter("Branch", searchParameters4);
            ReportParameter p3 = new ReportParameter("Department", searchParameters5);
            ReportParameter p6 = new ReportParameter("StaffCategory", searchParameters6);

            rvBankWiseIncentiveBonus.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });
            this.rvBankWiseIncentiveBonus.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvBankWiseIncentiveBonus.DataBind();
            ExportPDF();

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
        public ArrayList GetBranchByBankId(int Id)
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

        public bool ExportPDF()
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;


                byte[] bytes = rvBankWiseIncentiveBonus.LocalReport.Render(
                                "PDF", null, out mimeType, out encoding, out filenameExtension,
                                out streamids, out warnings);

                rvBankWiseIncentiveBonus.ServerReport.DisplayName = "Bank WIse Incentive Bonus";
                rvBankWiseIncentiveBonus.LocalReport.DisplayName = "Bank WIse Incentive Bonus";

                String fileName = "IncentiveBonus_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvBankWiseIncentiveBonus, fileName);
            }

            catch (Exception) { }

            return true;

        }

    }
}