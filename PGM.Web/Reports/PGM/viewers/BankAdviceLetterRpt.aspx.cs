using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;


namespace PGM.Web.Reports.PGM.viewers
{
    public partial class BankAdviceLetterRpt : PGMReportBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #region Button Event
        string selectedYear = string.Empty;
        string selectedMonth = string.Empty;
        string letterType;
        int bankId;
        int bankBranchId;
        string bonusType;
        int zoneInfoId;

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = String.Empty;

                selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                selectedMonth = Convert.ToString(ddlSelectMonth.SelectedValue);
                letterType = Convert.ToString(ddlSelectLetterType.SelectedValue);
                bankId = Convert.ToInt32(ddlBankName.SelectedValue);
                bankBranchId = Convert.ToInt32(ddlBranchName.SelectedValue);
                bonusType = ddlBonusType.SelectedValue;
                zoneInfoId = Convert.ToInt32(ddlSelectZone.SelectedValue);

                GenerateReport(selectedYear
                    , selectedMonth
                    , letterType
                    , bankId
                    , bankBranchId
                    , bonusType
                    , zoneInfoId);
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }
        #endregion

        private void GenerateReport(string selectedYear, string selectedMonth, string letterType, int bankId, int bankBranchId, string bonusType, int zoneInfoId)
        {
            #region Processing Report Data
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = (from s in _pgmContext.sp_PGM_BankAdviceLetterMaster(letterType, selectedYear, selectedMonth, bankId, bankBranchId, bonusType, zoneInfoId, numErrorCode, strErrorMsg)
                        select s).ToList();

            rvAdviceLetterRpt.Reset();

            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                rvAdviceLetterRpt.ProcessingMode = ProcessingMode.Local;
                rvAdviceLetterRpt.LocalReport.ReportPath =
                    Server.MapPath("~/Reports/PGM/rdlc/BankAdviceLetterMasterRpt.rdlc");

                rvAdviceLetterRpt.LocalReport.DataSources.Add(new ReportDataSource("dsBankAdviceLetter", data));

                string searchParameters = Common.CurrentDateTime.Date.ToString("dd-MMM-yyyy");
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvAdviceLetterRpt.LocalReport.SetParameters(new ReportParameter[] {p1});

                this.rvAdviceLetterRpt.LocalReport.SubreportProcessing +=
                    new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvAdviceLetterRpt.DataBind();

                //ExportToPDF
                String fileName = "BankAdviceLetter_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvAdviceLetterRpt, fileName);
            }

            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = string.Empty;

            switch (e.ReportPath)
            {
                case "_ReportHeader4AdviceLetter":
                    data = base.GetZoneInfoForReportHeader();
                    dsName = "DSCompanyInfo";
                    break;

                default:
                    break;
            }
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        protected void rvAdviceLetterRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Others

        private static string NumberToText(int number)
        {
            if (number == 0) return "Zero";

            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";

            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }

            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };

            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };

            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };

            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }

            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens

                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");

                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        private void PopulateDropdownList()
        {
            ddlSelectLetterType.DataSource = Common.PopulateLetterTypeList().OrderBy(x => x.Text).ToList();
            ddlSelectLetterType.DataValueField = "Value";
            ddlSelectLetterType.DataTextField = "Text";
            ddlSelectLetterType.DataBind();
            ddlSelectLetterType.Items.Insert(0, new ListItem() { Text = "Select One", Value = "0" });


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

            ddlBonusType.DataSource = _pgmContext.PGM_BonusType.OrderBy(x => x.BonusType).ToList();
            ddlBonusType.DataValueField = "BonusType";
            ddlBonusType.DataTextField = "BonusType";
            ddlBonusType.DataBind();
            ddlBonusType.Items.Insert(0, new ListItem("All", ""));

            ddlSelectMonth.DataSource = Common.PopulateMonthList();
            ddlSelectMonth.DataValueField = "Text";
            ddlSelectMonth.DataTextField = "Value";
            ddlSelectMonth.DataBind();
            ddlSelectMonth.Items.FindByValue(DateTime.Now.ToString("MMMM")).Selected = true;


            ddlSelectYear.DataSource = Common.PopulateYearList();
            ddlSelectYear.DataValueField = "Text";
            ddlSelectYear.DataTextField = "Value";
            ddlSelectYear.DataBind();

            ddlSelectZone.DataSource = ZoneListCached;
            ddlSelectZone.DataValueField = "Value";
            ddlSelectZone.DataTextField = "Text";
            ddlSelectZone.DataBind();
            ddlSelectZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

        }

        protected void ddlSelectLetterType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void ddlSelectYear_OnSelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void ddlSelectMonth_OnSelectedIndexChanged(object sender, EventArgs e)
        {
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
        #endregion

    }
}