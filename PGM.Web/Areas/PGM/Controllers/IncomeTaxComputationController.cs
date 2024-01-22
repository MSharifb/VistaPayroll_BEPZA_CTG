using DAL.PGM;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.IncomeTaxComputation;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web.Mvc;


namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class IncomeTaxComputationController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        private readonly PGMEntities _pgmContext;

        #endregion

        #region Ctor

        public IncomeTaxComputationController(PGMCommonService pgmCommonservice, PGMEntities context)
        {
            this._pgmCommonservice = pgmCommonservice;
            this._pgmContext = context;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, IncomeTaxComputationViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmCommonservice.GetIncomeTaxInfoList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false, model.IncomeYear, LoggedUserZoneInfoId)
                .DistinctBy(x => x.TaxRuleId)
                .ToList();

            totalRecords = _pgmCommonservice.GetIncomeTaxInfoList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true, model.IncomeYear, LoggedUserZoneInfoId)
                .DistinctBy(x => x.TaxRuleId)
                .Count();


            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

            if (request.SortingName == "IncomeYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IncomeYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IncomeYear).ToList();
                }
            }

            if (request.SortingName == "AssessmentYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssessmentYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssessmentYear).ToList();
                }
            }

            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            //list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.IncomeYear + "," + d.AssessmentYear + "," + d.TaxRuleId), new List<object>()
                    {
                       d.IncomeYear + "," + d.AssessmentYear + "," + d.TaxRuleId,
                        d.IncomeYear,
                        d.AssessmentYear,
                        "Details",
                        "Rollback"
                    }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult IncomeTaxDetail(string idYearMonth)
        {
            var model = new IncomeTaxComputationViewModel();
            if (idYearMonth != null)
            {
                string[] yearMonth = idYearMonth.Split(',');
                model.IncomeYear = yearMonth[0];
                model.AssessmentYear = yearMonth[1];
            }
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailList(JqGridRequest request, string incomeYear, string assessmentYear, IncomeTaxComputationViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            // dynamic list = null;

            var list = _pgmCommonservice.GetIncomeTaxDetailInfoList(incomeYear, assessmentYear, model.EmployeeInitial, model.DivisionId, model.EmploymentTypeId, model.DesignationId, LoggedUserZoneInfoId).OrderBy(x => x.EmpID).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

            if (request.SortingName == "DivisionId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DivisionId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DivisionId).ToList();
                }
            }

            if (request.SortingName == "EmpID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpID).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpID).ToList();
                }
            }

            if (request.SortingName == "EmployeeInitial")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeInitial).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeInitial).ToList();
                }
            }

            if (request.SortingName == "FullName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FullName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FullName).ToList();
                }
            }

            if (request.SortingName == "TaxableIncome")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxableIncome).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxableIncome).ToList();
                }
            }

            if (request.SortingName == "TaxLiability")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxLiability).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxLiability).ToList();
                }
            }

            if (request.SortingName == "InvestmentRebate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.InvestmentRebate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.InvestmentRebate).ToList();
                }
            }

            if (request.SortingName == "TaxPayable")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxPayable).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxPayable).ToList();
                }
            }

            if (request.SortingName == "TaxPerMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxPerMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxPerMonth).ToList();
                }
            }

            if (request.SortingName == "TaxDeducted")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxDeducted).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxDeducted).ToList();
                }
            }

            if (request.SortingName == "TaxDue")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxDue).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxDue).ToList();
                }
            }

            #endregion
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId + "," + d.IncomeYear), new List<object>()
                {
                   d.TaxRuleId + "," + d.EmployeeId,
                   d.EmpID,
                   d.IncomeYear,
                   d.AssessmentYear,
                   d.DivisionId,
                   d.EmploymentTypeId,
                   d.FullName,
                   d.DesignationId,
                   d.TaxableIncome,
                   d.TaxLiability,
                   d.InvestmentRebate,
                   d.TaxPayable,
                   d.TaxPerMonth,
                   d.TaxDeducted,
                   d.TaxDue,
                   "Remove"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new IncomeTaxComputationViewModel();
            return View(model);
        }

        [NoCache]
        public RedirectResult TaxComputationSheet(string idYearMonth)
        {
            return Redirect("~/Reports/PGM/viewers/IncomeTaxComputationSheetViewer.aspx?idYearMonth=" + idYearMonth);
        }

        [NoCache]
        public ActionResult CreateOrEdit()
        {
            IncomeTaxComputationViewModel model = new IncomeTaxComputationViewModel();
            PopulateDropdown(model);
            model.SalaryYear = Convert.ToString(Common.CurrentDateTime.Year);
            model.SalaryMonth = Common.CurrentDateTime.ToString("MMMM");

            return PartialView("_CreateOrEdit", model);
        }

        [NoCache]
        public JsonResult IncomeTaxComputationProcess(string incomeYear, string year, string month)
        {
            int result = 0;
            string Message = string.Empty;
            bool Success = true;
            string errorList = string.Empty;

            errorList = GetBusinessLogicValidation(incomeYear, year, month);

            if ((ModelState.IsValid) && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    result = _pgmCommonservice.PGMUnit.FunctionRepository.IncomeTaxComputationProcess(
                        incomeYear,
                        year,
                        month,
                        0,
                        LoggedUserZoneInfoId,
                        User.Identity.Name);

                    if (result == 0)
                    {
                        Message = "Income Tax Process has been completed successfully.";
                    }
                    else
                    {
                        Success = false;
                        Message = "Income Tax Process is failed.";
                    }
                }
                catch (Exception ex)
                {
                    Success = false;
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                Success = false;
                Message = errorList;
            }

            return Json(new
            {
                Success = Success,
                Message = Message
            });
        }

        [HttpPost, ActionName("Rollback")]
        [NoCache]
        public JsonResult IncomeTaxRollback(string idRollback)
        {
            int rollbackResult = 0;

            string[] salaryMonth = idRollback.Split(',');

            List<string> errorList = new List<string>();

            bool result = false;
            string errMsg = "Rollback is failed.";

            try
            {
                if ((ModelState.IsValid) && (errorList.Count == 0))
                {
                    rollbackResult = _pgmCommonservice.PGMUnit.FunctionRepository.IncomeTaxRollback(Convert.ToInt32(salaryMonth[2]), User.Identity.Name, LoggedUserZoneInfoId);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General,
                    "Income tax rollback is not completed. Please see inner exception.");
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [HttpPost, ActionName("RollbackIndividual")]
        [NoCache]
        public JsonResult RollbackIndividual(string idRollbackIndividual)
        {
            string[] ids = idRollbackIndividual.Split(',');

            int rollbackResult = 0;
            bool result = false;
            string errMsg = string.Empty;
            string errMsgList = string.Empty;

            //errMsgList = GetBusinessLogicValidationRollbackIndividual(ids[2], ids[3], Convert.ToInt32(ids[1]));

            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errMsgList)))
            {
                try
                {
                    rollbackResult = _pgmCommonservice.PGMUnit.FunctionRepository.IncomeTaxRollbackIndividual(Convert.ToInt32(ids[0]), Convert.ToInt32(ids[1]));
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                }
            }
            else
            {
                errMsg = errMsgList;
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [NoCache]
        private string GetBusinessLogicValidation(string incomeYear, string year, string month)
        {
            List<string> errorMessage = new List<string>();

            string[] incomeYearArray = incomeYear.Split('-');

            var startDate = Convert.ToDateTime(incomeYearArray[0] + "-" + "July-01");
            var endDate = Convert.ToDateTime(incomeYearArray[1] + "-" + "June-30");

            var salarydate = Convert.ToDateTime(year + "-" + month + "-" + "01");

            if ((salarydate < startDate) || (salarydate > endDate))
            {
                errorMessage.Add("Salary month & year must be between " + startDate.ToString("MMM/yy") + " and " + endDate.ToString("MMM/yy") + ".");
            }

            PGM_TaxRateRule taxRateFor = null;
            int assesseeTypeId = 0;
            foreach (var item in Enum.GetValues(typeof(PGMEnum.TaxAssesseeType)))
            {
                assesseeTypeId = Convert.ToInt32((PGMEnum.TaxAssesseeType)item);
                taxRateFor = _pgmCommonservice.PGMUnit.TaxRateMasterRepository.GetAll().FirstOrDefault(x => x.IncomeYear == incomeYear && x.AssesseeTypeId == assesseeTypeId);
                if (taxRateFor == null)
                    errorMessage.Add("Tax rate setup for " + ((PGMEnum.TaxAssesseeType)item) +
                                     " doesn't exist.Please, setup the tax rate for the income year of " +
                                     incomeYear);
            }

            var taxRuleExist = _pgmCommonservice.PGMUnit.TaxRule.GetAll().FirstOrDefault(x => x.IncomeYear == incomeYear);
            if (taxRuleExist == null)
            {
                errorMessage.Add("Tax rule doesn't exist. Please, setup the tax rule for the income year of " +
                                 incomeYear);
            }

            var taxRegionWiseMinimumRuleExist = _pgmCommonservice.PGMUnit.TaxRegionWiseMinRuleRepository.GetAll().FirstOrDefault(x => x.IncomeYear == incomeYear);
            if (taxRegionWiseMinimumRuleExist == null)
            {
                errorMessage.Add(
                    "Region wise minimum tax rule doesn't exist. Please, setup the region wise minimum tax rule for the income year of " +
                    incomeYear);
            }

            var taxExemptionRuleExist = _pgmCommonservice.PGMUnit.TaxExemptionRuleRepository.GetAll().FirstOrDefault(x => x.IncomeYear == incomeYear);
            if (taxExemptionRuleExist == null)
            {
                errorMessage.Add(
                    "Tax exemption rule doesn't exist. Please, setup the tax exemption rule for the income year of " +
                    incomeYear);
            }


            return Common.ErrorListToString(errorMessage);
        }

        //Individual Rollback Validation Need to fix
        [NoCache]
        private string GetBusinessLogicValidationRollbackIndividual(string year, string month, int empID)
        {
            string errMessage = string.Empty;

            var checkingOutInBankLetter = (from BM in _pgmCommonservice.PGMUnit.BankAdviceLetters.GetAll()
                                           join BD in _pgmCommonservice.PGMUnit.BankAdviceLetterDetails.GetAll()
                                               on BM.Id equals BD.BankLetterId
                                           where BM.SalaryYear == year && BM.SalaryMonth == month && BD.EmployeeId == empID
                                           select BM).ToList();

            if (checkingOutInBankLetter.Count > 0)
            {
                errMessage = "Rollback is failed.Because bank letter already generated.";
            }

            var checkingOutInWithheldPayment = (from w in _pgmCommonservice.PGMUnit.WithheldSalaryPayment.GetAll() where (w.SalaryYear == year && w.SalaryMonth == month && w.EmployeeId == empID) select w).ToList();
            if (checkingOutInWithheldPayment.Count > 0)
            {
                errMessage = "Rollback is denied.Because salary is already paid.";
            }
            return errMessage;
        }

        [NoCache]
        public ActionResult GetYearList()
        {
            var model = new IncomeTaxComputationViewModel();
            model.YearList = Common.PopulateYearList().DistinctBy(x => x.Value).ToList();
            return View("_Select", model.YearList);
        }

        [NoCache]
        public ActionResult GetMonthList()
        {
            var monthList = Common.PopulateMonthList().DistinctBy(x => x.Value).ToList();
            return View("_Select", monthList);
        }

        [NoCache]
        public ActionResult GetIncomeYear()
        {
            var model = new IncomeTaxComputationViewModel();
            model.IncomeYearList = _pgmCommonservice.PGMUnit.TaxRule.GetAll().DistinctBy(x => x.IncomeYear).Select(y => new SelectListItem()
            {
                Text = y.IncomeYear,
                Value = y.IncomeYear.ToString()
            }).ToList();

            return View("_Select", model.IncomeYearList);
        }

        [NoCache]
        public ActionResult GetDivisionList()
        {
            var DivisionList = Common.PopulateDDLList(_pgmCommonservice.PGMUnit.DivisionRepository.GetAll().OrderBy(x => x.Name).ToList());
            return View("_Select", DivisionList);
        }

        [NoCache]
        public ActionResult GetEmploymentCategoryList()
        {

            var EmployeeTypeList = Common.PopulateEmployeementTypeDDL(_pgmCommonservice.PGMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            return View("_Select", EmployeeTypeList);
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {

            var DesignationList = Common.PopulateEmployeeDesignationDDL(_pgmCommonservice.PGMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList());
            return View("_Select", DesignationList);
        }

        #endregion

        #region Utilities
        [NoCache]
        private void PopulateDropdown(IncomeTaxComputationViewModel model)
        {

            model.IncomeYearList = _pgmCommonservice.PGMUnit.TaxRule.GetAll().DistinctBy(x => x.IncomeYear).Select(y => new SelectListItem()
            {
                Text = y.IncomeYear,
                Value = y.IncomeYear.ToString()
            }).ToList();

            model.YearList = Common.PopulateYearList().DistinctBy(x => x.Value).Select(y => new SelectListItem()
            {
                Text = y.Value,
                Value = y.Text.ToString()
            }).ToList();

            model.MonthList = Common.PopulateMonthList().DistinctBy(x => x.Value).Select(y => new SelectListItem()
            {
                Text = y.Value,
                Value = y.Text.ToString()
            }).ToList();
        }

        #endregion
    }
}
