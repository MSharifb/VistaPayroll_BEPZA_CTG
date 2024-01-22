using DAL.PGM;
using DAL.PGM.CustomEntities;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.MonthlySalaryInfo;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class MonthlySalaryController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private readonly PGMMonthlySalaryService _monthlySalaryService;
        private readonly PGMEntities _pgmContext;
        #endregion

        #region Ctor

        public MonthlySalaryController(PGMCommonService pgmCommonservice, PGMMonthlySalaryService monthlySalaryService, PGMEntities context)
        {
            this._pgmCommonService = pgmCommonservice;
            this._monthlySalaryService = monthlySalaryService;
            this._pgmContext = context;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        #region Actions Methods

        [NoCache]
        public ActionResult Index()
        {
            var model = new MonthlySalaryViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, MonthlySalaryViewModel viewModel)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            // dynamic list = null;

            var list = _monthlySalaryService.GetSalaryInfoList(""
                                            , request.SortingName
                                            , request.SortingOrder.ToString()
                                            , request.PageIndex
                                            , request.RecordsCount
                                            , request.PagesCount.HasValue ? request.PagesCount.Value : 1
                                            , false
                                            , viewModel.SalaryYear
                                            , viewModel.SalaryMonth
                                            , LoggedUserZoneInfoId)
                                    .OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01"))
                                    .ToList();

            totalRecords = _monthlySalaryService.GetSalaryInfoList(""
                    , request.SortingName
                    , request.SortingOrder.ToString()
                    , request.PageIndex
                    , request.RecordsCount
                    , request.PagesCount.HasValue ? request.PagesCount.Value : 1
                    , true
                    , viewModel.SalaryYear
                    , viewModel.SalaryMonth
                    , LoggedUserZoneInfoId)
                .ToList().Count;

            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();
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

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(d.SalaryYear + "-" + d.SalaryMonth, new List<object>()
                {
                    d.SalaryYear+"-"+ d.SalaryMonth,
                    d.SalaryYear,
                    d.SalaryMonth,
                    d.IsConfirmed,
                    "Details",
                    "Rollback"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult GoToDetails(string idYearMonth)
        {
            var model = new MonthlySalaryViewModel();

            string[] yearMonth = idYearMonth.Split('-');
            model.SalaryYear = yearMonth[0];
            model.SalaryMonth = yearMonth[1];

            SalaryMonthInfo list = _monthlySalaryService
                .GetSalaryInfoList(
                    ""
                    , string.Empty
                    , string.Empty
                    , 0
                    , 0
                    , 1
                    , true
                    , model.SalaryYear
                    , model.SalaryMonth
                    , LoggedUserZoneInfoId)
                .ToList()
                .FirstOrDefault();

            if (list != null) // Label text purpose only
            {
                model.IsConfirmed = list.IsConfirmed;
            }
            GetApproverList(model);
            return View("MonthlySalaryDetails", model);
        }

        public void GetApproverList(MonthlySalaryViewModel model)
        {
            int employeeId = _pgmCommonService.PGMUnit.EmploymentInfoRepository.Get(x => x.EmpID == MyAppSession.EmpId).Select(x => x.Id).FirstOrDefault();
            var approverList = _pgmContext.ACC_getApproverListByZoneId(LoggedUserZoneInfoId, "AccVou", employeeId, null).ToList();
            var avlist = new List<SelectListItem>();
            foreach (var item in approverList)
            {
                avlist.Add(new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id.ToString()
                });
            }
            model.ApproverList = avlist;

            int ZoneId = _pgmCommonService.PGMUnit.ZoneInfoRepository.Get(x=>x.ZoneCode.Contains("BEO")).Select(s=>s.Id).FirstOrDefault();
            var cpfapproverList = _pgmContext.ACC_getApproverListByZoneId(ZoneId, "AccVou", employeeId, null).ToList();
            var cpflist = new List<SelectListItem>();
            foreach (var item in cpfapproverList)
            {
                cpflist.Add(new SelectListItem()
                {
                    Text = item.FullName,
                    Value = item.Id.ToString()
                });
            }
            model.CPFApproverList = cpflist;

        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailList(JqGridRequest request, string year, string month, MonthlySalaryViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            //  dynamic list = null;

            var list = _monthlySalaryService.GetSalaryDetailInfoList(""
                    , request.SortingName
                    , request.SortingOrder.ToString()
                    , request.PageIndex
                    , request.RecordsCount
                    , request.PagesCount.HasValue ? request.PagesCount.Value : 1
                    , false
                    , model.EmpID
                    , year
                    , month
                    , model.DivisionId
                    , model.EmploymentTypeId
                    , model.EmployeeInitial
                    , model.IsWithheld
                    , LoggedUserZoneInfoId)
                 .ToList();

            totalRecords = _monthlySalaryService.GetSalaryDetailInfoList(""
                    , request.SortingName
                    , request.SortingOrder.ToString()
                    , request.PageIndex
                    , request.RecordsCount
                    , request.PagesCount.HasValue ? request.PagesCount.Value : 1
                    , true
                    , model.EmpID
                    , year
                    , month
                    , model.DivisionId
                    , model.EmploymentTypeId
                    , model.EmployeeInitial
                    , model.IsWithheld
                    , LoggedUserZoneInfoId)
                .ToList().Count;

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

            if (request.SortingName == "IsWithheld")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsWithheld).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsWithheld).ToList();
                }
            }

            if (request.SortingName == "IsPaid")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsPaid).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsPaid).ToList();
                }
            }

            if (request.SortingName == "AccountNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AccountNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AccountNo).ToList();
                }
            }

            if (request.SortingName == "GrossSal")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.GrossSal).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.GrossSal).ToList();
                }
            }

            if (request.SortingName == "TotalDeduction")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TotalDeduction).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TotalDeduction).ToList();
                }
            }

            if (request.SortingName == "NetPay")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.NetPay).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.NetPay).ToList();
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id + "-" + d.EmployeeId + "-" + year + "-" + month), new List<object>()
                {
                   d.Id + "-" + d.EmployeeId + "-" + year + "-"+month,
                   year,
                   month,
                   d.EmpID,
                   d.DivisionId,
                   d.EmploymentTypeId,
                   d.FullName,
                   d.IsWithheld==true?"Yes":"No",
                   d.IsPaid==true?"Paid":"Not Paid",
                   d.AccountNo,
                   d.GrossSal,
                   d.TotalDeduction,
                   d.NetPay,
                   d.IsConfirmed,
                   "Remove"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult CreateOrEdit()
        {
            MonthlySalaryViewModel model = new MonthlySalaryViewModel();
            PopulateDropdown(model);
            model.SalaryYear = Convert.ToString(Common.CurrentDateTime.Year);
            model.SalaryMonth = Common.CurrentDateTime.ToString("MMMM");

            return PartialView("_CreateOrEdit", model);
        }

        [NoCache]
        public JsonResult SalaryProcess(string year, string month, string employeeId, Boolean processTaxWithSalary)
        {
            int result = 0;
            List<string> Message = new List<string>();
            bool Success = true;
            List<string> errorList = new List<string>();
            var messageToReturn = String.Empty;

            // employee primary key
            int employeeId_pk = 0;
            try
            {
                // If single employee then check for existance of employee
                //     possible scenario could be - employee is inactive or
                //     employee id does not exist in database.
                if (!String.IsNullOrEmpty(employeeId) && Common.GetInteger(employeeId) != 0)
                {
                    employeeId_pk = Common.GetInteger(employeeId);

                    var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                        .FirstOrDefault(t => t.Id == employeeId_pk
                                             && t.EmploymentStatusId == 1
                                             && t.DateofInactive == null
                                             && t.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId);

                    if (emp == null)
                    {
                        Success = false;
                        Message.Add("Invalid or Inactive Employee! Please Check.");
                    }
                } 
                // If single employee

                // Data validation
                errorList = GetBusinessLogicValidation(year, month, employeeId_pk);

                // Process
                if ((ModelState.IsValid) && (errorList.Count == 0))
                {
                    int totalProcessed = 0;
                    result = _pgmCommonService.PGMUnit.FunctionRepository.MonthlySalaryProcess(year, month, employeeId_pk, LoggedUserZoneInfoId, processTaxWithSalary, User.Identity.Name, out totalProcessed);

                    if (result == 0)
                    {
                        messageToReturn = "Salary Process has been completed successfully." + Environment.NewLine + "Total Processed: " + totalProcessed;
                    }
                    else
                    {
                        Success = false;
                        Message.Add("Salary Process is failed!");
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
            }
            catch (Exception ex)
            {
                Message.Add(CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save));
                Success = false;
            }

            if (!Success) messageToReturn = Common.ErrorListToString(Message);

            return Json(new
            {
                Success = Success,
                Message = messageToReturn
            });
        }

        [HttpPost, ActionName("Rollback")]
        [NoCache]
        public JsonResult RollbackConfirmed(string idYearMonth)
        {
            int rollbackResult = 0;
            string[] salaryMonth = idYearMonth.Split('-');
            bool result = false;
            string errMsg = string.Empty;
            string errorList = string.Empty;

            errorList = GetBusinessLogicValidationRollback(salaryMonth[0], salaryMonth[1]);

            if ((ModelState.IsValid) && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    rollbackResult = _pgmCommonService.PGMUnit.FunctionRepository.MonthlySalaryRollbackProcess(salaryMonth[0], salaryMonth[1], LoggedUserZoneInfoId, User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }
            else
            {
                errMsg = errorList;
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
            string[] ids = idRollbackIndividual.Split('-');

            int rollbackResult = 0;
            bool result = false;
            string errMsg = string.Empty;
            string errMsgList = string.Empty;

            errMsgList = GetBusinessLogicValidationRollbackIndividual(ids[2], ids[3], Convert.ToInt32(ids[1]));

            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errMsgList)))
            {
                try
                {
                    rollbackResult = _pgmCommonService.PGMUnit.FunctionRepository.MonthlySalaryRollbackIndividual(Convert.ToInt64(ids[0]), User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete, "Salary rollback cannot complete. Please see inner exception.");
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
        #endregion

        #region Other Actions Methods
        [NoCache]
        public JsonResult ConfirmMonthlySalary(string year, string month)
        {
            bool result = false;
            string errMsg = string.Empty;
            string errMsgList = string.Empty;
            var master = _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                .FirstOrDefault(s => s.SalaryYear == year && s.SalaryMonth == month && s.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId);

            if (master != null)
            {
                try
                {
                    var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                    var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                    int update = _pgmContext.sp_PGM_UpdateConfirmedSalary(year, month, LoggedUserZoneInfoId, numErrorCode, strErrorMsg);
                    update = Common.GetInteger(numErrorCode.Value);
                    if (update == 0)
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                }
            }
            else
            {
                result = false;
            }

            return Json(new
            {
                success = result
            });
        }

        [NoCache]
        public ActionResult GetYearList()
        {
            var model = new MonthlySalaryViewModel();
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
            var model = new MonthlySalaryViewModel();
            model.DivisionList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.DivisionRepository.GetAll().OrderBy(x => x.Name).ToList());
            return View("_Select", model.DivisionList);
        }

        [NoCache]
        public ActionResult GetEmployeeTypeList()
        {
            var model = new MonthlySalaryViewModel();
            model.EmployeeTypeList = Common.PopulateEmployeementTypeDDL(_pgmCommonService.PGMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList());
            return View("_Select", model.EmployeeTypeList);
        }

        [NoCache]
        public ActionResult GetWithheldStatusList()
        {
            var model = new MonthlySalaryViewModel();
            model.WithheldStatusList = Common.PopulateYesNoDDLList();
            return View("_Select", model.WithheldStatusList);
        }


        private string GetIncomeYear(string year, string month)
        {
            string incomeyear = string.Empty;
            DateTime dtDate = Convert.ToDateTime(year + "/" + month + "/01");

            if (dtDate.Month < 7)
            {
                incomeyear = (Convert.ToInt16(year) - 1).ToString() + "-" + year;
            }
            else
            {
                incomeyear = year + "-" + (Convert.ToInt16(year) + 1).ToString();
            }
            return incomeyear;
        }
        #endregion

        #region Utilities
        [NoCache]
        private void PopulateDropdown(MonthlySalaryViewModel model)
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

            //----------
            var emps = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                .Select(q => new
                {
                    ZoneInfoId = q.SalaryWithdrawFromZoneId,
                    EmpID = q.EmpID,
                    Id = q.Id,
                    DateOfInactive = q.DateofInactive,
                    DisplayText = q.FullName + " [" + q.EmpID + " ]"
                })
                .Where(x => x.ZoneInfoId == LoggedUserZoneInfoId && x.DateOfInactive == null).ToList();

            model.EmployeeList = emps.Select(e => new SelectListItem()
            {
                Text = e.DisplayText,
                Value = e.Id.ToString()
            }).ToList();
            //------------

        }

        [NoCache]
        public List<string> GetBusinessLogicValidation(string year, string month, int employeeIdpk)
        {
            List<string> errorMessage = new List<string>();
            dynamic lastProcessedSalary = null;

            // Check employee salary structure.
            //------------------------------------->>>>>>>>>
            List<vwPGMEmploymentInfo> query = null;
            // If single employee
            if (employeeIdpk != 0)
            {
                var monthIndex = Convert.ToInt32(Common.PopulateMonthList3().DistinctBy(x => x.Value).FirstOrDefault(x => x.Text == month).Value);
                var intYear = Convert.ToInt32(year);

                query =
                (from E in
                     _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList().Where(
                          t => t.Id == employeeIdpk
                              && t.EmploymentStatusId == 1 && t.DateofInactive == null
                              && (t.DateofJoining.Month <= monthIndex && t.DateofJoining.Year <= intYear)
                              && t.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId)
                 join S in _pgmCommonService.PGMUnit.EmpSalaryRepository.GetAll() on E.Id equals S.EmployeeId
                 select E).ToList();
            }
            // else all employee
            else
            {
                query =
                (from E in
                     _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList().Where(
                          t =>
                              t.EmploymentStatusId == 1 && t.DateofInactive == null &&
                              t.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId)
                 join S in _pgmCommonService.PGMUnit.EmpSalaryRepository.GetAll() on E.Id equals S.EmployeeId
                 select E).ToList();
            }

            if (query == null || query.Count() == 0)
            {
                errorMessage.Add("Sorry, No eligible employee found to process." + Environment.NewLine +
                                 "Please check employee salary structure and other necessary information " + Environment.NewLine +
                                 "(like Joining date, employment status is active, employment type is confirm.).");
                return errorMessage;
            }
            //-------------------------------------<<<<<<<<<


            // Validate salary process month.
            // ----------------------------------->>>>>>>>>
            // Step: 1 - Get last month salary information
            lastProcessedSalary = (from SM in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                                    where SM.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                                    select new
                                    {
                                        dtdate = Convert.ToDateTime(SM.SalaryYear + "-" + SM.SalaryMonth + "-01")
                                    }).Distinct().OrderBy(x => x.dtdate).ToList().LastOrDefault();


            // if last month salary found then validate salary process month -
            //     1. Salary process month cannot be greater than next month.
            //     2. and also salary process month cannot be less than last salary month.
            if (lastProcessedSalary != null)
            {
                DateTime LastMonth = Convert.ToDateTime(lastProcessedSalary.dtdate);
                DateTime nextMonth = LastMonth.AddMonths(1);
                DateTime salaryMonth = Convert.ToDateTime(year + "-" + month + "-01");

                // Step: 2 - Validate salary month.
                if (salaryMonth > nextMonth)
                {
                    errorMessage.Add("You can't skip the salary month." + Environment.NewLine +
                                     "Please process the salary in sequential order of salary month. " +
                                     Environment.NewLine +
                                     "The processed salary month last on " + LastMonth.ToString("MMM/yyyy"));

                    return errorMessage;
                }
                else if (salaryMonth < LastMonth)
                {
                    errorMessage.Add("You can't process previous month of last processed month." + Environment.NewLine +
                                     "Please process the salary in sequential order of salary month. " +
                                     Environment.NewLine +
                                     "The processed salary month last on " + LastMonth.ToString("MMM/yyyy"));

                    return errorMessage;
                }

            }
            // if no processed salary found then
            //        check salary process month cannot be greater than current month.
            else
            {
                if (Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01") < Convert.ToDateTime(year + "-" + month + "-01"))
                {
                    errorMessage.Add("Salary process month can't greater than current month '" + DateTime.Now.Month.ToString("MMM") + "'.");
                    return errorMessage;
                }
            }
            // -----------------------------------<<<<<<<<<<<<<

            // var salHeads = _pgmCommonservice.PGMUnit.FunctionRepository.PGM_FN_GetMappingSalaryHeads("", "");
            var salHeads = _pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll();

            if (salHeads != null)
            {
                bool found = false;

                StringBuilder msg1 = new StringBuilder(50);
                StringBuilder msg2 = new StringBuilder(50);
                msg2.AppendLine().Append(" Salary head did not mapped yet. Please do in Salary head mapping.");


                found = salHeads.Any(h => h.IsBasicHead == true);
                if (!found)
                {
                    msg1.AppendLine().Append("Basic");
                }

                // found = CheckExistSalaryHead(salHeads, PGMEnum.MappingSalaryHeads.HouseRent.ToString());
                found = salHeads.Any(h => h.IsHouseRentHead == true);
                if (!found)
                {
                    msg1.AppendLine().Append("House rent");
                }

                // found = CheckExistSalaryHead(salHeads, PGMEnum.MappingSalaryHeads.Medical.ToString());
                found = salHeads.Any(h => h.IsMedicalHead == true);
                if (!found)
                {
                    if (msg1.Length != 0) msg1.Append(", ");
                    msg1.AppendLine().Append("Medical");
                }

                // found = CheckExistSalaryHead(salHeads, PGMEnum.MappingSalaryHeads.Conveyance.ToString());
                found = salHeads.Any(h => h.IsConveyanceHead == true);
                if (!found)
                {
                    if (msg1.Length != 0) msg1.Append(", ");
                    msg1.AppendLine().Append("Conveyance");
                }

                found = salHeads.Any(h => h.IsPFOwnContributionHead == true);
                if (!found)
                {
                    if (msg1.Length != 0) msg1.Append(", ");
                    msg1.AppendLine().Append("PF Own Contribution");
                }

                found = salHeads.Any(h => h.IsPFCompanyContributionHead == true);
                if (!found)
                {
                    if (msg1.Length != 0) msg1.Append(", ");
                    msg1.AppendLine().Append("PF Company Contribution");
                }

                found = salHeads.Any(h => h.IsIncomeTaxDeductionHead == true);
                if (!found)
                {
                    if (msg1.Length != 0) msg1.Append(", ");
                    msg1.AppendLine().Append("Income tax");
                }


                if (msg1.Length != 0) errorMessage.Add(msg1.ToString() + msg2.ToString() + Environment.NewLine);
            }

            var incomeYear = GetIncomeYear(year, month);

            var taxRuleExist = _pgmCommonService.PGMUnit.TaxRule.GetAll().FirstOrDefault(x => x.IncomeYear == incomeYear);
            if (taxRuleExist == null)
            {
                errorMessage.Add("Income tax rule doesn't exist. Please, setup the tax rule for the income year of " + incomeYear + Environment.NewLine);
            }

            PGM_TaxRateRule taxRateFor = null;
            int assesseeTypeId = 0;
            foreach (var item in Enum.GetValues(typeof(PGMEnum.TaxAssesseeType)))
            {
                assesseeTypeId = Convert.ToInt32((PGMEnum.TaxAssesseeType)item);
                taxRateFor = _pgmCommonService.PGMUnit.TaxRateMasterRepository.GetAll().FirstOrDefault(x => x.IncomeYear == incomeYear && x.AssesseeTypeId == assesseeTypeId);
                if (taxRateFor == null)
                    errorMessage.Add("Tax rate setup for " + ((PGMEnum.TaxAssesseeType)item) + " doesn't exist.Please, setup the tax rate for the income year of " + incomeYear + Environment.NewLine);
            }

            var taxRegionWiseMinimumRuleExist = _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.GetAll().FirstOrDefault(x => x.IncomeYear == incomeYear);
            if (taxRegionWiseMinimumRuleExist == null)
            {
                errorMessage.Add("Region wise minimum tax rule doesn't exist. Please, setup the region wise minimum tax rule for the income year of " + incomeYear + Environment.NewLine);
            }

            return errorMessage;
        }

        [NoCache]
        public string GetBusinessLogicValidationRollback(string year, string month)
        {
            string errorMessage = string.Empty;
            dynamic lastProcessedMonth = null;
            // Get zone wise Last processed month(date)
            lastProcessedMonth = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                                  where tr.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                                  select new
                                  {
                                      dtdate = Convert.ToDateTime(tr.SalaryYear + "-" + tr.SalaryMonth + "-01")
                                  }).Distinct().OrderBy(x => x.dtdate).ToList().LastOrDefault();

            if (lastProcessedMonth != null)
            {
                DateTime LastMonth = Convert.ToDateTime(lastProcessedMonth.dtdate);
                DateTime nextMonth = LastMonth.AddMonths(1);

                DateTime salaryMonth = Convert.ToDateTime(year + "-" + month + "-01");

                if (salaryMonth != LastMonth)
                {
                    errorMessage = "Only last salary month allow to rollback!";
                    return errorMessage;
                }
            }
            //-----

            // TODO: check bank advice letter before salary rollback
            //var bLCheckOut = (from BM in _pgmCommonservice.PGMUnit.BankAdviceLetters.GetAll()
            //                  where BM.SalaryYear == year
            //                    && BM.SalaryMonth == month
            //                    && BM.LetterType.ToLower() == "Salary".ToLower()
            //                    && BM.ZoneInfoId == LoggedUserZoneInfoId
            //                  select BM).ToList();

            //if (bLCheckOut.Count > 0)
            //{
            //    errorMessage = "Rollback is denied, because bank advice letter is already existed for the month.";
            //    return errorMessage;
            //}

            // Check withheld payment list
            var withheldPaymentList = (from w in _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetAll()
                                       join s in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll() on w.EmployeeId equals s.EmployeeId
                                       where (w.SalaryYear == year && w.SalaryMonth == month)
                                       && (s.SalaryYear == year && s.SalaryMonth == month)
                                       select w).ToList();

            if (withheldPaymentList != null && withheldPaymentList.Count > 0)
            {
                errorMessage = "Rollback is denied, because salary for withheld employee is already paid!";
                return errorMessage;
            }

            var confirmedEntries =
                (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                 where tr.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                     && tr.SalaryYear == year && tr.SalaryMonth == month
                     && tr.IsConfirmed == true
                 select tr).ToList();

            if (confirmedEntries != null && confirmedEntries.Count > 0)
            {
                errorMessage = "Salary for this month has already been confirmed! You cannot rollback!";
                return errorMessage;
            }

            return errorMessage;
        }

        [NoCache]
        private string GetBusinessLogicValidationRollbackIndividual(string year, string month, int empID)
        {
            string errMessage = string.Empty;

            var checkingOutInBankLetter = (from BM in _pgmCommonService.PGMUnit.BankAdviceLetters.GetAll()
                                           join BD in _pgmCommonService.PGMUnit.BankAdviceLetterDetails.GetAll()
                                           on BM.Id equals BD.BankLetterId
                                           where BM.SalaryYear == year && BM.SalaryMonth == month && BD.EmployeeId == empID
                                           select BM).ToList();

            if (checkingOutInBankLetter.Count > 0)
            {
                errMessage = "Rollback is failed. Because bank letter already generated.";
            }

            var checkingOutInWithheldPayment = (from w in _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetAll()
                                                where (w.SalaryYear == year && w.SalaryMonth == month && w.EmployeeId == empID)
                                                select w).ToList();
            if (checkingOutInWithheldPayment.Count > 0)
            {
                errMessage = "Rollback is denied. Because withheld salary is already paid.";
            }

            var isConfirmed = _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                .FirstOrDefault(s => s.EmployeeId == empID
                && s.SalaryYear == year && s.SalaryMonth == month
                && s.IsConfirmed == true);

            if (isConfirmed != null)
            {
                errMessage = "Salary for this month has already been confirmed! You cannot rollback salary.";
            }

            return errMessage;
        }
        #endregion

        #region Voucher Processing

        public ActionResult SalaryVoucherPosting(string year, string month, int? approverId)
        {
            var result = string.Empty;
            //string vInfo = string.Empty;
            //var sessionUser = MyAppSession.User;
            //int UserID = 0;
            //string password = String.Empty;
            //string Username = String.Empty;
            //string ZoneID = String.Empty;
            //if (sessionUser != null)
            //{
            //    UserID = sessionUser.UserId;
            //    password = sessionUser.Password;
            //    Username = sessionUser.LoginId;
            //}
            //if (MyAppSession.ZoneInfoId > 0)
            //{
            //    ZoneID = MyAppSession.ZoneInfoId.ToString();
            //}

            //var obj = _pgmContext.PGM_uspVoucherPostingForSalary(year, month, Convert.ToInt32(ZoneID)).FirstOrDefault();
            //if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            //{
            //    url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            //}
            var VoucherInfo = new ObjectParameter("VoucherInfo", typeof(string));
            try
            {
                  _pgmContext.CommandTimeout = 0;
                  _pgmContext.PGM_SalaryAutoPVJVVoucher(LoggedUserZoneInfoId, month , year, MyAppSession.UserID, approverId, MyAppSession.EmpId, VoucherInfo);
                  result = VoucherInfo.Value.ToString();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            //url = "/PGM/VoucherTemplate?year="+ year +"&month=" + month;

            return Json(new
            {
                result
            });
        }
        public ActionResult CPFVoucherPosting(string year, string month, int? approverId)
        {
            var result = string.Empty;
            var Message = new ObjectParameter("Message", typeof(string));
            try
            {
                _pgmContext.CommandTimeout = 0;
                _pgmContext.CPF_AutoRVforPFLnCb(LoggedUserZoneInfoId, month, year, MyAppSession.UserID, approverId, MyAppSession.EmpId, Message);

                result = Message.Value.ToString();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return Json(new
            {
                result
            });
        }
        public ActionResult LoanVoucherPosting(string year, string month)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = String.Empty;
            string Username = String.Empty;
            string ZoneID = String.Empty;
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            var obj = _pgmContext.PGM_uspVoucherPostingForLoanReceived(year, month, Convert.ToInt32(ZoneID)).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }

        public ActionResult PFVoucherPosting(string year, string month)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = String.Empty;
            string Username = String.Empty;
            string ZoneID = String.Empty;
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            var obj = _pgmContext.PGM_uspVoucherPostingForPFContribution(year, month, Convert.ToInt32(ZoneID)).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }

        public ActionResult PFLoanVoucherPosting(string year, string month)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = String.Empty;
            string Username = String.Empty;
            string ZoneID = String.Empty;
            if (sessionUser != null)
            {
                UserID = sessionUser.UserId;
                password = sessionUser.Password;
                Username = sessionUser.LoginId;
            }
            if (MyAppSession.ZoneInfoId > 0)
            {
                ZoneID = MyAppSession.ZoneInfoId.ToString();
            }

            var obj = _pgmContext.PGM_uspVoucherPostingForPFLoanReceived(year, month, Convert.ToInt32(ZoneID)).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }

        #endregion

    }
}
