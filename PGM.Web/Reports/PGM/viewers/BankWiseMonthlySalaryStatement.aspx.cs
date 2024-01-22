using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.Services;
using System.Web.UI.WebControls;
using PGM.Web.Resources;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class BankWiseMonthlySalaryStatement : PGMReportBase
    {
        [NoCache]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId == 0) return;
                PopulateDropdownList();
            }
        }

        #region Button Event
        [NoCache]
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem.Text);

                var letterType = Convert.ToString(ddlSelectLetterType.SelectedValue);
                var bankId = Convert.ToInt32(ddlBankName.SelectedValue);
                var bankBranchId = Convert.ToInt32(ddlBranchName.SelectedValue);

                var bonusTypeId = Convert.ToInt32(ddlBonusType.SelectedValue);

                var zoneList = new List<FilteredZoneList>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        var obj = new FilteredZoneList(Convert.ToInt32(item.Value), item.Text);
                        zoneList.Add(obj);
                    }
                }

                if (zoneList == null || zoneList.Count == 0)
                {
                    lblMsg.Text = ErrorMessages.ZoneRequired;
                }
                else
                {
                    GenerateReport(selectedYear
                        , selectedMonth
                        , letterType
                        , bankId
                        , bankBranchId
                        , bonusTypeId
                        , zoneList);
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        #endregion

        [NoCache]
        private void GenerateReport(string selectedYear, string selectedMonth, string letterType, int? bankId, int? bankBranchId, int? bonusTypeId, List<FilteredZoneList> zoneList)
        {
            #region Processing Report Data

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));
            var reportHeader = base.GetZoneInfoForReportHeader();

            var data = (from s in base._pgmContext.PGM_SP_BankLetterDetailAddPayee(
                            selectedYear,
                            selectedMonth,
                            letterType,
                            bankId,
                            bankBranchId,
                            bonusTypeId,
                            numErrorCode, strErrorMsg)
                        select s)
                .ToList();

            data = data.Where(x => zoneList.Select(n => n.Id).Contains((int)x.SalaryWithdrawFromZoneId)).ToList();

            if (data.Count() > 0)
            {
                lblMsg.Text = "";
                rvAdviceLetterRpt.Reset();
                rvAdviceLetterRpt.ProcessingMode = ProcessingMode.Local;
                rvAdviceLetterRpt.LocalReport.ReportPath =
                    Server.MapPath("~/Reports/PGM/rdlc/BankWiseMonthlySalaryStatement.rdlc");
                rvAdviceLetterRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));
                rvAdviceLetterRpt.LocalReport.DataSources.Add(new ReportDataSource("DSBankWiseMonthlySalaryStatement",
                    data));

                string reportTitle = String.Concat(ddlSelectLetterType.SelectedItem.Text, " Statement for Bank").ToUpper();
                string searchParameters = String.Concat("For the month of ", ddlSelectMonth.SelectedItem.Text, "/", ddlSelectYear.SelectedItem.Text);
                string searchParameters1 = String.Concat("Name of Bank : ", ddlBankName.SelectedItem.Text, ", ",
                    ddlBranchName.SelectedItem.Text);

                ReportParameter pTitle = new ReportParameter("reportTitle", reportTitle);
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                ReportParameter p2 = new ReportParameter("bankInfo", searchParameters1);

                rvAdviceLetterRpt.LocalReport.SetParameters(new ReportParameter[] { p1, p2, pTitle });
            }
            else
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                rvAdviceLetterRpt.Reset();
            }
            rvAdviceLetterRpt.DataBind();

            //ExportToPDF
            String fileName = "BankWiseSalaryStatement_" + Guid.NewGuid() + ".pdf";
            ExportToPDF(rvAdviceLetterRpt, fileName);

            #endregion
        }

        [NoCache]
        protected void rvAdviceLetterRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Others
        [NoCache]
        private void PopulateDropdownList()
        {
            ddlSelectLetterType.DataSource = Common.PopulateLetterTypeList().OrderBy(x => x.Text).ToList();
            ddlSelectLetterType.DataValueField = "Value";
            ddlSelectLetterType.DataTextField = "Text";
            ddlSelectLetterType.DataBind();
            ddlSelectLetterType.Items.Insert(0, new ListItem() { Text = "[Select One]", Value = "0" });


            ddlBankName.DataSource = _pgmContext.acc_BankInformation.OrderBy(x => x.bankName).ToList();
            ddlBankName.DataValueField = "id";
            ddlBankName.DataTextField = "bankName";
            ddlBankName.DataBind();
            ddlBankName.Items.Insert(0, new ListItem("[All]", "0"));

            ddlBranchName.DataSource = _pgmContext.acc_BankBranchInformation.OrderBy(x => x.branchName).ToList();
            ddlBranchName.DataValueField = "id";
            ddlBranchName.DataTextField = "branchName";
            ddlBranchName.DataBind();
            ddlBranchName.Items.Insert(0, new ListItem("[All]", "0"));

            ddlBonusType.DataSource = _pgmContext.PGM_BonusType.OrderBy(x => x.BonusType).ToList();
            ddlBonusType.DataValueField = "Id";
            ddlBonusType.DataTextField = "BonusType";
            ddlBonusType.DataBind();
            ddlBonusType.Items.Insert(0, new ListItem("[All]", "0"));


            ddlSelectMonth.DataSource = Common.PopulateMonthList();
            ddlSelectMonth.DataValueField = "Text";
            ddlSelectMonth.DataTextField = "Value";
            ddlSelectMonth.DataBind();
            ddlSelectMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;


            ddlSelectYear.DataSource = Common.PopulateYearList();
            ddlSelectYear.DataValueField = "Text";
            ddlSelectYear.DataTextField = "Value";
            ddlSelectYear.DataBind();

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

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

        #endregion
    }
}