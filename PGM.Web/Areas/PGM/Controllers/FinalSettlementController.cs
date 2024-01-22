using DAL.PGM;
using DAL.PGM.CustomEntities;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.FinalSettlement;
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
    public class FinalSettlementController : Controller
    {
        #region Fields

        private readonly PGMCommonService _pgmCommonService;
        private readonly PGMEntities _pgmContext;

        #endregion

        #region Ctor

        public FinalSettlementController(PGMCommonService pgmCommonservice, PGMEntities pgmContext)
        {
            this._pgmCommonService = pgmCommonservice;
            this._pgmContext = pgmContext;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion 

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, FinalSettlementSearchModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmCommonService.FinalSettlementSearchList(
                model.FromDate == DateTime.MinValue ? (DateTime?)null : model.FromDate,
                model.ToDate == DateTime.MinValue ? (DateTime?)null : model.ToDate,
                model.Division,
                model.EmpID,
                model.FullName
                );

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();
            #region Sorting
            if (request.SortingName == "DateofSettlement")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateofSettlement).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateofSettlement).ToList();
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

            if (request.SortingName == "SalaryPayable")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryPayable).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryPayable).ToList();
                }
            }

            if (request.SortingName == "GratuityPayable")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.GratuityPayable).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.GratuityPayable).ToList();
                }
            }

            if (request.SortingName == "LeaveEncasement")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.LeaveEncasement).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.LeaveEncasement).ToList();
                }
            }

            if (request.SortingName == "OtherAdjustment")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OtherAdjustment).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OtherAdjustment).ToList();
                }
            }

            if (request.SortingName == "ShortageofNoticePeriod")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ShortageofNoticePeriod).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ShortageofNoticePeriod).ToList();
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

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId), new List<object>()
                {
                    d.EmployeeId,
                    d.DateofSettlement.ToString(DateAndTime.GlobalDateFormat),
                    d.FromDate.ToString(DateAndTime.GlobalDateFormat),
                    d.ToDate.ToString(DateAndTime.GlobalDateFormat),
                    d.EmpID,
                    d.FullName,
                    d.Division,
                    d.SalaryPayable,
                    d.GratuityPayable,
                    d.LeaveEncasement,
                    d.NetPFBalance,
                    d.NetPayable,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index()
        {
            var model = new FinalSettlementViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            FinalSettlementViewModel model = new FinalSettlementViewModel();
            model.DateofSettlement = Common.CurrentDateTime;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(FinalSettlementViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                var pgm_finalSettlement = model.ToEntity();
                pgm_finalSettlement.IUser = User.Identity.Name;
                pgm_finalSettlement.IDate = Common.CurrentDateTime;

                try
                {
                    _pgmCommonService.PGMUnit.FinalSettlementRepository.Add(pgm_finalSettlement);
                    _pgmCommonService.PGMUnit.FinalSettlementRepository.SaveChanges();

                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    return RedirectToAction("PrepareVoucher", new { id = pgm_finalSettlement.EmployeeId, type = "success" });

                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }

            model.DateofSettlement = Common.CurrentDateTime;

            return View(model);
        }

        [NoCache]
        private string GetBusinessLogicValidation(FinalSettlementViewModel model)
        {
            string errorMessage = string.Empty;
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            var objEmoployee = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            var gratuityRule = _pgmCommonService.PGMUnit.GratuityPolicyRepository.GetAll().FirstOrDefault();
            var objGratuity = _pgmCommonService.PGMUnit.GratuitySettlementRepository.GetByID(model.EmployeeId, "EmployeeId");

            TimeSpan eligibleServiceLength = Convert.ToDateTime(emp.DateofInactive) - emp.DateofJoining;
            Decimal eligibleSLength = 0;
            eligibleSLength += Math.Round(Convert.ToDecimal(eligibleServiceLength.TotalDays + 1) / 365, 2);
            if (emp.DateofInactive > model.DateofSettlement)
            {
                errorMessage = "Date of final settlement must be equal or higher than the date of separation.";
            }

            //if (objEmoployee.DateofConfirmation != null)
            //{
            //    if (gratuityRule != null && (eligibleSLength >= gratuityRule.EligibleServiceLength))
            //    {
            //        if (objGratuity == null)
            //        {
            //            errorMessage = "Settle gratuity before final settlement.";
            //        }
            //    }
            //    else
            //    {
            //        errorMessage = "Gratuity rule must be needed.";
            //    }
            //}

            return errorMessage;
        }

        [NoCache]
        public ActionResult Edit(int id)
        {

            var finalSettlement = _pgmCommonService.PGMUnit.FinalSettlementRepository.GetByID(id, "EmployeeId");

            FinalSettlementViewModel model = finalSettlement.ToModel();

            if (model.EmployeeId > 0)
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

                model.EmpID = emp.EmpID;
                model.EmployeeInitial = emp.EmployeeInitial;
                model.FullName = emp.FullName;
                model.Division = emp.DivisionName;
                model.Designation = emp.DesignationName;
                model.DateofSeperation = Convert.ToDateTime(emp.DateofInactive).ToString(DateAndTime.GlobalDateFormat);
                model.DateofJoining = Convert.ToDateTime(emp.DateofJoining).ToString(DateAndTime.GlobalDateFormat);
                model.DateofConfirmation = Convert.ToDateTime(emp.DateofConfirmation).ToString(DateAndTime.GlobalDateFormat);
                model.DateofBirth = Convert.ToDateTime(emp.DateofBirth).ToString(DateAndTime.GlobalDateFormat);

                var PFInfo = _pgmCommonService.PGMUnit.CpfSettlementRepository.GetAll().FirstOrDefault(x => x.CPF_MembershipInfo.EmployeeId == model.EmployeeId);
                if (PFInfo != null)
                {
                    model.EmpContribution = PFInfo.EmpContributionInPeriod + PFInfo.EmpOpening;
                    model.ComContribution = PFInfo.ComContributionInPeriod + PFInfo.ComOpening;
                    model.EmpProftInPeriod = PFInfo.EmpProftInPeriod;
                    model.ComProftInPeriod = PFInfo.ComProftInPeriod;
                    model.OtherDeduction = PFInfo.OtherDeduction;
                    model.ForfeitedAmount = PFInfo.ForfeitedAmount;
                    model.WithdrawnAmount = PFInfo.EmpWithdrawnInPeriod;
                    model.DueLoan = PFInfo.DueLoan != null ? Convert.ToDecimal(PFInfo.DueLoan) : default(decimal);
                }
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(FinalSettlementViewModel model)
        {
            string errMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);

            if (ModelState.IsValid)
            {
                model.IsError = 1;
                PGM_FinalSettlement pgm_entity = model.ToEntity();

                pgm_entity.EUser = User.Identity.Name;
                pgm_entity.EDate = Common.CurrentDateTime;
                if (string.IsNullOrEmpty(model.ErrMsg))
                {
                    try
                    {
                        _pgmCommonService.PGMUnit.FinalSettlementRepository.Update(pgm_entity, "EmployeeId");
                        _pgmCommonService.PGMUnit.FinalSettlementRepository.SaveChanges();

                        model.IsError = 0;
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    }
                }
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int? id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            var entity = _pgmCommonService.PGMUnit.FinalSettlementRepository.GetByID(id, "EmployeeId");
            try
            {
                if (entity != null)
                {
                    _pgmCommonService.PGMUnit.FinalSettlementRepository.Delete(entity.EmployeeId, "EmployeeId", null);
                    _pgmCommonService.PGMUnit.FinalSettlementRepository.SaveChanges();

                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                }
                else
                {
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
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
        public JsonResult GetEmployeeInfo(FinalSettlementViewModel model)
        {

            string errorList = string.Empty;
            errorList = GetValidationChecking(model);

            var obj = _pgmCommonService.PGMUnit.FunctionRepository.GetFinalSettlementIndividual2(model.EmployeeId).FirstOrDefault();

            #region Date conversion And Service Length

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (obj != null)
                    {
                        return Json(new
                        {
                            EmpId = obj != null ? obj.EmpID : default(string),
                            FullName = obj != null ? obj.EmployeeName : default(string),
                            Division = obj != null ? obj.Division : default(string),
                            Designation = obj != null ? obj.Designation : default(string),
                            JoiningDate = obj.DateofJoining != null ? Convert.ToDateTime(obj.DateofJoining).ToString(DateAndTime.GlobalDateFormat) : default(string),
                            DateOfConfirmation = obj.DateofConfirmation != null ? Convert.ToDateTime(obj.DateofConfirmation).ToString(DateAndTime.GlobalDateFormat) : default(string),
                            DateOfSeperation = obj.DateofInactive != null ? Convert.ToDateTime(obj.DateofInactive).ToString(DateAndTime.GlobalDateFormat) : default(string),
                            DateOfBirth = obj.DateofBirth != null ? Convert.ToDateTime(obj.DateofBirth).ToString(DateAndTime.GlobalDateFormat) : default(string),
                            ServiceDuration = obj.SurviceDuration != null ? obj.SurviceDuration : default(string),
                            BasicSalary = obj != null ? obj.BasicSalary : default(decimal),
                            SalaryPayable = obj != null ? obj.SalaryPayable : default(decimal),
                            lastMonthWorkedDays = obj != null ? obj.LastMonthWorkingDays : default(int),
                            TotalFullEarnLeave = obj.TotalFullEarnLeave != null ? obj.TotalFullEarnLeave : default(double),
                            TotalEncashLeave = obj.TotalEncashLeave != null ? obj.TotalEncashLeave : default(double),
                            NetEncashLeave = obj.NetEncashLeave != null ? obj.NetEncashLeave : default(double),
                            LeaveEncashAmount = obj.LeaveEncashAmount != null ? obj.LeaveEncashAmount : default(double),
                            EmpContribution = obj.EmpContribution != null ? obj.EmpContribution : default(decimal),
                            ComContribution = obj.ComContribution != null ? obj.ComContribution : default(decimal),
                            EmpProftInPeriod = obj.EmpProftInPeriod != null ? obj.EmpProftInPeriod : default(decimal),
                            ComProftInPeriod = obj.ComProftInPeriod != null ? obj.ComProftInPeriod : default(decimal),
                            OtherDeduction = obj.OtherDeduction,
                            ForfeitedAmount = obj.ForfeitedAmount != null ? obj.ForfeitedAmount : default(decimal),
                            WithdrawnAmount = obj.EmpWithdrawnInPeriod,
                            DueLoan = obj.DueLoan != null ? obj.DueLoan : default(decimal),
                            NetPFBalance = obj.NetPFBalance != null ? obj.NetPFBalance : default(decimal),
                        });
                    }

                    else
                    {
                        return Json(new { Result = false });
                    }

                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
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

            #endregion
        }

        [NoCache]
        private string GetValidationChecking(FinalSettlementViewModel model)
        {
            string errorMessage = string.Empty;
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            var SStruc = _pgmCommonService.PGMUnit.EmpSalaryRepository.GetByID(model.EmployeeId, "EmployeeId");
            var EFinalSettlememt = _pgmCommonService.PGMUnit.FinalSettlementRepository.GetByID(model.EmployeeId, "EmployeeId");

            if (EFinalSettlememt != null)
            {
                errorMessage = "FinalSettlementCompleted";
            }
            if (emp != null)
            {
                if (emp.DateofInactive == null)
                {
                    errorMessage = "ActiveEmployee";
                }

            }
            if (SStruc == null)
            {
                errorMessage = "SalaryStructure";
            }
            return errorMessage;
        }

        [NoCache]
        public JsonResult GetLeaveEncashment(int? id, int? unAdjustedLeave)
        {
            dynamic salaryRate = null;
            dynamic leaveEncashment = null;

            var model = new FinalSettlementViewModel();

            try
            {
                salaryRate = _pgmCommonService.GetSalaryRateByEmployeeID(id);
                leaveEncashment = Math.Round(salaryRate.Rate * unAdjustedLeave, 2);
            }
            catch (Exception ex)
            {
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }

            if (salaryRate != null)
            {
                return this.Json(leaveEncashment, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { leaveEncashment = 0 });
            }
        }

        public ActionResult PrepareVoucher(int id, string type)
        {
            var entity = _pgmCommonService.PGMUnit.FinalSettlementRepository.GetAll().Where(m => m.EmployeeId == id).FirstOrDefault();
            var model = entity.ToModel();
            model.Mode = CrudeAction.Edit;

            if (type == "success")
            {
                model.ErrClss = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            return View(model);
        }
        public ActionResult FinalSettlementVoucherPosting(int employeeId)
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

            var obj = _pgmContext.PGM_uspVoucherPostingForFinalSettlement(employeeId).FirstOrDefault();
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
