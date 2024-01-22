using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class HeadWiseReport : PGMReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId != 0)
                {
                    PopulateDropdownList();
                }
            }
        }

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedYear = ddlYear.SelectedValue.ToString();
                var selectedMonth = ddlMonth.SelectedValue.ToString();

                var selectedZoneList = new List<int>();                
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneList.Add(Convert.ToInt32(item.Value));
                    }
                }
                var strZoneIds = ConvertListToString<int>(selectedZoneList.ToArray());

                var selectedHeadList = new List<int>();
                var selectedHeadListText = new List<String>();
                foreach (ListItem item in ddlHead.Items)
                {
                    if (item.Selected)
                    {
                        selectedHeadList.Add(Convert.ToInt32(item.Value));
                        selectedHeadListText.Add(item.Text);
                    }
                }
                var strHeadIds = ConvertListToString<int>(selectedHeadList.ToArray());
                var strReportTitle = "Statement of " + ConvertListToString(selectedHeadListText.ToArray()) + " Head(s)";
                GenerateReport(strHeadIds
                    , selectedYear
                    , selectedMonth
                    , Convert.ToInt32(ddlEmployeeStaff.SelectedValue)
                    , Convert.ToInt32(ddlBankName.SelectedValue)
                    , Convert.ToInt32(ddlBranchName.SelectedValue)
                    , strZoneIds
                    , strReportTitle);
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        protected void rvHeadWiseReport_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        public void GenerateReport(string headList, string fromYear, string fromMonth, int staffCategoryId, int bankId, int branchId, string zoneList, string strReportTitle)
        {
            #region Processing Report Data
            rvHeadWiseReport.Reset();

            var reportHeader = base.GetZoneInfoForReportHeader();
            var data = (from s in base._pgmContext.PGM_SP_HeadWiseReport(fromYear, fromMonth, headList, staffCategoryId, bankId, branchId, zoneList) select s)
                .ToList();

            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                rvHeadWiseReport.ProcessingMode = ProcessingMode.Local;
                if (headList.Split(',').Count() == 1)
                {
                    rvHeadWiseReport.LocalReport.ReportPath =
                        Server.MapPath("~/Reports/PGM/rdlc/HeadWiseReport_1Head.rdlc");
                }
                else if (headList.Split(',').Count() == 2)
                {
                    rvHeadWiseReport.LocalReport.ReportPath =
                        Server.MapPath("~/Reports/PGM/rdlc/HeadWiseReport_2Heads.rdlc");
                }
                else if (headList.Split(',').Count() == 3)
                {
                    rvHeadWiseReport.LocalReport.ReportPath =
                        Server.MapPath("~/Reports/PGM/rdlc/HeadWiseReport_3Heads.rdlc");
                }
                else
                {
                    rvHeadWiseReport.LocalReport.ReportPath =
                        Server.MapPath("~/Reports/PGM/rdlc/HeadWiseReport.rdlc");
                }

                rvHeadWiseReport.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));
                rvHeadWiseReport.LocalReport.DataSources.Add(new ReportDataSource("DSHeadWiseReport", data));

                string reportTitle = strReportTitle;
                var searchParameters1 = "For the month of " + fromYear + "/" + fromMonth;

                if (!String.IsNullOrEmpty(ddlBankName.SelectedItem.Text))
                {
                    searchParameters1 = String.Concat(searchParameters1, " [Name of Bank : ",
                        ddlBankName.SelectedItem.Text, ", ",
                        ddlBranchName.SelectedItem.Text, "]");
                }
                ReportParameter pTitle = new ReportParameter("reportTitle", reportTitle);
                ReportParameter p1 = new ReportParameter("param", searchParameters1);

                rvHeadWiseReport.LocalReport.SetParameters(new ReportParameter[] {p1, pTitle});

                rvHeadWiseReport.DataBind();

                //ExportToPDF
                String fileName = "HeadWiseReport_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvHeadWiseReport, fileName);
            }

            #endregion
        }


        protected void ddlBankName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlBranchName.Items.Clear();
            var id = Convert.ToInt32(ddlBankName.SelectedValue);
            if (id > 0)
            {
                ddlBranchName.DataSource = _pgmContext.acc_BankBranchInformation.Where(x => x.bankId == id).OrderBy(x => x.branchName).ToList();
                ddlBranchName.DataValueField = "id";
                ddlBranchName.DataTextField = "branchName";
                ddlBranchName.DataBind();
                ddlBranchName.Items.Insert(0, new ListItem("All", "0"));
            }
            else
            {
                ddlBranchName.DataSource = _pgmContext.acc_BankBranchInformation.OrderBy(x => x.branchName).ToList();
                ddlBranchName.DataValueField = "id";
                ddlBranchName.DataTextField = "branchName";
                ddlBranchName.DataBind();
                ddlBranchName.Items.Insert(0, new ListItem("All", "0"));
            }

        }


        #region User Methods
        private void PopulateDropdownList()
        {
            var ddlHeadList = _pgmContext.PRM_SalaryHead.OrderBy(x => x.HeadName).ToList();
            ddlHead.DataSource = Common.PopulateSalaryHeadDDL(ddlHeadList);
            ddlHead.DataValueField = "Value";
            ddlHead.DataTextField = "Text";
            ddlHead.DataBind();
            ddlHead.Items[0].Selected = true;

            ddlEmployeeStaff.DataSource = _pgmContext.PRM_StaffCategory.OrderBy(x => x.Name).ToList();
            ddlEmployeeStaff.DataValueField = "Id";
            ddlEmployeeStaff.DataTextField = "Name";
            ddlEmployeeStaff.DataBind();
            ddlEmployeeStaff.Items.Insert(0, new ListItem("All", "0"));

            ddlMonth.DataSource = Common.PopulateMonthList();
            ddlMonth.DataValueField = "Text";
            ddlMonth.DataTextField = "Value";
            ddlMonth.DataBind();
            ddlMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;

            ddlYear.DataSource = Common.PopulateYearList();
            ddlYear.DataValueField = "Text";
            ddlYear.DataTextField = "Value";
            ddlYear.DataBind();

            ddlBankName.DataSource = _pgmContext.acc_BankInformation.OrderBy(x => x.bankName).ToList();
            ddlBankName.DataValueField = "id";
            ddlBankName.DataTextField = "bankName";
            ddlBankName.DataBind();
            ddlBankName.Items.Insert(0, new ListItem("All", "0"));

            ddlBranchName.DataSource = _pgmContext.acc_BankBranchInformation.OrderBy(x => x.branchName).ToList();
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

        #endregion
    }
}