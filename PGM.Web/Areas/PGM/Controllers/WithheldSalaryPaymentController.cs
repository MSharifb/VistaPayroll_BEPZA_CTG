using DAL.PGM;
using DAL.PGM.CustomEntities;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.WithheldSalaryPayment;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

/*
Revision History (RH):
		SL	    : 01
		Author	: Md. Amanullah
		Date	: 2016-Jan-26
        SCR     : ERP_BEPZA_PGM_SCR.doc (SCR#54 Case-2)
		Desc.	: Withdraw withheld
		---------
*/

namespace PGM.Web.Areas.PGM.Controllers
{
    public class WithheldSalaryPaymentController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor

        public WithheldSalaryPaymentController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region Message Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, WithheldSalaryPaymentSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<WithheldSalaryPaymentSearchModel> list = null;
            list = (from SM in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                    join E in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on SM.EmployeeId equals E.Id

                    where (string.IsNullOrEmpty(model.SalaryYear) || SM.SalaryYear == model.SalaryYear)
                        && (string.IsNullOrEmpty(model.SalaryMonth) || SM.SalaryMonth == model.SalaryMonth)
                        && SM.IsWithheld == true
                        && E.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId

                    select new WithheldSalaryPaymentSearchModel()
                    {
                        Id = SM.Id,
                        SalaryYear = SM.SalaryYear,
                        SalaryMonth = SM.SalaryMonth,

                    }).Concat(from SM in _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetAll()
                              join E in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on SM.EmployeeId equals E.Id

                              where (string.IsNullOrEmpty(model.SalaryYear) || SM.SalaryYear == model.SalaryYear)
                                  && (string.IsNullOrEmpty(model.SalaryMonth) || SM.SalaryMonth == model.SalaryMonth)
                                  && E.ZoneInfoId == LoggedUserZoneInfoId

                              select new WithheldSalaryPaymentSearchModel()
                              {
                                  Id = SM.SalaryId,
                                  SalaryYear = SM.SalaryYear,
                                  SalaryMonth = SM.SalaryMonth,

                              }).DistinctBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();


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

            #endregion

            foreach (var d in list)
            {

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.SalaryYear,
                    d.SalaryMonth,
                    "Payment",                  
                    "Edit"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new WithheldSalaryPaymentViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Payment(int? id)
        {
            var model = new WithheldSalaryPaymentViewModel();
            var salaryMaster = (from s in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                                where s.Id == id
                                select s).FirstOrDefault();

            if (salaryMaster != null)
            {
                model.SalaryYear = salaryMaster.SalaryYear;
                model.SalaryMonth = salaryMaster.SalaryMonth;

                model.PaymentList = new List<WithheldSalaryPaymentViewModel>();
                foreach (var item in GetSalaryList(salaryMaster.SalaryYear, salaryMaster.SalaryMonth, false))
                {
                    var salary = new WithheldSalaryPaymentViewModel();

                    salary.AccountNo = item.AccountNo;
                    salary.Payable = item.HeadAmount;
                    salary.IsPaid = item.IsPaid;
                    salary.EmployeeId = item.EmployeeId;
                    salary.SalaryId = item.SalaryId;
                    salary.EmployeeInitial = item.EmployeeInitial;
                    salary.FullName = item.FullName;
                    //salary.Mode = "Payment";
                    model.PaymentList.Add(salary);
                }
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Payment(WithheldSalaryPaymentViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = GetBusinessLogicValidation(model);

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    PGM_Salary updateSalaryMaster = null;
                    PGM_WithheldSalaryPayment pgm_withheldSalaryPayment = null;
                    WithheldSalaryPaymentViewModel objPayment = null;

                    foreach (var item in model.PaymentList.Where(x => x.IsPaid == true))
                    {
                        objPayment = new WithheldSalaryPaymentViewModel();

                        objPayment.SalaryId = item.SalaryId;
                        objPayment.EmployeeId = item.EmployeeId;
                        objPayment.SalaryYear = model.SalaryYear;
                        objPayment.SalaryMonth = model.SalaryMonth;
                        objPayment.PaymentDate = item.PaymentDate;
                        objPayment.Payable = item.Payable;
                        objPayment.Remarks = item.Remarks;
                        objPayment.IDate = DateTime.Now;
                        objPayment.IUser = User.Identity.Name;

                        pgm_withheldSalaryPayment = objPayment.ToEntity();

                        _pgmCommonService.PGMUnit.WithheldSalaryPayment.Add(pgm_withheldSalaryPayment);
                        updateSalaryMaster = _pgmCommonService.PGMUnit.SalaryMasterRepository.GetByID(objPayment.SalaryId);
                        updateSalaryMaster.IsPaid = true;
                        updateSalaryMaster.IsWithheld = false; /*RH#01*/

                        _pgmCommonService.PGMUnit.SalaryMasterRepository.SaveChanges();
                    }
                    model.IsError = 0;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General, "Withheld payment is not completed. Please see inner exception.");
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var model = new WithheldSalaryPaymentViewModel();
            model.IsError = 1;

            var salaryMaster = (from s in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                                where s.Id == id
                                select s).FirstOrDefault();
            model.PaymentList = new List<WithheldSalaryPaymentViewModel>();

            if (salaryMaster != null)
            {
                model.SalaryYear = salaryMaster.SalaryYear;
                model.SalaryMonth = salaryMaster.SalaryMonth;
                model.Mode = CrudeAction.Edit;
                foreach (var item in GetSalaryListAfterPay(salaryMaster.SalaryYear, salaryMaster.SalaryMonth, true))
                {
                    var salary = new WithheldSalaryPaymentViewModel();

                    salary.AccountNo = item.AccountNo;
                    salary.Payable = item.HeadAmount;
                    salary.IsPaid = item.IsPaid;
                    salary.EmployeeId = item.EmployeeId;
                    salary.SalaryId = item.SalaryId;
                    salary.EmployeeInitial = item.EmployeeInitial;
                    salary.FullName = item.FullName;

                    model.PaymentList.Add(salary);
                }
            }

            var widthDrawList = (from s in _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetAll()
                                 where s.SalaryMonth == salaryMaster.SalaryMonth && s.SalaryYear == salaryMaster.SalaryYear
                                 select s).ToList();

            var ExistingItem = (from PL in model.PaymentList
                                join WDL in widthDrawList on PL.SalaryId equals WDL.SalaryId
                                select PL).ToList();

            if (salaryMaster != null && ExistingItem != null)
            {
                model.SalaryYear = salaryMaster.SalaryYear;
                model.SalaryMonth = salaryMaster.SalaryMonth;
                //model.PaymentDate = _pgmCommonservice.PGMUnit.WithheldSalaryPayment.GetAll().Where(x => ExistingItem.Any(s=>s.SalaryId == x.SalaryId)).FirstOrDefault().PaymentDate;

                model.PaymentList = new List<WithheldSalaryPaymentViewModel>();
                WithheldSalaryPaymentViewModel salary = null;
                //foreach (var item in GetSalaryList(salaryMaster.SalaryYear, salaryMaster.SalaryMonth, true))
                foreach (var item in ExistingItem)
                {
                    salary = new WithheldSalaryPaymentViewModel();

                    salary.AccountNo = item.AccountNo;
                    salary.PaymentDate = _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetAll().Where(x => x.SalaryId == item.SalaryId).FirstOrDefault().PaymentDate;
                    salary.Payable = item.Payable;
                    salary.IsPaid = item.IsPaid;
                    salary.EmployeeId = item.EmployeeId;
                    salary.SalaryId = item.SalaryId;
                    salary.EmployeeInitial = item.EmployeeInitial;
                    salary.FullName = item.FullName;
                    salary.Remarks = _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetAll().Where(x => x.SalaryId == item.SalaryId).FirstOrDefault().Remarks;
                    salary.Mode = CrudeAction.Edit;
                    model.PaymentList.Add(salary);
                }
            }
            else
            {
                model.ErrMsg = "No record for editing.";
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(WithheldSalaryPaymentViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = GetBusinessLogicValidation(model);

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    WithheldSalaryPaymentViewModel objPayment = null;
                    PGM_WithheldSalaryPayment pgm_withheldSalaryPayment = null;
                    PGM_Salary updateSalaryMaster = null;
                    foreach (var item in model.PaymentList)
                    {
                        objPayment = new WithheldSalaryPaymentViewModel();

                        objPayment.SalaryId = item.SalaryId;
                        objPayment.EmployeeId = item.EmployeeId;
                        objPayment.SalaryYear = model.SalaryYear;
                        objPayment.SalaryMonth = model.SalaryMonth;
                        objPayment.PaymentDate = model.PaymentDate;
                        objPayment.Payable = item.Payable;
                        objPayment.Remarks = model.Remarks;
                        objPayment.IDate = DateTime.Now;
                        objPayment.IUser = User.Identity.Name;

                        pgm_withheldSalaryPayment = objPayment.ToEntity();
                        _pgmCommonService.PGMUnit.WithheldSalaryPayment.Update(pgm_withheldSalaryPayment, "SalaryId");

                        updateSalaryMaster = _pgmCommonService.PGMUnit.SalaryMasterRepository.GetByID(objPayment.SalaryId);
                        updateSalaryMaster.IsPaid = item.IsPaid;

                        _pgmCommonService.PGMUnit.WithheldSalaryPayment.SaveChanges();

                        if (item.IsPaid == false)
                        {
                            _pgmCommonService.PGMUnit.FunctionRepository.DeleteWithheldSalaryPaymentBySalaryID(Convert.ToInt64(item.SalaryId));
                        }
                    }
                    model.IsError = 0;
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

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            var getWithheldSalary = _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetByID(Convert.ToInt64(id), "SalaryId");

            int retn = 0;
            bool result = false;
            string errMsg = string.Empty;

            if (getWithheldSalary != null)
            {
                try
                {
                    retn = _pgmCommonService.PGMUnit.FunctionRepository.DeleteWithheldSalaryPaymentBySalaryID(Convert.ToInt64(id));

                    if (retn == 0)
                    {
                        result = true;
                        errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete, "Withheld delete operation cannot complete. Please see inner exception.");
                }
            }
            else
            {
                errMsg = "Withheld salary must be payment. Then delete.";
            }
            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }


        [HttpPost, ActionName("WithheldRollback")]
        public JsonResult WithheldRollbackConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;
            int retn = 0;

            var getWithheldSalary = _pgmCommonService.PGMUnit.WithheldSalaryPayment.GetByID(Convert.ToInt64(id), "SalaryId");

            if (getWithheldSalary != null)
            {
                try
                {
                    retn = _pgmCommonService.PGMUnit.FunctionRepository.DeleteWithheldSalaryPaymentBySalaryID(Convert.ToInt64(id));

                    if (retn == 0)
                    {
                        result = true;
                        errMsg = "Rollback Successfully Complete.";
                    }
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete, "Withheld rollback cannot complete. Please see inner exception.");
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

        private List<WithheldSalaryPaymentSearchModel> GetSalaryList(string year, string month, bool isPaid)
        {
            var model = (from SM in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                         join SD in _pgmCommonService.PGMUnit.SalaryDetailsRepository.GetAll() on SM.Id equals SD.SalaryId
                         join E in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on SM.EmployeeId equals E.Id
                         where SM.SalaryYear == year && SM.SalaryMonth == month && SM.IsWithheld == true && SM.IsPaid == isPaid
                          && (SD.HeadName == "Net Pay" || SD.HeadName == "Net Payable")
                          && E.ZoneInfoId == LoggedUserZoneInfoId

                         select new WithheldSalaryPaymentSearchModel()
                         {
                             SalaryYear = SM.SalaryYear,
                             SalaryMonth = SM.SalaryMonth,
                             AccountNo = SM.AccountNo,
                             HeadAmount = SD.HeadAmount,
                             IsPaid = SM.IsPaid,
                             EmployeeId = SM.EmployeeId,
                             SalaryId = SM.Id,
                             EmployeeInitial = E.EmpID,
                             FullName = E.FullName
                         }
                ).ToList();
            return model;
        }

        private List<WithheldSalaryPaymentSearchModel> GetSalaryListAfterPay(string year, string month, bool isPaid)
        {
            var model = (from SM in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                         join SD in _pgmCommonService.PGMUnit.SalaryDetailsRepository.GetAll() on SM.Id equals SD.SalaryId
                         join E in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on SM.EmployeeId equals E.Id

                         where SM.SalaryYear == year && SM.SalaryMonth == month && SM.IsWithheld == false && SM.IsPaid == isPaid
                          && (SD.HeadName == "Net Pay" || SD.HeadName == "Net Payable")
                          && E.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId

                         select new WithheldSalaryPaymentSearchModel()
                         {
                             SalaryYear = SM.SalaryYear,
                             SalaryMonth = SM.SalaryMonth,
                             AccountNo = SM.AccountNo,
                             HeadAmount = SD.HeadAmount,
                             IsPaid = SM.IsPaid,
                             EmployeeId = SM.EmployeeId,
                             SalaryId = SM.Id,
                             EmployeeInitial = E.EmpID,
                             FullName = E.FullName
                         }
                ).ToList();
            return model;
        }

        private string GetBusinessLogicValidation(WithheldSalaryPaymentViewModel model)
        {
            string errorMessage = string.Empty;
            var salaryDate = Convert.ToDateTime(model.SalaryYear + "-" + model.SalaryMonth + "01");
            foreach (var item in model.PaymentList)
            {
                if (salaryDate >= item.PaymentDate)
                {
                    errorMessage = "Payment date must higher than the salary date.";
                }
            }
            return errorMessage;
        }

        [NoCache]
        public ActionResult GetYearList()
        {
            var model = new WithheldSalaryPaymentViewModel();
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
        public ActionResult GetPaymentStatusList()
        {
            var PaymentStatusList = Common.PopulatePaymentStatusList().ToList();
            return View("_Select", PaymentStatusList);
        }

        private void populateDropdown(WithheldSalaryPaymentViewModel model)
        {
            model.YearList = Common.PopulateYearList().ToList();
            model.MonthList = Common.PopulateMonthList().ToList();
        }

        #endregion
    }
}
