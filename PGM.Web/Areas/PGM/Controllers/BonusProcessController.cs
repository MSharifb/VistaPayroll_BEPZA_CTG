using DAL.PGM;
using DAL.PGM.CustomEntities;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.BonusProcess;
using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class BonusProcessController : BaseController
    {
        #region Fields

        private readonly PGMCommonService _pgmCommonService;
        private readonly PgmBonusService _pgmBonusService;
        private readonly PGMEntities _pgmContext;

        #endregion

        #region Ctor

        public BonusProcessController(PGMCommonService pgmCommonservice, PgmBonusService pgmBonusService, PGMEntities context)
        {
            _pgmCommonService = pgmCommonservice;
            _pgmBonusService = pgmBonusService;
            _pgmContext = context;
        }

        #endregion

        #region Message Properties
        public string Message { get; set; }

        #endregion

        #region Actions

        [NoCache]
        public ActionResult Index()
        {
            var model = new BonusProcessViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, BonusProcessSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmBonusService.GetBonusMasterData("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false, model.BonusYear, model.BonusMonth, LoggedUserZoneInfoId).OrderBy(x => Convert.ToDateTime(x.BonusYear + "-" + x.BonusMonth + "-01")).ToList();

            totalRecords = _pgmBonusService.GetBonusMasterData("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true, model.BonusYear, model.BonusMonth, LoggedUserZoneInfoId).OrderBy(x => Convert.ToDateTime(x.BonusYear + "-" + x.BonusMonth + "-01")).Count();

            totalRecords = list.Count;

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

            if (request.SortingName == "BonusYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BonusYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BonusYear).ToList();
                }
            }

            if (request.SortingName == "BonusMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BonusMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BonusMonth).ToList();
                }
            }

            if (request.SortingName == "EffectiveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDate).ToList();
                }
            }

            if (request.SortingName == "BonusType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BonusType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BonusType).ToList();
                }
            }

            if (request.SortingName == "BonusAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BonusAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BonusAmount).ToList();
                }
            }

            if (request.SortingName == "AmountType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AmountType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AmountType).ToList();
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
            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(d.Id + "-" + d.BonusYear + "-" + d.BonusMonth, new List<object>()
                {
                    d.Id + "-" + d.BonusYear + "-" + d.BonusMonth,
                    d.BonusYear,
                    d.BonusMonth,
                    d.EffectiveDate.ToString(DateAndTime.GlobalDateFormat),
                    d.IsAll == true ? "All" : "Individual",
                    d.BonusType,
                    d.BonusAmount,
                    d.AmountType,
                    d.RevenueStamp,
                    "Details",
                    "Rollback"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailsList(JqGridRequest request, BonusDetailsSearchModel model, string bonusId)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            int bId = Convert.ToInt32(bonusId);

            var list = _pgmBonusService.GetBonusDetailsList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, model.EmployeeInitial, model.EmpID, model.FullName, model.DivisionId, bId, false).ToList(); ;

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

            if (request.SortingName == "Division")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Division).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Division).ToList();
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


            if (request.SortingName == "BasicSalary")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpBasicSalary).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpBasicSalary).ToList();
                }
            }

            if (request.SortingName == "BonusAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpBonusAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpBonusAmount).ToList();
                }
            }

            if (request.SortingName == "RevenueStamp")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpRevenueStamp).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpRevenueStamp).ToList();
                }
            }
            if (request.SortingName == "NetPayable")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpNetPayable).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpNetPayable).ToList();
                }
            }

            #endregion

            totalRecords = _pgmBonusService.GetBonusDetailsList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, model.EmployeeInitial, model.EmpID, model.FullName, model.DivisionId, bId, true).Count();

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            //list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                var bonus = _pgmCommonService.PGMUnit.BonusMasterRepository.GetByID(d.BonusId);

                response.Records.Add(new JqGridRecord(d.Id + "-" + d.BonusId + "-" + d.EmployeeId + "-" + bonus.BonusYear + "-" + bonus.BonusMonth, new List<object>()
                {
                    d.Id+"-"+d.BonusId + "-" + d.EmployeeId + "-" + bonus.BonusYear + "-" + bonus.BonusMonth,
                    d.EmpID,
                    d.FullName,
                    d.Division,
                    d.DivisionId,
                    d.AccountNo,
                    d.EmpBasicSalary,
                    d.EmpBonusAmount,
                    d.EmpRevenueStamp,
                    d.EmpNetPayable,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new BonusProcessViewModel();
            model.EffectiveDate = Common.CurrentDateTime;
            PopulateDropdown(model);
            model.InfoType = "Individual";

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(BonusProcessViewModel model)
        {
            string errorList = string.Empty;
            int result = 0;

            ApplyBusinessRule(model);

            errorList = GetBusinessLogicValidation(model);

            if (model.AmountType == "Fixed")
            {
                ModelState.Remove("BasicCalculationMonth");
            }
            if (model.IsAll)
            {
                ModelState.Remove("EmployeeId");
            }

            if ((ModelState.IsValid) && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    string errorMsg = string.Empty;
                    result = _pgmCommonService.PGMUnit.FunctionRepository.BonusProcess
                        (
                            model.BonusYear,
                            model.BonusMonth,
                            model.EffectiveDate,
                            model.BonusTypeId,
                            model.AmountType,
                            model.BonusAmount,
                            model.RevenueStamp,
                            model.ReligionId,

                            model.DepartmentId,
                            model.SectionId,
                            model.StaffCategoryId,
                            model.JobGradeId,
                            model.IsAll,
                            model.EmployeeId,
                            LoggedUserZoneInfoId,
                            model.BasicCalculationMonth,

                            model.Remarks,
                            User.Identity.Name,
                            out errorMsg
                        );

                    if (result == 0)
                    {
                        model.IsError = 0;
                        model.ErrMsg = "Bonus Process has been completed successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        if (result == 59998) // custom error code
                        {
                            model.ErrMsg = errorMsg;
                            model.errClass = "failed";
                        }
                        else
                        {
                            model.ErrMsg = "Bonus Process is failed.";
                        }

                        model.IsError = 1;
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
                    model.Message = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
                }
                else
                {
                    model.Message = errorList;
                }
            }

            PopulateDropdown(model);
            return View(model);
        }

        private static void ApplyBusinessRule(BonusProcessViewModel model)
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
        }

        [NoCache]
        public ActionResult BonusDetail(string idBonusDetail)
        {
            string[] ids = idBonusDetail.Split('-');
            var model = new BonusDetailsViewModel();
            model.IsError = 1;
            try
            {
                int intId = Convert.ToInt32(ids[0]);

                var bonus = _pgmCommonService.PGMUnit.BonusMasterRepository.GetByID(intId);

                if (bonus != null)
                {
                    model.BonusId = intId;
                    model.BonusYear = bonus.BonusYear;
                    model.BonusMonth = bonus.BonusMonth;
                    model.BonusType = _pgmCommonService.PGMUnit.BonusTypeRepository.GetByID(Convert.ToInt32(bonus.BonusTypeId)).BonusType;
                }
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
                Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
            return Json(new
            {
                Success = success,
                result = Message

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Rollback")]
        [NoCache]
        public JsonResult ProcessBonusRollback(string idRollback)
        {
            string[] ids = idRollback.Split('-');

            int bonusId = int.Parse(ids[0]);
            string strBonusYear = ids[1];
            string strBonusMonth = ids[2];

            bool result = false;
            string errMsg = "Rollback is failed.";

            try
            {

                errMsg = GetBusinessLogicValidationRollback(strBonusYear, strBonusMonth, false, 0);

                if ((ModelState.IsValid) && string.IsNullOrEmpty(errMsg))
                {
                    int rollbackResult = _pgmCommonService.PGMUnit.FunctionRepository.BonusProcessRollback(bonusId, User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback has been completed successfully.";
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
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

            int bonusId = int.Parse(ids[1]);
            int empId = int.Parse(ids[2]);
            string strBonusYear = ids[3];
            string strBonusMonth = ids[4];

            int rollbackResult = 0;

            bool result = false;
            string errMsg = "Rollback individual is failed.";

            try
            {
                errMsg = GetBusinessLogicValidationRollback(strBonusYear, strBonusMonth, true, empId);

                if ((ModelState.IsValid) && string.IsNullOrEmpty(errMsg))
                {
                    rollbackResult = _pgmCommonService.PGMUnit.FunctionRepository.BonusProcessRollbackIndividaul(bonusId, empId, User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        result = true;
                        errMsg = "Rollback individual has been completed successfully.";
                    }
                }
                else
                {
                    result = false;
                }

            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
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
            var model = new BonusProcessViewModel();
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
            var DivisionList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.DivisionRepository.GetAll().ToList());
            return View("_Select", DivisionList);
        }

        #endregion

        #region Utilities

        [NoCache]
        private string GetBusinessLogicValidation(BonusProcessViewModel model)
        {
            string errorMessage = string.Empty;

            if (model.IsAll == false && model.EmployeeId == 0)
            {
                return "Please select an employee!";
            }

            var bonusType = (from bt in _pgmCommonService.PGMUnit.BonusTypeRepository.GetAll()
                             where bt.Id == model.BonusTypeId
                             select bt).FirstOrDefault();

            if (model.IsAll)
            {
                //*** Search for duplicate record ***//
                var isBonusAlreadyProcessed =
                    (from b in _pgmCommonService.PGMUnit.BonusMasterRepository.GetAll()
                     join bd in _pgmCommonService.PGMUnit.BonusDetailsRepository.GetAll() on b.Id equals bd.BonusId
                     where
                     b.BonusYear == model.BonusYear
                     && b.BonusMonth == model.BonusMonth
                     && b.BonusTypeId == model.BonusTypeId
                     && bd.BonusWithdrawFromZoneId == LoggedUserZoneInfoId
                     select b).ToList();

                if (isBonusAlreadyProcessed.Count > 0)
                {
                    if (bonusType != null)
                    {
                        return bonusType.BonusType + " for " + model.BonusYear + "/" + model.BonusMonth + " is already processed. Please rollback first.";
                    }
                    else
                    {
                        return "Something wrong during duplicate check!";
                    }
                }

                //*** Search for eligible employee ***//
                var zoneWiseEmpList = _pgmCommonService.PGMUnit.FunctionRepository
                    .GetEmployeeList(LoggedUserZoneInfoId, Convert.ToDateTime(model.BonusYear + "-" + model.BonusMonth + "-01"));

                var eligibleEmployees = from e in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                                        join eh in zoneWiseEmpList on e.Id equals eh.Id
                                        where e.IsBonusEligible == true
                                        select new { eh.Id };

                if (eligibleEmployees == null || !eligibleEmployees.Any())
                {
                    return "No bonus eligible employee found!";
                }
            }
            else
            {
                var duplicateSingleCount =
                (from bm in _pgmCommonService.PGMUnit.BonusMasterRepository.GetAll()
                 join bd in _pgmCommonService.PGMUnit.BonusDetailsRepository.GetAll() on bm.Id equals bd.BonusId
                 where bm.BonusYear == model.BonusYear
                       && bm.BonusMonth == model.BonusMonth
                       && bm.BonusTypeId == model.BonusTypeId
                       && bd.EmployeeId == model.EmployeeId
                 select bm).ToList();

                if (duplicateSingleCount.Count > 0)
                {
                    return bonusType.BonusType + " for employee: " + model.EmpID + " in " + model.BonusYear + "/" + model.BonusMonth + " is already processed. Please rollback first.";
                }
            }

            if (model.BonusAmount <= 0)
            {
                return "Bonus amount must be greater than zero!";
            }

            return errorMessage;
        }

        [NoCache]
        public string GetBusinessLogicValidationRollback(string strBonusYear, string strBonusMonth, bool isSingle, int empId)
        {
            StringBuilder errorMessage = new StringBuilder();

            // TODO: Need to validate with bank advice letter.
            if (!isSingle)
            {
                var checkingOut = _pgmBonusService.GetBonusEmployeeExistList(strBonusYear, strBonusMonth).ToList();
                //if (checkingOut.Count() != 0)
                //{
                //    errorMessage.AppendLine().Append("Rollback is failed. Because bank letter already generated. ");
                //}
            }
            else
            {
                var checkBankLetterForSingle = _pgmBonusService.GetBonusIndividualEmployeeExistList(strBonusYear, strBonusMonth, empId).ToList();
                //if (checkBankLetterForSingle.Count() != 0)
                //{
                //    errorMessage.AppendLine().Append("Rollback individual is failed.Because bank letter already generated. ");
                //}
            }

            return errorMessage.ToString();
        }

        [NoCache]
        private void PopulateDropdown(BonusProcessViewModel model)
        {
            model.BonusTypeList = Common.PopulateBonusTypeDDLList(_pgmCommonService.PGMUnit.BonusTypeRepository.GetAll());
            model.ReligionList = Common.PopulateReligionDDL(_pgmCommonService.PGMUnit.Religion.GetAll());
            model.AmountTypeList = Common.PopulateAmountType().ToList();
            model.YearList = Common.PopulateYearList().ToList();
            model.MonthList = Common.PopulateMonthList().ToList();

            model.DepartmentList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.DivisionRepository.GetAll());
            model.SectionList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.SectionRepository.GetAll());
            model.StuffCategoryList = Common.PopulateDDLList(_pgmCommonService.PGMUnit.StaffCategoryRepository.GetAll());
            model.JobGradeList = Common.PopulateJobGradeDDL(_pgmCommonService.GetLatestJobGrade());

            Dictionary<int, string> getBasicFromList = Common.GetEnumAsDictionary<PGMEnum.GetBasicCalculationMonthForBonus>();
            foreach (KeyValuePair<int, string> item in getBasicFromList)
            {
                model.BasicCalculationMonthList.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            // Populate bonus eligible employee list only.
            var bonusEligibleEmp = _pgmCommonService.PGMUnit.FunctionRepository
                .GetEmployeeList(LoggedUserZoneInfoId, true)
                .Where(e => e.IsBonusEligible)
                .ToList();

            var emp = _pgmCommonService.PGMUnit.FunctionRepository
                .GetEmployeeListForDDL(LoggedUserZoneInfoId)
                .Where(e => bonusEligibleEmp.Select(b => b.Id).Contains(e.Id))
                .ToList();

            model.EmployeeList = Common.PopulateEmployeeDDL(emp);
            //--------<<<<<<<

            model.InfoTypeList.Add("All");
            model.InfoTypeList.Add("Individual");
        }

        [NoCache]
        public JsonResult GetReligionByBonusTypeID(int? typeId)
        {
            var religion = (from b in _pgmCommonService.PGMUnit.BonusTypeRepository.GetAll()
                            join r in _pgmCommonService.PGMUnit.Religion.GetAll() on b.ReligionId equals r.Id
                            where b.Id == typeId
                            select new { r.Name, b.ReligionId }).LastOrDefault();

            if (religion != null)
            {
                return this.Json(new
                {
                    religion = religion.Name,
                    ReligionId = religion.ReligionId
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { religion = "All" }, JsonRequestBehavior.AllowGet);
            }
        }

        [NoCache]
        public ActionResult VoucherPosting(int id)
        {
            string url = string.Empty;
            var sessionUser = MyAppSession.User;
            int UserID = 0;
            string password = "";
            string Username = "";
            string ZoneID = "";
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

            var obj = _pgmContext.PGM_uspVoucherPostingForBonus(id).FirstOrDefault();
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
