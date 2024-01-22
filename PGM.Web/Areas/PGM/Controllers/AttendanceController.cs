using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.Attendance;

using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class AttendanceController : BaseController
    {

        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        private List<vwPGMEmploymentInfo> _emps;
        #endregion

        #region Constructor
        public AttendanceController(PGMCommonService pgmCommonService)
        {
            this._pgmCommonservice = pgmCommonService;
        }
        #endregion

        #region Action

        public ActionResult Index()
        {
            var model = new AttendanceViewModel();

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, AttendanceViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            this._emps = _pgmCommonservice
                .PGMUnit
                .FunctionRepository
                .GetEmployeeList()
                .Where(e => e.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId)
                .ToList();

            var list = (from a in _pgmCommonservice.PGMUnit.AttendanceRepository.GetAll()
                        join emp in _emps on a.EmployeeId equals emp.Id
                        select new AttendanceViewModel
                        {
                            AttYear = a.AttYear,
                            AttMonth = a.AttMonth,
                        }).DistinctBy(d => new { d.AttMonth, d.AttYear }).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.EmpID))
                {
                    list = list.Where(t => t.EmpID == model.EmpID).ToList();
                }

                if (!String.IsNullOrEmpty(model.AttYear))
                {
                    list = list.Where(t => t.AttYear == model.AttYear).ToList();
                }

                if (!String.IsNullOrEmpty(model.AttMonth))
                {
                    list = list.Where(t => t.AttMonth == model.AttMonth).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "AttYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AttYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AttYear).ToList();
                }
            }

            if (request.SortingName == "AttMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AttMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AttMonth).ToList();
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
                response.Records.Add(new JqGridRecord(d.AttYear + "-" + d.AttMonth, new List<object>()
                {
                    d.AttYear+"-"+ d.AttMonth,
                    d.AttYear,
                    d.AttMonth,                    
                    "Details"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult GetDetailList(JqGridRequest request, string year, string month, AttendanceViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            this._emps = _pgmCommonservice
                .PGMUnit
                .FunctionRepository
                .GetEmployeeList()
                .Where(e => e.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId)
                .ToList();

            var list = (from a in _pgmCommonservice.PGMUnit.AttendanceRepository.GetAll()
                        join emp in _emps on a.EmployeeId equals emp.Id
                        where a.AttYear.ToString() == year && a.AttMonth.ToString() == month
                        select new AttendanceViewModel
                        {
                            Id = a.Id,
                            EmpID = emp.EmpID,
                            EmployeeId = emp.Id,
                            EmployeeName = emp.FullName,
                            EmployeeDesignation = emp.DesignationName,
                            AttYear = a.AttYear,
                            AttMonth = a.AttMonth,
                            CalenderDays = Convert.ToInt32(a.CalenderDays),
                            AttFromDate = a.AttFromDate,
                            AttToDate = a.AttToDate,
                            TotalPresent = Convert.ToDecimal(a.TotalPresent),
                            TotalCasualLeave = Convert.ToDecimal(a.TotalCasualLeave),
                            TotalEarnedLeave = Convert.ToDecimal(a.TotalEarnedLeave),
                            TotalOthersLeave = Convert.ToDecimal(a.TotalOthersLeave),
                            TotalAttendance = Convert.ToDecimal(a.TotalAttendance),
                            Remark = a.Remark
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
                    d.EmployeeDesignation ,
                    d.AttYear,
                    d.AttMonth,
                    d.CalenderDays,
                    d.TotalPresent,
                    d.TotalAttendance,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new AttendanceViewModel();
            model.ZoneIdDuringAttendance = LoggedUserZoneInfoId;

            populateDropdown(model);
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(AttendanceViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (string.IsNullOrEmpty(model.AccountNo))
                    {
                        model.AccountNo = string.Empty;
                    }

                    var entity = model.ToEntity();
                    _pgmCommonservice.PGMUnit.AttendanceRepository.Add(entity);
                    _pgmCommonservice.PGMUnit.AttendanceRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    return RedirectToAction("GoToDetails", new { idYearMonth = model.AttYear + "-" + model.AttMonth, type = "success" });
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

            populateDropdown(model);
            model.strMode = "Create";

            return View(model);
        }

        public ActionResult Edit(int Id)
        {
            var model = _pgmCommonservice.PGMUnit.AttendanceRepository.GetByID(Id).ToViewModel();

            var emp = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

            model.EmpID = emp == null ? "" : emp.EmpID;
            model.EmployeeName = emp.FullName;
            model.EmployeeDesignation = emp.DesignationName;
            model.AccountNo = emp.BankAccountNo;
            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(AttendanceViewModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (string.IsNullOrEmpty(model.AccountNo))
                    {
                        model.AccountNo = string.Empty;
                    }

                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    var entity = model.ToEntity();

                    _pgmCommonservice.PGMUnit.AttendanceRepository.Update(entity);
                    _pgmCommonservice.PGMUnit.AttendanceRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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

            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _pgmCommonservice.PGMUnit.AttendanceRepository.Delete(id);
                _pgmCommonservice.PGMUnit.AttendanceRepository.SaveChanges();

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
            var model = new AttendanceViewModel();

            string[] yearMonth = idYearMonth.Split('-');
            model.AttYear = yearMonth[0];
            model.AttMonth = yearMonth[1];

            if (!string.IsNullOrEmpty(type) && type == "success")
            {
                model.IsError = 0;
                model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }

            return View("DetailsList", model);
        }

        #endregion

        #region Other

        private string GetBusinessLogicValidation(AttendanceViewModel model)
        {
            if (model.strMode == "Create")
            {
                var r = _pgmCommonservice
                    .PGMUnit
                    .AttendanceRepository
                    .Get(t => t.EmployeeId == model.EmployeeId
                            && t.AttYear == model.AttYear
                            && t.AttMonth == model.AttMonth)
                    .ToList();

                if (r != null && r.Count() != 0)
                {
                    return "Attendance already exist for this month of this employee";
                }
            }
            else if (model.strMode == "Edit")
            {
                var r = _pgmCommonservice.PGMUnit
                    .AttendanceRepository
                    .Get(e => e.EmployeeId == model.EmployeeId
                        && e.AttYear == model.AttYear
                        && e.AttMonth == model.AttMonth && e.Id != model.Id)
                    .ToList();

                if (r != null && r.Count() != 0)
                {
                    return "Attendance already exist for this month of this employee";
                }
            }

            return string.Empty;
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

        private void populateDropdown(AttendanceViewModel model)
        {
            model.YearList = pGetYear();
            model.MonthList = pGetMonth();

            var emp = _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeListForDDL(LoggedUserZoneInfoId);
            model.EmployeeList = Common.PopulateEmployeeDDL(emp);
        }

        private IList<SelectListItem> pGetYear()
        {
            return Common.PopulateYearList();
        }

        private IList<SelectListItem> pGetMonth()
        {
            return Common.PopulateMonthList();
        }

        [NoCache]
        public JsonResult GetMonthDaysAndDate(string year, string month)
        {
            if (String.IsNullOrEmpty(year) || String.IsNullOrEmpty(month))
            {
                return Json(new
                {
                    Success = 1
                }, JsonRequestBehavior.AllowGet);
            }

            int monthInDigit = DateTime.ParseExact(month, "MMMM", CultureInfo.InvariantCulture).Month;
            int days = DateTime.DaysInMonth(Convert.ToInt32(year), monthInDigit);
            DateTime firstDayOfMonth = new DateTime(Convert.ToInt32(year), monthInDigit, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            return Json(new
            {
                Success = 1,
                Days = days,
                FirstDate = firstDayOfMonth,
                LastDate = lastDayOfMonth
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}