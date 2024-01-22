using Domain.PGM;
using PGM.Web.Areas.PGM.Models.MonthlySalaryDistribution;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class SalaryDistributionController : Controller
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Ctor

        public SalaryDistributionController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        #region Actions
        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, SalaryDistributionViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            //dynamic list = null;

            var list = _pgmCommonservice.GetSalaryDistributionInfoList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false, model.SalaryYear, model.SalaryMonth).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();

            totalRecords = _pgmCommonservice.GetSalaryDistributionInfoList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true, model.SalaryYear, model.SalaryMonth).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).Count();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            //list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();
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

            if (request.SortingName == "SalaryYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryYear).ToList();
                }
            }
            if (request.SortingName == "SalaryMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryMonth).ToList();
                }
            }

            #endregion
            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(d.SalaryYear + "-" + d.SalaryMonth + "-" + d.EmployeeId, new List<object>()
                    {
                        d.SalaryYear + "-" + d.SalaryMonth + "-" + d.EmployeeId,
                        d.SalaryYear,
                        d.SalaryMonth,
                        "Details",
                        "Rollback"
                    }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailList(JqGridRequest request, string year, string month, SalaryDistributionViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            // dynamic list = null;

            var list = _pgmCommonservice.GetSalaryDistributionDetailInfoList(year, month, model.EmployeeInitial, model.DivisionId, model.ProjectId).OrderBy(x => x.EmpID).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };
            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

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

            if (request.SortingName == "SalaryYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryYear).ToList();
                }
            }
            if (request.SortingName == "SalaryMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryMonth).ToList();
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

            if (request.SortingName == "GrossSalary")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.GrossSalary).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.GrossSalary).ToList();
                }
            }
            if (request.SortingName == "TotalHours")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TotalHours).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TotalHours).ToList();
                }
            }

            if (request.SortingName == "TotalHours")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TotalHours).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TotalHours).ToList();
                }
            }

            if (request.SortingName == "RatePerHour")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RatePerHour).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RatePerHour).ToList();
                }
            }
            if (request.SortingName == "ProjectNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ProjectNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ProjectNo).ToList();
                }
            }
            if (request.SortingName == "ProjectHour")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ProjectHour).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ProjectHour).ToList();
                }
            }
            if (request.SortingName == "ProjectAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ProjectAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ProjectAmount).ToList();
                }
            }
            #endregion
            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                   d.Id,
                   d.EmpID,
                   d.DivisionId,
                   d.EmployeeInitial,
                   d.FullName,
                   d.GrossSalary,
                   d.TotalHours,
                   d.RatePerHour,
                   d.ProjectId,
                   d.ProjectNo,
                   d.ProjectHour,
                   d.ProjectAmount
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult SalaryDistributionDetail(string id)
        {
            var model = new SalaryDistributionViewModel();
            string[] yearMonth = id.Split('-');
            model.SalaryYear = yearMonth[0];
            model.SalaryMonth = yearMonth[1];

            return View(model);
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new SalaryDistributionViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult CreateOrEdit()
        {
            SalaryDistributionViewModel model = new SalaryDistributionViewModel();
            PopulateDropdown(model);
            model.SalaryYear = Convert.ToString(Common.CurrentDateTime.Year);
            model.SalaryMonth = Common.CurrentDateTime.ToString("MMMM");

            return PartialView("_CreateOrEdit", model);
        }

        [NoCache]
        public JsonResult SalaryDistributionProcess(string year, string month, int EmployeeId)
        {
            int result = 0;

            List<string> Message = new List<string>();
            bool Success = true;
            List<string> errorList = new List<string>();

            errorList = GetBusinessLogicValidation(year, month);

            if ((ModelState.IsValid) && (errorList.Count == 0))
            {
                result = _pgmCommonservice.PGMUnit.FunctionRepository.MonthlySalaryDistributionProcess(year, month, User.Identity.Name);

                if (result == 0)
                {
                    Message.Add("Salary Distribution Process has been completed successfully.");
                }
                else
                {
                    Success = false;
                    Message.Add("Salary Distribution Process is failed.");
                }
            }
            else
            {
                Success = false;
                foreach (var msg in errorList)
                {
                    Message.Add(msg);
                }
            }

            return Json(new
            {
                Success = Success,
                Message = Message
            });
        }

        [HttpPost, ActionName("Rollback")]
        [NoCache]
        public JsonResult MonthlySalaryDistributionRollback(string id)
        {
            int rollbackResult = 0;

            string[] salaryMonth = id.Split('-');

            List<string> errorList = new List<string>();

            errorList = GetBusinessLogicValidationRollback(salaryMonth[0], salaryMonth[1]);


            bool result = false;
            string errMsg = "Rollback is failed.";

            try
            {
                if ((ModelState.IsValid) && (errorList.Count == 0))
                {
                    rollbackResult = _pgmCommonservice.PGMUnit.FunctionRepository.MonthlySalaryDistributionRollbackProcess(salaryMonth[0], salaryMonth[1], User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }

            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General, "Rollback is not completed. Please see inner exception.");
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [NoCache]
        public ActionResult GetYearList()
        {
            var model = new SalaryDistributionViewModel();
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
        public ActionResult GetDivisionList()
        {
            var model = new SalaryDistributionViewModel();
            model.DivisionList = Common.PopulateCountryDivisionDDL(_pgmCommonservice.PGMUnit.DivisionRepository.GetAll().ToList());
            return View("_Select", model.DivisionList);
        }
        #endregion
        #region Utilities

        [NoCache]
        private void PopulateDropdown(SalaryDistributionViewModel model)
        {

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

        [NoCache]
        public List<string> GetBusinessLogicValidation(string year, string month)
        {
            List<string> errorMessage = new List<string>();
            dynamic list = null;

            DateTime nextMonth;
            DateTime LastMonth;
            DateTime salaryMonth;

            list = (from tr in _pgmCommonservice.PGMUnit.SalaryDistributionMasterRepository.GetAll()
                    select new
                    {
                        dtdate = Convert.ToDateTime(tr.SalaryYear + "-" + tr.SalaryMonth + "-01")
                    }).Distinct().OrderBy(x => x.dtdate).ToList().LastOrDefault();
            if (list != null)
            {
                LastMonth = Convert.ToDateTime(list.dtdate);
                nextMonth = LastMonth.AddMonths(1);

                salaryMonth = Convert.ToDateTime(year + "-" + month + "-01");

                if ((salaryMonth > nextMonth) || (salaryMonth < LastMonth))
                {
                    errorMessage.Add("Invalid salary month");
                }
            }
            
            return errorMessage;

        }

        [NoCache]
        public List<string> GetBusinessLogicValidationRollback(string year, string month)
        {
            List<string> errorMessage = new List<string>();
            dynamic list = null;

            DateTime nextMonth;
            DateTime LastMonth;
            DateTime salaryMonth;

            list = (from tr in _pgmCommonservice.PGMUnit.SalaryDistributionMasterRepository.GetAll()
                    select new
                    {
                        dtdate = Convert.ToDateTime(tr.SalaryYear + "-" + tr.SalaryMonth + "-01")
                    }).Distinct().OrderBy(x => x.dtdate).ToList().LastOrDefault();
            if (list != null)
            {
                LastMonth = Convert.ToDateTime(list.dtdate);
                nextMonth = LastMonth.AddMonths(1);

                salaryMonth = Convert.ToDateTime(year + "-" + month + "-01");

                if (salaryMonth != LastMonth)
                {
                    errorMessage.Add("Only last salary month allow to rollback.");
                }
            }

            return errorMessage;
        }

        #endregion

    }
}
