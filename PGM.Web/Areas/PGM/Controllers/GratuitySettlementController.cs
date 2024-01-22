using DAL.PGM;
using DAL.PGM.CustomEntities;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.GratuitySettlement;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PGM.Web.Controllers;


namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class GratuitySettlementController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor
        public GratuitySettlementController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, GratuitySettlementSearchModel model)
        {

            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmCommonService.GratuitySettlementSearchList(
                model.FromDate == DateTime.MinValue ? (DateTime?)null : model.FromDate,
                model.ToDate == DateTime.MinValue ? (DateTime?)null : model.ToDate,
                model.PaymentStatus,
                model.EmpID,
                model.FullName
                ).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();
            #region Sorting
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

            if (request.SortingName == "PaymentStatus")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PaymentStatus).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PaymentStatus).ToList();
                }
            }

            if (request.SortingName == "ServiceLength")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ServiceLength).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ServiceLength).ToList();
                }
            }

            if (request.SortingName == "PayableAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PayableAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PayableAmount).ToList();
                }
            }
            if (request.SortingName == "PaymentDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateofPayment).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateofPayment).ToList();
                }
            }

            #endregion
            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId), new List<object>()
                {
                    d.EmployeeId,
                    d.FromDate.ToString(DateAndTime.GlobalDateFormat),
                    d.ToDate.ToString(DateAndTime.GlobalDateFormat),
                    d.EmpID,
                    d.FullName,
                    d.ServiceLength,
                    d.PayableAmount,
                    d.DateofPayment.ToString(DateAndTime.GlobalDateFormat),
                    d.PaymentStatus,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new GratuitySettlementViewModel();
            PrepareModel(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            GratuitySettlementViewModel model = new GratuitySettlementViewModel();
            model.DateofPayment = Common.CurrentDateTime;
            PrepareModel(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(GratuitySettlementViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var pgm_gratuitySettlement = model.ToEntity();
                pgm_gratuitySettlement.IUser = User.Identity.Name;
                pgm_gratuitySettlement.IDate = Common.CurrentDateTime;

                try
                {
                    _pgmCommonService.PGMUnit.GratuitySettlementRepository.Add(pgm_gratuitySettlement);
                    _pgmCommonService.PGMUnit.GratuitySettlementRepository.SaveChanges();

                    model.IsError = 0;
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

            PrepareModel(model);
            model.DateofPayment = Common.CurrentDateTime;
            return View(model);
        }

        [NoCache]
        private string GetBusinessLogicValidation(GratuitySettlementViewModel model)
        {
            string errorMessage = string.Empty;
            var objResignation = _pgmCommonService.PGMUnit.EmpSeperationRepository.GetByID(model.EmployeeId, "EmployeeId");
            if (objResignation == null)
            {
                errorMessage = "Gratuity settlement will be eligible for the resigned employee.";
            }

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            if (emp.DateofConfirmation == null)
            {
                errorMessage = "Employee must be permanent.";
            }

            return errorMessage;
        }

        [NoCache]
        public ActionResult Edit(int id)
        {

            PGM_GratuityPayment pgm_gratuitySettlement = _pgmCommonService.PGMUnit.GratuitySettlementRepository.GetByID(id, "EmployeeId");

            GratuitySettlementViewModel model = pgm_gratuitySettlement.ToModel();

            PrepareModel(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(GratuitySettlementViewModel model)
        {
            PGM_FinalSettlement pgm_finalSettlement = _pgmCommonService.PGMUnit.FinalSettlementRepository.GetByID(model.EmployeeId, "EmployeeId");
            model.IsError = 1;

            if (ModelState.IsValid && pgm_finalSettlement == null)
            {
                PGM_GratuityPayment pgm_entity = model.ToEntity();

                pgm_entity.EUser = User.Identity.Name;
                pgm_entity.EDate = Common.CurrentDateTime;

                try
                {
                    _pgmCommonService.PGMUnit.GratuitySettlementRepository.Update(pgm_entity, "EmployeeId");
                    _pgmCommonService.PGMUnit.GratuitySettlementRepository.SaveChanges();
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
                model.ErrMsg = "Can’t update the record, because final settlement is done";
            }

            PrepareModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            PGM_FinalSettlement pgm_finalSettlement = _pgmCommonService.PGMUnit.FinalSettlementRepository.GetByID(id, "EmployeeId");

            var entity = _pgmCommonService.PGMUnit.GratuitySettlementRepository.GetByID(id, "EmployeeId");
            try
            {
                if (pgm_finalSettlement == null)
                {
                    if (entity != null)
                    {
                        _pgmCommonService.PGMUnit.GratuitySettlementRepository.Delete(entity.EmployeeId, "EmployeeId", null);
                        _pgmCommonService.PGMUnit.GratuitySettlementRepository.SaveChanges();
                        result = true;
                        errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    }
                    else
                    {
                        errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
                    }
                }
                else
                {
                    errMsg = "Can’t delete the record, because final settlement is done";
                }
            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [NoCache]
        public ActionResult GetPaymentStatusList()
        {
            var PaymentStatusList = Common.PopulatePaymentStatusList().ToList();
            return View("_Select", PaymentStatusList);
        }

        [NoCache]
        public JsonResult GetServiceLengthAndGratuityAmount(int employeeId)
        {
            bool isSuccess = false;
            String message = String.Empty;
            String strServiceLength = String.Empty;
            decimal gratuityAmount = 0.0M;

            try
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(employeeId);
                if (emp != null)
                {
                    var gratuityPolicy = _pgmCommonService.PGMUnit.GratuityPolicyRepository.GetAll().FirstOrDefault();
                    if (gratuityPolicy != null)
                    {
                        DateTime fromDate = Common.CurrentDateTime;
                        if (gratuityPolicy.IsEligibleFromJoiningDate)
                            fromDate = emp.DateofJoining;
                        else
                            fromDate = emp.DateofConfirmation == null ? emp.DateofJoining : Convert.ToDateTime(emp.DateofConfirmation);

                        if (gratuityPolicy.EffectiveDate > fromDate)
                        {
                            fromDate = gratuityPolicy.EffectiveDate;
                        }

                        if (emp.DateofInactive != null)
                        {
                            var serviceLength = _pgmCommonService.PGMUnit.FunctionRepository.GetServiceLength(fromDate,
                                Convert.ToDateTime(emp.DateofInactive));

                            var inTotalMonths = (serviceLength.Years * 12) + serviceLength.Months;

                            if (inTotalMonths >= gratuityPolicy.EligibleAfterMonthof)
                            {
                                strServiceLength = serviceLength.Years.ToString() + " year(s) " +
                                         serviceLength.Months.ToString() +
                                         " month(s) " + serviceLength.Days.ToString() + " day(s)";


                                // Last salary month
                                var lastProcessedSalary = (from SM in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                                                           where SM.EmployeeId == employeeId
                                                           select new
                                                           {
                                                               id = SM.Id
                                                           }).OrderBy(x => x.id).ToList().LastOrDefault();

                                if (lastProcessedSalary != null)
                                {
                                    var grossSalaryHeads = _pgmCommonService.PGMUnit.GratuityGrossSalaryHeadRepository
                                        .GetAll().Select(g => g.HeadId).ToList();

                                    // Gratuity gross salary from last salary month
                                    Decimal gratuityGrossSalary =
                                        (from SD in _pgmCommonService.PGMUnit.SalaryDetailsRepository.GetAll()
                                         where SD.HeadId != null && (SD.SalaryId == lastProcessedSalary.id && grossSalaryHeads.Contains((int)SD.HeadId))
                                         select new
                                         {
                                             SD.HeadId,
                                             SD.HeadAmount
                                         }).ToList().Select(s => (decimal?)s.HeadAmount ?? 0).Sum();


                                    TimeSpan eligibleServiceLength = Convert.ToDateTime(emp.DateofInactive) - fromDate;
                                    Decimal decServiceLength = Math.Round(Convert.ToDecimal(eligibleServiceLength.TotalDays) / 365, 2);

                                    gratuityAmount = decServiceLength * gratuityGrossSalary * 2;
                                }

                                isSuccess = true;
                                message = "Success";
                            }
                            else
                            {
                                message = "EligibleAfterMonthDoesNotReachYet";
                            }
                        }
                        else
                        {
                            message = "EmployeeIsActiveYet";
                        }
                    }
                    else
                    {
                        message = "NoGratuityPolicyFound";
                    }
                }
                else
                {
                    message = "NoEmployeeFound";
                }
            }
            catch (Exception ex)
            {
                message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }

            return Json(new
            {
                ServiceLength = strServiceLength,
                GratuityAmount = gratuityAmount,
                Success = isSuccess,
                Message = message
            }, JsonRequestBehavior.AllowGet);
        }
        
        [NoCache]
        private void PrepareModel(GratuitySettlementViewModel model)
        {
            var empList = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeListForDDL(LoggedUserZoneInfoId, false);
            model.EmployeeList = Common.PopulateEmployeeDDL(empList);

            if (model.EmployeeId > 0)
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                model.Designation = emp.DesignationName;
                model.DateofJoining = emp.DateofJoining;
                model.DateofConfirmation = emp.DateofConfirmation;
                model.DateofSeperation = emp.DateofInactive;
            }
        }

        #endregion

    }
}
