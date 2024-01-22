using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using PGM.Web.Resources;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class BonusStatementRpt : PGMReportBase
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId == 0) return;

                await PopulateDropdownList();

                // Populate zone form session. It is not possible to do this with other thread except UI thread.
                ddlZone.DataSource = ZoneListCached;
                ddlZone.DataValueField = "Value";
                ddlZone.DataTextField = "Text";
                ddlZone.DataBind();
                ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
            }
        }

        #region User Methods
        private async Task PopulateDropdownList()
        {

            await Task.Run(() =>
            {
                ddlBank.DataSource = _pgmContext.acc_BankInformation.OrderBy(x => x.bankName).ToList();
                ddlBank.DataValueField = "id";
                ddlBank.DataTextField = "bankName";
                ddlBank.DataBind();
                ddlBank.Items.Insert(0, new ListItem("All", "0"));
            }).ConfigureAwait(false);


            await Task.Run(() =>
            {
                ddlBranch.DataSource = _pgmContext.acc_BankBranchInformation.OrderBy(x => x.branchName).ToList();
                ddlBranch.DataValueField = "id";
                ddlBranch.DataTextField = "branchName";
                ddlBranch.DataBind();
                ddlBranch.Items.Insert(0, new ListItem("All", "0"));
            }).ConfigureAwait(false);


            await Task.Run(() =>
            {
                ddlDesignation.DataSource = _pgmContext.PRM_Designation.OrderBy(x => x.Name).ToList();
                ddlDesignation.DataValueField = "Id";
                ddlDesignation.DataTextField = "Name";
                ddlDesignation.DataBind();
                ddlDesignation.Items.Insert(0, new ListItem("All", "0"));
            }).ConfigureAwait(false);

            await Task.Run(() =>
            {
                ddlEmployeeStaff.DataSource = _pgmContext.PRM_StaffCategory.OrderBy(x => x.Name).ToList();
                ddlEmployeeStaff.DataValueField = "Id";
                ddlEmployeeStaff.DataTextField = "Name";
                ddlEmployeeStaff.DataBind();
                ddlEmployeeStaff.Items.Insert(0, new ListItem("All", "0"));
            }).ConfigureAwait(false);

            await Task.Run(() =>
            {
                ddlMonth.DataSource = Common.PopulateMonthList();
                ddlMonth.DataValueField = "Text";
                ddlMonth.DataTextField = "Value";
                ddlMonth.DataBind();
                ddlMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;
            }).ConfigureAwait(false);

            await Task.Run(() =>
            {
                ddlYear.DataSource = Common.PopulateYearList();
                ddlYear.DataValueField = "Text";
                ddlYear.DataTextField = "Value";
                ddlYear.DataBind();
            }).ConfigureAwait(false);

            await Task.Run(() =>
            {
                ddlBonusType.DataSource = _pgmContext.PGM_BonusType.OrderBy(x => x.BonusType).ToList();
                ddlBonusType.DataValueField = "Id";
                ddlBonusType.DataTextField = "BonusType";
                ddlBonusType.DataBind();
                ddlBonusType.Items.Insert(0, new ListItem("[All]", "0"));
            }).ConfigureAwait(false);
        }

        #endregion
        [NoCache]
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = String.Empty;

                var selectedZoneList = new List<int>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneList.Add(Convert.ToInt32(item.Value));
                    }
                }

                var strZoneIds = ConvertListToString(selectedZoneList.ToArray());
                var bonusTypeId = Convert.ToInt32(ddlBonusType.SelectedValue);

                if (selectedZoneList == null || selectedZoneList.Count == 0)
                {
                    lblMsg.Text = ErrorMessages.ZoneRequired;
                }
                else
                {
                    GenerateReport(
                        Convert.ToInt32(ddlBank.SelectedValue)
                        , Convert.ToInt32(ddlBranch.SelectedValue)
                        , Convert.ToInt32(ddlDesignation.SelectedValue)
                        , ddlYear.SelectedValue.ToString()
                        , ddlMonth.SelectedValue.ToString()
                        , Convert.ToInt32(ddlEmployeeStaff.SelectedValue)
                        , strZoneIds, bonusTypeId);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }
        [NoCache]
        protected void rvBonusStatement_ReportRefresh(object sender, CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
        [NoCache]
        public void GenerateReport(int bankId, int branchId, int designationId, string selectedYear, string selectedMonth, int staffCategoryId, String zoneList, int bonusTypeId)
        {
            try
            {
                #region Processing Report Data
                rvBonusStatement.Reset();

                var dsReportHeader = base.GetZoneInfoForReportHeader();
                var data = (from s in
                                base._pgmContext.PGM_SP_BonusStatement(selectedYear,
                                                                        selectedMonth,
                                                                        bankId,
                                                                        branchId,
                                                                        designationId,
                                                                        0,
                                                                        zoneList,
                                                                        bonusTypeId)
                            select s).ToList();

                if (!data.Any())
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
                else
                {
                    rvBonusStatement.ProcessingMode = ProcessingMode.Local;
                    rvBonusStatement.LocalReport.ReportPath =
                        Server.MapPath("~/Reports/PGM/rdlc/BonusStatementReport.rdlc");

                    rvBonusStatement.LocalReport.DataSources.Add(new ReportDataSource("dsBonusStatement", data));
                    rvBonusStatement.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", dsReportHeader));

                    string reportTitle = "BONUS STATEMENT";
                    string parameterLine2 = "For the month of" + " " + selectedMonth.Substring(0, 3) + "-" + selectedYear.ToString() + "";

                    ReportParameter pTitle = new ReportParameter("reportTitle", reportTitle);
                    ReportParameter p2 = new ReportParameter("param", parameterLine2);
                    rvBonusStatement.LocalReport.SetParameters(new ReportParameter[] { p2, pTitle });

                    rvBonusStatement.DataBind();

                    //ExportToPDF
                    var fileName = "BonusStatement_" + Guid.NewGuid() + ".pdf";
                    if (data.Count() > 0 && !String.IsNullOrEmpty(fileName))
                    {
                        ExportToPDF(rvBonusStatement, fileName);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }


        [NoCache]
        protected void ddlBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlBranch.Items.Clear();
            var id = Convert.ToInt32(ddlBank.SelectedValue);
            if (id > 0)
            {
                ddlBranch.DataSource = _pgmContext.acc_BankBranchInformation.Where(x => x.bankId == id).OrderBy(x => x.branchName).ToList();
                ddlBranch.DataValueField = "id";
                ddlBranch.DataTextField = "branchName";
                ddlBranch.DataBind();
                ddlBranch.Items.Insert(0, new ListItem("All", "0"));
            }
            else
            {
                ddlBranch.DataSource = _pgmContext.acc_BankBranchInformation.OrderBy(x => x.branchName).ToList();
                ddlBranch.DataValueField = "id";
                ddlBranch.DataTextField = "branchName";
                ddlBranch.DataBind();
                ddlBranch.Items.Insert(0, new ListItem("All", "0"));
            }
        }


    }
}