using DAL.PGM.CustomEntities;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.BonusProcess;
using PGM.Web.Areas.PGM.Models.IncentiveBonusProcess;
using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;


namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class IncentiveBonusProcessController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private readonly PgmBonusService _pgmBonusService;
        #endregion

        #region Ctor

        public IncentiveBonusProcessController(PGMCommonService pgmCommonservice, PgmBonusService pgmBonusService)
        {
            _pgmCommonService = pgmCommonservice;
            _pgmBonusService = pgmBonusService;
        }

        #endregion

        #region Message Properties

        public string Message { get; set; }

        #endregion



        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, IncentiveBonusProcessSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmBonusService.GetIncentiveBonusProcess("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false, model.FinancialYear, model.IncentiveBonusDate, model.DepartmentId, model.StaffCategoryId, model.EmploymentTypeId).OrderBy(x => x.Id).ToList();
            totalRecords = list.Count();//_pgmCommonservice.GetIncentiveBonusProcess("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true, model.FinancialYear, model.IncentiveBonusDate, model.DepartmentId, model.StaffCategoryId, model.EmploymentTypeId).Count();

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

            if (request.SortingName == "FinancialYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FinancialYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FinancialYear).ToList();
                }
            }

            if (request.SortingName == "IncentiveBonusDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IncentiveBonusDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IncentiveBonusDate).ToList();
                }
            }

            if (request.SortingName == "DepartmentId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DepartmentId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DepartmentId).ToList();
                }
            }

            if (request.SortingName == "StaffCategoryId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.StaffCategoryId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.StaffCategoryId).ToList();
                }
            }

            if (request.SortingName == "EmploymentTypeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmploymentTypeId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmploymentTypeId).ToList();
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
                response.Records.Add(new JqGridRecord(d.Id.ToString(), new List<object>()
                {
                    d.Id,
                    d.FinancialYear,
                    d.IncentiveBonusDate.ToString(DateAndTime.GlobalDateFormat),
                    d.DepartmentName,
                    d.StaffCategoryName,
                    d.EmploymentTypeName,
                    "Details",
                    "Rollback"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailsList(JqGridRequest request, IncentiveBonusDetailProcessSearchModel model, string IncentiveBonusId)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<IncentiveBonusDetailProcessSearchModel> list = null;

            if (IncentiveBonusId != "")
            {
                int bId = Convert.ToInt32(IncentiveBonusId);

                list = _pgmBonusService.GetIncentiveBonusDetailsList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false, model.EmpID, model.FullName, bId).ToList(); ;
                totalRecords = list.Count(); // _pgmCommonservice.GetBonusDetailsList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true, model.EmployeeInitial, model.EmpID, model.FullName, model.DivisionId, bId).OrderBy(x => x.EmpID).Count();

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

                if (request.SortingName == "BasicSalary")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.BasicSalary).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.BasicSalary).ToList();
                    }
                }

                if (request.SortingName == "IncentiveBonusAmount")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.IncentiveBonusAmount).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.IncentiveBonusAmount).ToList();
                    }
                }

                if (request.SortingName == "RevenueStamp")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.RevenueStamp).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.RevenueStamp).ToList();
                    }
                }
                if (request.SortingName == "NetPayable")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.NetPayable).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.NetPayable).ToList();
                    }
                }

                #endregion
            }

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                var IncentiveBonus = _pgmCommonService.PGMUnit.IncentiveBonusRepository.GetByID(d.IncentiveBonusId);

                response.Records.Add(new JqGridRecord(d.Id.ToString(), new List<object>()
                {
                    d.Id,
                    d.EmpID,
                    d.FullName,
                    d.BasicSalary,
                    d.IncentiveBonusAmount,
                    d.RevenueStamp,
                    d.NetPayable,
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new IncentiveBonusViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new IncentiveBonusViewModel();
            model.IncentiveBonusDate = Common.CurrentDateTime;

            populateDropdown(model);
            model.InfoType = "Individual";

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(IncentiveBonusViewModel model)
        {
            var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();

            string errorList = string.Empty;

            int result = 0;
            errorList = GetBusinessLogicValidation(model);

            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errorList)))
            {
                ApplyBusinessRule(model);

                try
                {
                    result = _pgmCommonService.PGMUnit.FunctionRepository.IncentiveBonusProcess
                        (
                            model.FinancialYear,
                            model.IncentiveBonusDate,
                            model.BonusTypeId,
                            model.OrderDate,
                            model.OrderRefNo,
                            model.Remark,
                            model.FormulaSelect,
                            model.FinancialYearForFormula,
                            model.BasicSalaryCalFromFinancialYear,
                            model.IncentiveBonusDay,
                            model.DayOfMonth,
                            model.TotalNumOfMonth,
                            model.DayOfYear,
                            model.CalenderYearForFormula,
                            model.BasicSalaryCalFromCalenderYear,
                            model.LEELDOY,
                            model.TDoYWELD,
                            model.FormulaFactor,

                            LoggedUserZoneInfoId,
                            model.DepartmentId,
                            model.StaffCategoryId,
                            model.EmploymentTypeId,

                            model.IsAll,
                            model.EmployeeId,
                            model.RevenueStamp,
                            User.Identity.Name,
                            DateTime.Now,
                            null,
                            null
                        );

                    if (result == 0)
                    {
                        model.IsError = 0;
                        model.ErrMsg = "Incentive bonus process has been completed successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        model.IsError = 1;
                        model.ErrMsg = "Bonus Process is failed.";
                    }
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                if (errorList == string.Empty)
                {
                    model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
                }
                else
                {
                    model.ErrMsg = errorList;
                }
            }

            populateDropdown(model);
            return View(model);
        }

        private static void ApplyBusinessRule(IncentiveBonusViewModel model)
        {
            if (model.InfoType.Equals("All"))
            {
                model.IsAll = true;
                model.EmployeeId = 0;
            }
            else
            {
                model.IsAll = false;
            }

            model.FinancialYear = Common.GetFinancialYearByDate(model.IncentiveBonusDate);

            if (model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F1.ToString()))
            {
                //F2
                model.TotalNumOfMonth = null;
                model.DayOfYear = null;

                //F3
                model.CalenderYearForFormula = null;
                model.BasicSalaryCalFromCalenderYear = null;
                model.LEELDOY = null;
                model.TDoYWELD = null;

                model.IncentiveBonusDay = Convert.ToInt32(model.IncentiveBonusDayF1);
                model.DayOfMonth = model.DayOfMonthF1;

                model.FormulaFactor = Convert.ToDecimal(model.IncentiveBonusDay) / Convert.ToDecimal(model.DayOfMonth);
            }
            else if (model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F2.ToString()))
            {
                //F1
                model.DayOfMonth = null;

                //F3
                model.CalenderYearForFormula = null;
                model.BasicSalaryCalFromCalenderYear = null;
                model.LEELDOY = null;
                model.TDoYWELD = null;

                model.DayOfYear = model.DayOfYearF2;
                model.IncentiveBonusDay = Convert.ToInt32(model.IncentiveBonusDayF2);

                model.FormulaFactor = (Convert.ToDecimal(model.TotalNumOfMonth) / Convert.ToDecimal(model.DayOfYear)) * Convert.ToDecimal(model.IncentiveBonusDay);
            }
            else if (model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F3.ToString()))
            {
                //F2
                model.TotalNumOfMonth = null;
                model.DayOfYear = null;

                model.IncentiveBonusDay = Convert.ToInt32(model.IncentiveBonusDayF3);
                model.DayOfMonth = model.DayOfMonthF3;

                model.FormulaFactor = Convert.ToDecimal(model.IncentiveBonusDay) / (Convert.ToDecimal(model.DayOfMonth) * Convert.ToDecimal(model.TDoYWELD));
            }
        }

        [NoCache]
        public ActionResult BonusDetail(string idBonusDetail)
        {
            string[] ids = idBonusDetail.Split('-');
            var model = new IncentiveBonusViewModel();
            model.IsError = 1;

            try
            {
                int intId = Convert.ToInt32(ids[0]);

                var entity = _pgmCommonService.PGMUnit.IncentiveBonusRepository.GetByID(intId);

                model.Id = entity.Id;
                model.FinancialYear = entity.FinancialYear;
                model.OrderRefNo = entity.OrderRefNo;
                model.OrderDate = entity.OrderDate;
                model.IncentiveBonusDate = entity.IncentiveBonusDate;
            }
            catch
            {
                model.ErrMsg = "Id must be an integer number";
            }

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult BonusDetail(BonusDetailsViewModel model)
        {
            var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();

            bool success = true;
            string Message = string.Empty;

            try
            {
                var bonusEntity = model.ToEntity();
                _pgmCommonService.PGMUnit.BonusDetailsRepository.Add(bonusEntity);
                _pgmCommonService.PGMUnit.BonusDetailsRepository.SaveChanges();

                Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            catch (Exception ex)
            {
                success = false;
                Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }
            return Json(new
            {
                Success = success,
                result = Message

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Rollback")]
        [NoCache]
        public JsonResult ProcessIncentiveBonusRollback(string idRollback)
        {
            string[] ids = idRollback.Split('-');

            int rollbackResult = 0;
            //var checkingOut = _pgmCommonservice.GetBonusEmployeeExistList(ids[1], ids[2]).ToList();
            bool result = false;
            string errMsg = "Rollback is failed.";

            try
            {
                if (ModelState.IsValid)
                {
                    rollbackResult = _pgmCommonService.PGMUnit.FunctionRepository.IncentiveBonusProcessRollback(Convert.ToInt32(ids[0]), User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
                else
                {
                    result = false;
                    errMsg = "Rollback is failed.Because bank letter already generated. ";
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

        [HttpPost, ActionName("RollbackIndividual")]
        [NoCache]
        public JsonResult RollbackProcessBonusIndividual(string idRollback)
        {
            string[] ids = idRollback.Split('-');

            int rollbackResult = 0;
            //var checkingOut = _pgmCommonservice.GetBonusIndividualEmployeeExistList(ids[3], ids[4], Convert.ToInt32(ids[1])).ToList();
            bool result = false;
            string errMsg = "Rollback individual is failed.";

            try
            {
                if (ModelState.IsValid)// && checkingOut.Count() == 0)
                {
                    rollbackResult = _pgmCommonService.PGMUnit.FunctionRepository.IncentiveBonusProcessRollbackIndividaul(Convert.ToInt32(ids[0]), User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback individual has been completed successfully.";
                    }
                }
                else
                {
                    result = false;
                    errMsg = "Rollback individual is failed.Because bank letter already generated. ";
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
        public JsonResult GetEmployeeInfo(IncentiveBonusViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(model.EmployeeId);

            var obj = _pgmCommonService.PGMUnit.FunctionRepository.EmployeeForBonusIndividual(model.EmployeeId, string.Empty).FirstOrDefault();

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpID = obj != null ? obj.EmpID : default(string),
                            EmployeeId = obj != null ? obj.Id : default(int),
                            FullName = obj != null ? obj.EmployeeName : default(string),
                            AccountNo = obj != null ? obj.BankAccountNo : default(string),
                            DivisionId = obj != null ? obj.DivisionId : default(int),
                            DesignationId = obj != null ? obj.DesignationId : default(int),
                            DesignationName = obj != null ? obj.DesignationName : default(string),
                            BankId = obj != null ? obj.BankId : default(int),
                            BranchId = obj != null ? obj.BankBranchId : default(int),
                            BasicSalary = obj != null ? obj.BasicSalary : default(decimal),
                            JobGradeId = obj != null ? obj.GradeId : default(int)
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception)
                {
                    return Json(new { Result = false });
                }
            }
            else
            {
                return Json(new
                {
                    Result = errorList
                });
            }
        }

        [NoCache]
        private string GetValidationChecking(int employeeId)
        {
            string errorMessage = string.Empty;
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(employeeId);
            if (emp != null)
            {
                if (emp.DateofInactive != null)
                {
                    errorMessage = "InactiveEmployee";
                }
            }

            return errorMessage;
        }

        [NoCache]
        public ActionResult GetFinancialYearList()
        {
            // need to verify
           // var FinancialYearList = Common.PopulateFinancialYearDllList2(_famCommonservice.FAMUnit.FinancialYearInformationRepository.GetAll().OrderByDescending(t => t.FinancialYearStartDate));
            return View("_Select", null);
        }

        #region Utilities

        [NoCache]
        private string GetBusinessLogicValidation(IncentiveBonusViewModel model)
        {
            string errorMessage = string.Empty;

            if (model.InfoType == "Individual" && model.EmployeeId == 0)
            {
                errorMessage = "Please select an employee";
                return errorMessage;
            }

            DateTime BasicSalCalFromDate = DateTime.MinValue;

            if (
                model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F1.ToString())
                || model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F2.ToString())
                )
            {
                BasicSalCalFromDate = Convert.ToDateTime(model.BasicSalaryCalFromFinancialYear);
            }

            if (model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F3.ToString()))
            {
                BasicSalCalFromDate = Convert.ToDateTime(model.BasicSalaryCalFromCalenderYear);
            }

            var employeeSalary = (from s in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll().Where(x => (Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")) == Convert.ToDateTime(BasicSalCalFromDate.Year + "-" + BasicSalCalFromDate.Month + "-01")) select s).ToList();

            if (employeeSalary.Count == 0)
            {
                errorMessage = "Salary process of month " + BasicSalCalFromDate.Year + "/" + BasicSalCalFromDate.Month + " must be exist for Incentive bonus.";
                return errorMessage;
            }

            if (model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F1.ToString()))
            {
                if (model.DayOfMonthF1 == null || model.DayOfMonthF1 == 0)
                {
                    errorMessage = "Please give day of month for Formula A";
                    return errorMessage;
                }
                if (model.IncentiveBonusDayF1 == null || model.IncentiveBonusDayF1 == 0)
                {
                    errorMessage = "Please give incentive bonus day for Formula A";
                    return errorMessage;
                }
            }
            else if (model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F2.ToString()))
            {
                if (model.TotalNumOfMonth == null || model.TotalNumOfMonth == 0)
                {
                    errorMessage = "Please give total number of month for Formula B";
                    return errorMessage;
                }

                if (model.DayOfYearF2 == null || model.DayOfYearF2 == 0)
                {
                    errorMessage = "Please give day of year for Formula B";
                    return errorMessage;
                }

                if (model.IncentiveBonusDayF2 == null || model.IncentiveBonusDayF2 == 0)
                {
                    errorMessage = "Please give incentive bonus day for Formula B";
                    return errorMessage;
                }
            }
            else if (model.FormulaSelect.Equals(PGMEnum.IncentiveBonusFormulaSelect.F3.ToString()))
            {
                if (model.DayOfYearF3 == null || model.DayOfYearF3 == 0)
                {
                    errorMessage = "Please give day of year for Formula C";
                    return errorMessage;
                }

                if (model.LEELDOY == null || model.LEELDOY == 0)
                {
                    errorMessage = "Please give less entitle earn leave for Formula C";
                    return errorMessage;
                }

                if (model.TDoYWELD == null || model.TDoYWELD == 0)
                {
                    errorMessage = "Please give without entitle leave days for Formula C";
                    return errorMessage;
                }

                if (model.IncentiveBonusDayF3 == null || model.IncentiveBonusDayF3 == 0)
                {
                    errorMessage = "Please give incentive bonus day for Formula C";
                    return errorMessage;
                }

                if (model.DayOfMonthF3 == null || model.DayOfMonthF3 == 0)
                {
                    errorMessage = "Please give day of month for Formula C";
                    return errorMessage;
                }
            }

            if (model.RevenueStamp <= 0.0M)
            {
                errorMessage = "R/S amount must be greater than zero.";
                return errorMessage;
            }

            if (model.IncentiveBonusDate < BasicSalCalFromDate)
            {
                errorMessage = "Incentive Bonus payment date must be greater then calculation from date";
                return errorMessage;
            }

            return errorMessage;
        }

        [NoCache]
        private void populateDropdown(IncentiveBonusViewModel model)
        {
            model.AmountTypeList = Common.PopulateAmountType().ToList();

            model.DepartmentList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.DivisionRepository.GetAll());
            model.StuffCategoryList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.StaffCategoryRepository.GetAll());
            model.EmploymentTypeList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.EmploymentTypeRepository.GetAll());
            //model.FinancialYearList = Common.PopulateFinancialYearDllList2(_pgmCommonService.PGMUnit.FinancialYearInformationRepository.GetAll().OrderByDescending(t => t.FinancialYearStartDate));
            model.CalenderYearList = Common.PopulateYearList();
            model.BonusTypeList = Common.PopulateBonusTypeDDLList(_pgmCommonService.PGMUnit.BonusTypeRepository.GetAll());

            string headType = PGMEnum.SalaryHeadType.Addition.ToString();
            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(_pgmCommonService.PGMUnit.SalaryHeadRepository.Get(t => t.HeadType == headType).ToList());

            model.InfoTypeList.Add("All");
            model.InfoTypeList.Add("Individual");
        }

        [NoCache]
        public JsonResult GetEmployeeInfoForAutoComplete(string EmpID)
        {
            string errorList = string.Empty;
            //errorList = GetValidationChecking(model.EmployeeId);

            var obj = _pgmCommonService.PGMUnit.FunctionRepository.EmployeeForBonusIndividual(0, EmpID).FirstOrDefault();

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpID = obj != null ? obj.EmpID : default(string),
                            EmployeeId = obj != null ? obj.Id : default(int),
                            FullName = obj != null ? obj.EmployeeName : default(string),
                            AccountNo = obj != null ? obj.BankAccountNo : default(string),
                            DivisionId = obj != null ? obj.DivisionId : default(int),
                            DesignationId = obj != null ? obj.DesignationId : default(int),
                            DesignationName = obj != null ? obj.DesignationName : default(string),
                            BankId = obj != null ? obj.BankId : default(int),
                            BranchId = obj != null ? obj.BankBranchId : default(int),
                            BasicSalary = obj != null ? obj.BasicSalary : default(decimal),
                            JobGradeId = obj != null ? obj.GradeId : default(int)
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch
                {
                    return Json(new { Result = false });
                }
            }
            else
            {
                return Json(new
                {
                    Result = errorList
                });
            }
        }

        [NoCache]
        public ActionResult GetEmployeeAutoComplete(string searchString)
        {
            var emps = from s in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                    .Where(x => x.DateofInactive == null && x.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId)
                    .ToArray()
                select s.EmpID;

            var empInitial = emps.Where(item => item.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) == 0);
            return Json(empInitial, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
