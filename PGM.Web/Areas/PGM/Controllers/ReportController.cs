using PGM.Web.Areas.PGM.Models.Report;
using System.Web.Mvc;


namespace PGM.Web.Areas.PGM.Controllers
{
    public class ReportController : Controller
    {
        #region Fields
             private readonly ReportViewerViewModel _model;
        #endregion

        #region Ctor
        public ReportController()
        {
            _model = new ReportViewerViewModel();
        }
        #endregion

        #region Report Actions

        public ActionResult ElectricBill()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/ElectricityBillRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult MonthlySalary()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/MonthlySalaryRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult SalaryPaySlip()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/SalaryPaySlip.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult SalaryPaySlipDetail()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/SalaryPaySlipDetail.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult MyPaySlip()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/MyPaySlip.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult YearlySalaryStatement()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/YearlySalaryStatementRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult BankAdviceLetter()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/BankAdviceLetterRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult OtherAdjustment()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/MonthlyOtherAdjustment.aspx");
            return View("ReportViewer", _model);
        }

        //public ActionResult DivisionWiseSalary()
        //{
        //    _model.ReportPath = Url.Content("~/Reports/PGM/viewers/DivisionWiseSalaryRpt.aspx");
        //    return View("ReportViewer", _model);
        //}


        //public ActionResult FestivalBonus()
        //{
        //    _model.ReportPath = Url.Content("~/Reports/PGM/viewers/FestivalBonusRpt.aspx");
        //    return View("ReportViewer", _model);
        //}

        //public ActionResult OverTime()
        //{
        //    _model.ReportPath = Url.Content("~/Reports/PGM/viewers/OverTimeRpt.aspx");
        //    return View("ReportViewer", _model);
        //}

        public ActionResult IncomeTaxComputation()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/IncomeTaxComputationRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult IncomeTaxSummary()
        {

            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/IncomeTaxSummaryRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult TaxComputationSheet(string idYearMonth)
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/CommonViewer.aspx?idYearMonth=" + idYearMonth);
            return View("ReportViewer", _model);
        }

        //public ActionResult GratuityFundRequiredSummary()
        //{
        //    _model.ReportPath = Url.Content("~/Reports/PGM/viewers/GratuityFundRequiredSummaryRpt.aspx");
        //    return View("ReportViewer", _model);
        //}

        //public ActionResult GratuityIndividual()
        //{
        //    _model.ReportPath = Url.Content("~/Reports/PGM/viewers/GratuityIndividualRpt.aspx");
        //    return View("ReportViewer", _model);
        //}

        public ActionResult BonusPaySlip()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/BonusPaySlipRpt.aspx");
            return View("ReportViewer", _model);
        }

        //public ActionResult GratuityProvision()
        //{
        //    _model.ReportPath = Url.Content("~/Reports/PGM/viewers/GratuityProvisionRpt.aspx");
        //    return View("ReportViewer", _model);
        //}


        public ActionResult LeaveEncashmentReport()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/LeaveEncashmentRpt.aspx");
            return View("ReportViewer", _model);
        }


        public ActionResult LeaveEncashmentPaySlip()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/LeaveEncashmentPaySlipRpt.aspx");
            return View("ReportViewer", _model);
        }


        public ActionResult FinalPaymentSheet()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/FinalPaymentSheetRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult MonthlyPayrollExpSheet()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/MonthlyPayrollExpSheetRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult BankWiseMonthlyElectricityBill()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/BankWiseMonthlyElectricityBillRpt.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult BankWiseMonthlySalaryStatement()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/BankWiseMonthlySalaryStatement.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult OvertimeBillforStaff()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/OvertimeReportStaffRpt.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult RefreshmentBillForOfficer()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/RefreshmentBillForOfficer.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult AttendanceReport()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/AttendanceRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult NightBillRpt()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/NightBillRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult MonthlyBonusStatement()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/BonusStatementRpt.aspx");
            return View("ReportViewer", _model);
        }
        public ActionResult HeadWiseReport()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/HeadWiseReport.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult BankWiseIncentiveBonus()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/BankWiseIncentiveBonus.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult JournalVoucher()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/JournalVoucherRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult EmployeeBasicInfo()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/EmployeeBasicInfo.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult FinalSettlementRpt()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/FinalSettlementRpt.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult MISReport()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/MISReport.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult HeadWiseSummaryReport()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/HeadWiseSummaryReport.aspx");
            return View("ReportViewer", _model);
        }

        public ActionResult MonthlySalaryDifferenceReport()
        {
            _model.ReportPath = Url.Content("~/Reports/PGM/viewers/MonthlySalaryDifferenceReport.aspx");
            return View("ReportViewer", _model);
        }
        #endregion
    }
}
