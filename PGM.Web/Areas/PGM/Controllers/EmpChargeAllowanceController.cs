using Domain.PGM;
using PGM.Web.Areas.PGM.Models.EmpChargeAllowance;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class EmpChargeAllowanceController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private static string message;
        #endregion

        #region Ctor

        public EmpChargeAllowanceController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        [NoCache]
        public ActionResult Index(string message, bool? isSuccess)
        {
            var model = new EmpChargeAllowanceModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public JsonResult GetList(JqGridRequest request, EmpChargeAllowanceModel model)
        {
            int totalRecords = 0;

            var list = (from ca in _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.GetAll()
                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on ca.EmployeeId equals emp.Id

                        where emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                        select new EmpChargeAllowanceModel
                        {
                            Id = ca.Id,
                            EmpID = emp.EmpID,
                            FullName = emp.FullName,
                            DesignationId = emp.DesignationId,
                            Designation = emp.DesignationName,
                            StartDate = ca.StartDate,
                            EndDate =ca.EndDate,
                            MonthlyAllowance = ca.MonthlyAllowance
                        }).ToList();

            totalRecords = list.Count;
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            #region Filter
            if (!String.IsNullOrEmpty(model.EmpID))
            {
                list = list.Where(t => t.EmpID.Contains(model.EmpID)).ToList();
            }

            // Employee Name
            if (model.DesignationId != 0)
            {
                list = list.Where(t => t.DesignationId == model.DesignationId).ToList();
            }
            #endregion

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

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmpID,
                    d.FullName,
                    d.Designation,
                    Convert.ToDateTime(d.StartDate).ToString(DateAndTime.GlobalDateFormat),
                    d.EndDate == null || d.EndDate == DateTime.MinValue  ? "" : Convert.ToDateTime(d.EndDate).ToString(DateAndTime.GlobalDateFormat),
                    d.MonthlyAllowance,
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new EmpChargeAllowanceModel();
            PrepareModel(model);

            model.BasicSalary = _pgmCommonService.PGMUnit.FunctionRepository.GetBasicSalary(model.EmployeeId);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(EmpChargeAllowanceModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.Mode = CrudeAction.AddNew;

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    ApplyBusinessLogic(model);

                    var entity = model.ToEntity();

                    _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.Add(entity);
                    _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return RedirectToAction("Index", new { message = model.Message, isSuccess = true });
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                model.Message = errorList;
            }

            if (model.IsError == 0)
                ViewBag.SuccessMessage = model.Message;
            else
                ViewBag.ErrorMessage = model.Message;

            PrepareModel(model);

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(long? id)
        {
            var entity = _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.GetByID64Bit(id);
            var model = entity.ToModel();

            PrepareModel(model);

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(EmpChargeAllowanceModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.Mode = CrudeAction.Edit;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    if (model.Id != 0)
                    {
                        var entity = model.ToEntity();

                        _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.Update_64Bit(entity);
                        _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.SaveChanges();

                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        model.IsError = 0;

                        return RedirectToAction("Index", new { message = model.Message, isSuccess = true });
                    }
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.IsError = 1;
                model.Message = errorList;
            }

            model.Mode = CrudeAction.Edit;

            if (model.IsError == 0)
            {
                return View("Index", model);
            }

            PrepareModel(model);

            ViewBag.ErrorMessage = model.Message;
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(long id)
        {
            bool isSuccessful = false;
            string errMsg = string.Empty;
            var entity = _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.GetByID64Bit(id);

            if (entity != null)
            {
                var model = entity.ToModel();
                model.Mode = CrudeAction.Delete;
                errMsg = GetBusinessLogicValidation(model);
            }

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    if (id != 0)
                    {
                        _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.Delete_64Bit(id);
                        _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.SaveChanges();

                        message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                        isSuccessful = true;
                    }
                }
                catch (Exception ex)
                {
                    isSuccessful = false;
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }
            else
            {
                isSuccessful = false;
            }

            return Json(new
            {
                Success = isSuccessful,
                Message = isSuccessful == true ? message : errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        public JsonResult GetMonthlyAllowance(int employeeId)
        {
            try
            {
                Decimal basicSalary = _pgmCommonService.PGMUnit.FunctionRepository.GetBasicSalary(employeeId);
                var chargeAllowanceEntity = _pgmCommonService.PGMUnit.ChargeAllowanceRuleRepositoty.GetAll().SingleOrDefault();

                decimal monthlyAllowance = 0M;
                if (chargeAllowanceEntity != null)
                {
                    monthlyAllowance = chargeAllowanceEntity.ChargeAllowancePercent * basicSalary / 100;

                    if (monthlyAllowance > chargeAllowanceEntity.MaxChargeAllowance)
                    {
                        monthlyAllowance = chargeAllowanceEntity.MaxChargeAllowance;
                    }
                }

                return Json(new
                {
                    MonthlyAllowance = monthlyAllowance,
                    Success = true,
                    Message = "Success"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                return Json(new { Success = false, Message = errMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {
            var list = _pgmCommonService.PGMUnit.DesignationRepository.GetAll();
            var listDes = Common.PopulateDDLList(list);

            return PartialView("_Select", listDes);
        }

        #region Utilities

        private void ApplyBusinessLogic(EmpChargeAllowanceModel model)
        {
            if (Convert.ToBoolean(model.IsContinuous) == true)
            {
                model.EndDate = null;
            }

            if (model.EDate.Equals(DateTime.MinValue))
            {
                model.EDate = null;
            }
        }

        private void PrepareModel(EmpChargeAllowanceModel model)
        {
            var chargeAllowanceRule = _pgmCommonService.PGMUnit.ChargeAllowanceRuleRepositoty.GetAll().SingleOrDefault();
            model.MaxAllowance = chargeAllowanceRule.MaxChargeAllowance;

            var empList = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeListForDDL(LoggedUserZoneInfoId);
            model.EmployeeList = Common.PopulateEmployeeDDL(empList);

            if (model.EmployeeId > 0)
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                model.DesignationId = emp.DesignationId;
                model.Designation = emp.DesignationName;
                model.Department = emp.DivisionName;
            }
        }

        private string GetBusinessLogicValidation(EmpChargeAllowanceModel model)
        {
            if (model.EmployeeId == 0)
            {
                return "Please select an employee.";
            }

            if (model.Mode.Equals(CrudeAction.Delete) == false)
            {
                if (model.MaxAllowance < model.MonthlyAllowance)
                {
                    return "Monthly allowance is exceed Maximum charge";
                }
            }

            if (model.IsContinuous == false && model.EndDate == null)
            {
                return "Please choose 'End Date'";
            }

            if (model.Mode.Equals(CrudeAction.AddNew))
            {
                string query = String.Empty;
                query = @"SELECT * FROM PGM_Salary sal";
                query += " WHERE sal.EmployeeId = " + model.EmployeeId;
                query += " AND '" + Convert.ToDateTime(model.StartDate).ToString("yyyy-MMM-dd") +
                         "' <= Convert (Date, sal.SalaryYear + ' ' + sal.SalaryMonth + ' 01')";

                if (model.EndDate != null)
                {
                    query +=
                        " AND CONVERT(VARCHAR(25),DATEADD(dd,-(DAY(DATEADD(mm,1,CONVERT (DATE,sal.SalaryYear + ' ' + sal.SalaryMonth + ' 01' )))),DATEADD(mm,1,CONVERT (DATE,sal.SalaryYear + ' ' + sal.SalaryMonth + ' 01' ))),101)"
                        + " <= " + "'" + Convert.ToDateTime(model.EndDate).ToString("yyyy-MMM-dd") + "'";
                }

                var salaryChecking = _pgmCommonService.PGMUnit.SalaryMasterRepository.GetWithRawSql(query)
                    .FirstOrDefault();

                if (salaryChecking != null)
                {
                    return "Salary has been processed for the month. " + model.Mode + " is not acceptable.";
                }
            }

            var empChargeAllowance = _pgmCommonService.PGMUnit.EmpChargeAllowanceRepositoty.Get(t => t.EmployeeId == model.EmployeeId).OrderByDescending(t => t.StartDate).FirstOrDefault();

            if (empChargeAllowance != null)
            {
                if (model.Mode.Equals(CrudeAction.AddNew))
                {
                    if (Convert.ToBoolean(empChargeAllowance.IsContinuous) == true)
                    {
                        return "Continuous charge allowance found for this employee. Could not apply charge allowance.";
                    }
                    else if (empChargeAllowance.StartDate <= model.StartDate && model.StartDate <= empChargeAllowance.EndDate)
                    {
                        return "Another charge allowance found for this employee. Start date must be greater then " + Convert.ToDateTime(empChargeAllowance.EndDate).ToString(DateAndTime.GlobalDateFormat) + ".";
                    }
                }

                if (model.Mode.Equals(CrudeAction.Edit))
                {
                    if (model.StartDate < empChargeAllowance.StartDate)
                    {
                        return "Another charge allowance found for this employee. Start date must be equal or greater then " + empChargeAllowance.StartDate.ToString(DateAndTime.GlobalDateFormat) + ".";
                    }
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
