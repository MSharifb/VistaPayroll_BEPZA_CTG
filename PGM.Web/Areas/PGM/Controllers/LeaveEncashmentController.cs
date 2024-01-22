using DAL.PGM.CustomEntities;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.LeaveEncashment;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class LeaveEncashmentController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor

        public LeaveEncashmentController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion 

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, LeaveEncashmentSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            // dynamic list = null;

            var list = _pgmCommonService.GetLeaveEncashmentSearchedList(""
                    , request.SortingName
                    , request.SortingOrder.ToString()
                    , request.PageIndex
                    , request.RecordsCount
                    , request.PagesCount.HasValue ? request.PagesCount.Value : 1
                    , false
                    , out totalRecords
                    , model.SalaryYear
                    , model.SalaryMonth
                    , model.ProjectNo
                    , model.EmpID
                    , model.EmployeeInitial
                    , model.FullName
                    , LoggedUserZoneInfoId);

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

            //if (request.SortingName == "EmployeeInitial")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.EmployeeInitial).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.EmployeeInitial).ToList();
            //    }
            //}

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

            //if (request.SortingName == "ProjectNo")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.ProjectNo).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.ProjectNo).ToList();
            //    }
            //}
            if (request.SortingName == "EncasementDays")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EncashmentDays).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EncashmentDays).ToList();
                }
            }
            if (request.SortingName == "EncasementRate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EncashmentRate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EncashmentRate).ToList();
                }
            }
            if (request.SortingName == "EncasementAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EncashmentAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EncashmentAmount).ToList();
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.SalaryYear,
                    d.SalaryMonth,
                    d.EmpID,
                    d.FullName,
                    d.EncashmentDays,
                    d.EncashmentRate,
                    d.EncashmentAmount,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new LeaveEncashmentViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            LeaveEncashmentViewModel model = new LeaveEncashmentViewModel();
            model.PaymentDate = DateTime.Now;
            populateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(LeaveEncashmentViewModel model)
        {
            string errorList = string.Empty;

            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var entity = model.ToEntity();

                try
                {
                    _pgmCommonService.PGMUnit.LeaveEncashment.Add(entity);
                    _pgmCommonService.PGMUnit.LeaveEncashment.SaveChanges();

                    var reult = _pgmCommonService.PGMUnit.FunctionRepository.LMS_LedgerUpdate(Convert.ToInt32(model.intLeaveYearID), model.LeaveTypeId, model.EmpID, "1");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }

            populateDropdown(model);

            return View(model);
        }

        [NoCache]
        private string GetBusinessLogicValidation(LeaveEncashmentViewModel model)
        {
            string errorMessage = string.Empty;
            var employee = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

            if (model.IsAdjustWithSalary && Common.GetInteger(model.SalaryHeadId) == 0)
            {
                errorMessage = "Please select salary head.";
            }
            if (employee.DateofInactive != null)
            {
                errorMessage = "Employee must be active.";
            }
            if (model.EncashmentRate <= 0)
            {
                errorMessage = "Encashment rate must be greater than zero.";
            }
            if (model.EncashmentDays <= 0)
            {
                errorMessage = "Encashment day(s) must be greater than zero.";
            }

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(model.SalaryMonth) || model.SalaryMonth == tr.SalaryMonth)
                                   && (string.IsNullOrEmpty(model.SalaryYear) || model.SalaryYear == tr.SalaryYear)
                                   && (model.EmployeeId == tr.EmployeeId)
                               select tr.EmployeeId)
                               .ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;
            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for the month. Please try another month.";
            }

            return errorMessage;
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(LeaveEncashmentViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var entity = model.ToEntity();
                ArrayList lstGradeSteps = new ArrayList();

                try
                {
                    Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                    _pgmCommonService.PGMUnit.LeaveEncashment.Update(entity, NavigationList);
                    _pgmCommonService.PGMUnit.LeaveEncashment.SaveChanges();

                    var reult = _pgmCommonService.PGMUnit.FunctionRepository.LMS_LedgerUpdate(Convert.ToInt32(model.intLeaveYearID), model.LeaveTypeId, model.EmpID, "1");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }


            populateDropdown(model);

            //readonly fields
            if (model.EmployeeId != 0)
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                model.EmpID = emp.EmpID;
                model.Division = emp.DivisionName;
            }

            return View(model);
        }

        [NoCache]
        public ActionResult GetYearList()
        {
            var model = new LeaveEncashmentViewModel();
            model.YearList = Common.PopulateYearList().ToList();
            return View("_Select", model.YearList);
        }

        [NoCache]
        public ActionResult GetMonthList()
        {
            var monthList = Common.PopulateMonthList().ToList();
            return View("_Select", monthList);
        }

        [NoCache]
        public JsonResult GetLeaveTypeByID(int? year, int? leaveId, int? employeeId)
        {
            dynamic balanceDays = null;

            LeaveEncashmentViewModel model = new LeaveEncashmentViewModel();
            model.IsError = 1;

            try
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(Convert.ToInt32(employeeId));
                balanceDays = _pgmCommonService.GetClosingBalanceByYear(year, leaveId, emp.EmpID).FirstOrDefault();

            }
            catch (Exception ex)
            {
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }

            dynamic result;

            if (balanceDays != null)
            {
                result = new { LeaveYearID = balanceDays.intLeaveYearID, LeaveYear = balanceDays.strYearTitle, LeaveDays = balanceDays.fltCB };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { LeaveYearID = 0, LeaveYear = "", LeaveDays = 0 }, JsonRequestBehavior.AllowGet);
            }
        }

        [NoCache]
        public JsonResult GetEmployeeInfo(LeaveEncashmentViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(model);

            var ObjleaveEncashments = _pgmCommonService.PGMUnit.FunctionRepository.GetLeaveEncashmentIndividual(model.EmployeeId).FirstOrDefault();

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (ObjleaveEncashments != null)
                    {
                        return Json(new
                        {
                            EmpId = ObjleaveEncashments.EmpID != null ? ObjleaveEncashments.EmpID : default(string),
                            EmployeeInitial = ObjleaveEncashments.EmployeeInitial != null ? ObjleaveEncashments.EmployeeInitial : default(string),
                            FullName = ObjleaveEncashments.EmployeeName != null ? ObjleaveEncashments.EmployeeName : default(string),
                            Division = ObjleaveEncashments.Division != null ? ObjleaveEncashments.Division : default(string),
                            AccountNo = ObjleaveEncashments.BankAccountNo != null ? ObjleaveEncashments.BankAccountNo : default(string),
                            BankId = ObjleaveEncashments.BankId,
                            BranchId = ObjleaveEncashments.BankBranchId,
                            BasicSalary = ObjleaveEncashments.BasicSalary,
                            GrossSalary = ObjleaveEncashments.GrossSalary,
                            BankNBranch = ObjleaveEncashments.BankBranchName,
                            Rate = Math.Round(Convert.ToDecimal(ObjleaveEncashments.SalaryRate), 2)
                        });
                    }

                    else
                    {
                        return Json(new { Result = false });
                    }

                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
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
        private string GetValidationChecking(LeaveEncashmentViewModel model)
        {
            string errorMessage = string.Empty;
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

            if (emp != null)
            {
                if (emp.DateofInactive != null)
                {
                    errorMessage = "InactiveEmployee";
                }
            }

            return errorMessage;
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            var model = _pgmCommonService.PGMUnit.LeaveEncashment.GetByID(id);
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(model.SalaryMonth) || model.SalaryMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(model.SalaryYear) || model.SalaryYear == tr.SalaryYear)
                               && (model.EmployeeId == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;

            if (totalRecords > 0)
            {
                result = false;
                errMsg = "Sorry! Salary has been processed.";
            }
            else
            {
                try
                {
                    _pgmCommonService.PGMUnit.LeaveEncashment.Delete(id);
                    _pgmCommonService.PGMUnit.LeaveEncashment.SaveChanges();

                    var reult = _pgmCommonService.PGMUnit.FunctionRepository.LMS_LedgerUpdate(Convert.ToInt32(model.intLeaveYearID), model.LeaveTypeId, emp.EmpID, "1");

                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }
        #endregion 

        #region Utilities
        [NoCache]
        private void populateDropdown(LeaveEncashmentViewModel model)
        {
            //model.ProjectList = Common.PopulateProjectNumber(_pimCommonservice.PIMUnit.ProjectInformation.GetAll().OrderBy(x => x.ProjectNo).ToList());
            model.LeaveTypeList = Common.PopulateLeaveType(_pgmCommonService.PGMUnit.LeaveTypeInfo.GetAll().OrderBy(x => x.strLeaveType).Where(lv => lv.bitIsEncashable == true).ToList());
            model.YearList = Common.PopulateYearList().ToList();
            model.MonthList = Common.PopulateMonthList().ToList();
            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(_pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll().Where(e => e.HeadType == "Addition").ToList());

        }

        #endregion
    }
}
