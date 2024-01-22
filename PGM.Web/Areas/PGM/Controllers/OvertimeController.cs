using DAL.PGM;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.OverTime;
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
    public class OvertimeController : BaseController
    {
        private readonly PGMCommonService _pgmCommonService;
        private readonly PGMEntities _pgmContext;

        public OvertimeController(PGMCommonService pgmCommonservice, PGMEntities pgmContext)
        {
            _pgmCommonService = pgmCommonservice;
            _pgmContext = pgmContext;
        }

        public ActionResult Index()
        {
            var model = new OvertimeModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, OvertimeModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from ot in _pgmCommonService.PGMUnit.OvertimeRepository.GetAll()
                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on ot.EmployeeId equals emp.Id
                        where ot.ZoneIdDuringOvertime == LoggedUserZoneInfoId

                        select new OvertimeModel
                        {
                            OTYear = ot.OTYear,
                            OTMonth = ot.OTMonth,
                        }).DistinctBy(d => new { d.OTMonth, d.OTYear }).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.EmpID))
                {
                    list = list.Where(t => t.EmpID == model.EmpID).ToList();
                }

                if (model.OTYear != 0)
                {
                    list = list.Where(t => t.OTYear == model.OTYear).ToList();
                }

                if (model.OTMonth != 0)
                {
                    list = list.Where(t => t.OTMonth == model.OTMonth).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "OTMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OTMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OTMonth).ToList();
                }
            }

            if (request.SortingName == "OTYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OTYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OTYear).ToList();
                }
            }

            #endregion

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(d.OTYear + "-" + d.OTMonth, new List<object>()
                {
                    d.OTYear+"-"+ d.OTMonth,
                    d.OTYear,
                    UtilCommon.GetMonthName(d.OTMonth),
                    "Details"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult GetDetailList(JqGridRequest request, string year, string month, OvertimeModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<OvertimeModel> list = (from ot in _pgmCommonService.PGMUnit.OvertimeRepository.GetAll()
                                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on ot.EmployeeId equals emp.Id

                                        where ot.OTYear.ToString() == year && ot.OTMonth.ToString() == month
                                              && (model.DesignationId == 0 || emp.DesignationId == model.DesignationId)

                                        && ot.ZoneIdDuringOvertime == LoggedUserZoneInfoId

                                        select new OvertimeModel
                                        {
                                            Id = ot.Id,
                                            EmpID = emp.EmpID,
                                            EmployeeId = emp.Id,
                                            EmployeeName = emp.FullName,
                                            Designation = emp.DesignationName,
                                            AccountNo = ot.AccountNo,
                                            IsImpactToSalary = ot.IsImpactToSalary,
                                            OTYear = ot.OTYear,
                                            OTMonth = ot.OTMonth,
                                            BasicSalary = ot.BasicSalary,
                                            WorkedHours = ot.WorkedHours,
                                            ApprovedHours = ot.ApprovedHours,
                                            OvertimeRate = ot.OvertimeRate,

                                            RevenueStamp = ot.RevenueStamp,
                                            DeductionPercentage = ot.DeductionPercentage,
                                            NetPayable = ot.NetPayable,
                                            Remarks = ot.Remarks,
                                        }).OrderBy(x => x.Id).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.EmpID))
                {
                    list = list.Where(t => t.EmpID == model.EmpID).ToList();
                }

                if (!String.IsNullOrEmpty(model.EmployeeName))
                {
                    list = list.Where(t => t.EmployeeName == model.EmployeeName).ToList();
                }

                //if (model.OTMonth != 0)
                //{
                //    list = list.Where(t => t.OTMonth == model.OTMonth).ToList();
                //}
            }

            #region sorting

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

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                   d.Id,
                    d.EmpID ,
                    d.EmployeeId ,
                    d.EmployeeName ,
                    d.Designation ,
                    d.OTYear,
                    UtilCommon.GetMonthName(d.OTMonth),
                    d.ApprovedHours,

                    d.RevenueStamp,
                    d.DeductionPercentage,
                    d.NetPayable,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new OvertimeModel();

            model.ZoneIdDuringOvertime = LoggedUserZoneInfoId;

            populateDropdown(model);
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(OvertimeModel model)
        {
            string errorList = string.Empty;
            errorList = GetBusinessLogicValidation(model);
            model.IsError = 1;

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    model.IsImpactToSalary = false;

                    var entity = model.ToEntity();
                    _pgmCommonService.PGMUnit.OvertimeRepository.Add(entity);
                    _pgmCommonService.PGMUnit.OvertimeRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    return RedirectToAction("GoToDetails", new { idYearMonth = model.OTYear + "-" + model.OTMonth, type="success" });
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
            model.strMode = "Create";

            return View(model);
        }

        public ActionResult Edit(long? Id)
        {
            var model = _pgmCommonService.PGMUnit.OvertimeRepository.GetByID64Bit(Id).ToModel();
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            var designation = _pgmCommonService.PGMUnit.DesignationRepository.GetByID(model.DesignationId);

            model.EmpID = emp == null ? "" : emp.EmpID;
            model.EmployeeName = emp.FullName;
            model.DesignationId = model.DesignationId;
            model.Designation = designation == null ? "" : designation.Name;
            model.AccountNo = emp.BankAccountNo;
            model.BasicSalary = model.BasicSalary;

            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(OvertimeModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    var entity = model.ToEntity();

                    _pgmCommonService.PGMUnit.OvertimeRepository.Update_64Bit(entity);
                    _pgmCommonService.PGMUnit.OvertimeRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

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
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                _pgmCommonService.PGMUnit.OvertimeRepository.Delete_64Bit(id);
                _pgmCommonService.PGMUnit.OvertimeRepository.SaveChanges();

                return Json(new
                {
                    Success = 1,
                    Message = ErrorMessages.DeleteSuccessful
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GoToDetails(string idYearMonth, string type = "")
        {
            var model = new OvertimeModel();

            string[] yearMonth = idYearMonth.Split('-');
            model.OTYear = Convert.ToInt32(yearMonth[0]);
            model.OTMonth = Convert.ToInt32(yearMonth[1]);
            model.MonthName = UtilCommon.GetMonthName(model.OTMonth);

            if (!string.IsNullOrEmpty(type) && type == "success")
            {
                model.IsError = 0;
                model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }

            return View("DetailsList", model);
        }

        [NoCache]
        [HttpPost]
        public PartialViewResult AddEmployee(OvertimeModel model)
        {
            return PartialView("_PartialDetail", model);
        }

        [NoCache]
        public ActionResult GetYear()
        {
            return PartialView("Select", pGetYear());
        }

        [NoCache]
        public ActionResult GetMonth()
        {
            return PartialView("Select", pGetMonth());
        }

        private void populateDropdown(OvertimeModel model)
        {
            model.YearList = pGetYear();
            model.MonthList = pGetMonth();

            // var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeListForDDL(LoggedUserZoneInfoId);
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetAllEmployeeListForDDL();
            model.EmployeeListForSearch = Common.PopulateEmployeeDDL(emp);
        }

        private IList<SelectListItem> pGetYear()
        {
            return Common.PopulateYearList();
        }

        private IList<SelectListItem> pGetMonth()
        {
            return Common.PopulateMonthList3();
        }

        private string GetBusinessLogicValidation(OvertimeModel model)
        {
            if (string.IsNullOrEmpty(model.AccountNo))
            {
                model.AccountNo = string.Empty;
            }

            if (model.DeductionPercentage == null)
            {
                model.DeductionPercentage = 0;
            }

            var ot = _pgmCommonService.PGMUnit.OvertimeRepository.Get
                (t => t.EmployeeId == model.EmployeeId
                && t.OTYear == model.OTYear
                && t.OTMonth == model.OTMonth).ToList();

            if (ot != null && ot.Count() != 0)
            {
                return "Overtime already exist for this month of this employee";
            }

            return string.Empty;
        }

        public decimal GetYearMonthWiseBasicSalary(int employeeId, string year, string month)
        {
            decimal? basicSalary = _pgmCommonService.PGMUnit.FunctionRepository.GetBasicSalary(employeeId, year, month);
            if (basicSalary != null)
                return Convert.ToDecimal(basicSalary);
            else
                return 0M;
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {
            var Designation = new List<SelectListItem>();

            Designation = _pgmCommonService.PGMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name)
                .ToList()
                .Select(y => new SelectListItem()
                {
                    Text = y.Name,
                    Value = y.Id.ToString()
                }).ToList();

            return PartialView("_Select", Designation);
        }


        public ActionResult PrepareVoucher(long id, string type)
        {
            var entity = _pgmCommonService.PGMUnit.OvertimeRepository.GetByID64Bit(id);
            var model = entity.ToModel();
            //model.tempTotalBankCharge = entity.TotalBankCharge;
            model.strMode = "Edit";
            populateDropdown(model);
            //GetFDRType(model);
            if (type == "success")
            {
                model.errClass = "success";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            return View(model);
        }

        public ActionResult VoucherPosting(string year, int month)
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

            var obj = _pgmContext.PGM_uspVoucherPostingForOvertime(year, Convert.ToInt32(month), 0, Convert.ToInt32(ZoneID)).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }

    }
}