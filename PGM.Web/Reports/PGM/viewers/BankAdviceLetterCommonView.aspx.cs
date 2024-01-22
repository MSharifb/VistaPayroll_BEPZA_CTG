using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Objects;
using System.Linq;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class BankAdviceLetterCommonView : PGMReportBase
    {
        static int id = default(int);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    var requestId = Request.QueryString["id"];
                    int num1;
                    lblMsg.Text = String.Empty;

                    if (int.TryParse(requestId, out num1))
                    {
                        id = Convert.ToInt32(requestId);

                        var searchId = (from s in _pgmContext.PGM_BankLetter.Where(p => p.Id == id) select s).FirstOrDefault();
                        if (searchId != null)
                        {
                            GenerateReport(searchId.LetterType
                                , searchId.SalaryYear
                                , searchId.SalaryMonth
                                , searchId.BankId
                                , searchId.BranchId
                                , searchId.BonusType
                                , searchId.ReferenceNo);
                            //selectedYear, selectedMonth, letterType, bankId, bankBranchId, bonusType
                        }
                    }
                    else
                    {
                        lblMsg.Text = Common.GetCommomMessage(CommonMessage.IDNotFound);
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        private void GenerateReport(string letterType, string SalaryYear, string SalaryMonth, int? BankId, int? BranchId, string BonusType, string ReferenceNo)
        {
            #region Processing Report Data
            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            var data = _pgmContext.sp_PGM_BankAdviceLetterMaster(letterType, SalaryYear, SalaryMonth,BankId, BranchId, BonusType, LoggedUserZoneInfoId, numErrorCode, strErrorMsg).DistinctBy(x => x.ReferenceNo).ToList();

            if (ReferenceNo!=string.Empty)
            {
                data = data.Where(x => x.ReferenceNo == ReferenceNo).ToList();
            }

            rvBankLetterCommonRpt.Reset();

            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                rvBankLetterCommonRpt.ProcessingMode = ProcessingMode.Local;
                rvBankLetterCommonRpt.LocalReport.ReportPath =
                    Server.MapPath("~/Reports/PGM/rdlc/BankAdviceLetterMasterRpt.rdlc");
                rvBankLetterCommonRpt.LocalReport.DataSources.Add(new ReportDataSource("dsBankAdviceLetter", data));

                string searchParameters = Common.CurrentDateTime.Date.ToString("dd-MMM-yyyy");
                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvBankLetterCommonRpt.LocalReport.SetParameters(new ReportParameter[] {p1});

                this.rvBankLetterCommonRpt.LocalReport.SubreportProcessing +=
                    new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvBankLetterCommonRpt.DataBind();
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

        protected void rvBankLetterCommonRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Page_Load(null, null);
        }

        protected void btnBack_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("~/PGM/BankAdviceLetter/Index");
        }
    }
}