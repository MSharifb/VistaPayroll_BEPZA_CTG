using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.Refreshment;
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
    public class RefreshmentController : BaseController
    {
        //
        // GET: /PGM/Refreshment/
        private readonly PGMCommonService _pgmCommonService;
        private readonly PGMEntities _pgmContext;

        public RefreshmentController(PGMCommonService pgmCommonService, PGMEntities pgmContext)
        {
            this._pgmCommonService = pgmCommonService;
            this._pgmContext = pgmContext;
        }

        #region Action

        public ActionResult Index()
        {
            var model = new RefreshmentViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, RefreshmentViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            var list = (from r in _pgmCommonService.PGMUnit.RefreshmentRepository.GetAll()
                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on r.EmployeeId equals emp.Id
                        where emp.ZoneInfoId == LoggedUserZoneInfoId
                        //where emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                        select new RefreshmentViewModel
                        {
                            RYear = r.RYear,
                            RMonth = r.RMonth,
                        }).DistinctBy(d => new { d.RMonth, d.RYear }).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.EmpID))
                {
                    list = list.Where(t => t.EmpID == model.EmpID).ToList();
                }

                if (!String.IsNullOrEmpty(model.RYear))
                {
                    list = list.Where(t => t.RYear == model.RYear).ToList();
                }

                if (!String.IsNullOrEmpty(model.RMonth))
                {
                    list = list.Where(t => t.RMonth == model.RMonth).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "RYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RYear).ToList();
                }
            }

            if (request.SortingName == "RMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RMonth).ToList();
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
                response.Records.Add(new JqGridRecord(d.RYear + "-" + d.RMonth, new List<object>()
                {
                    d.RYear+"-"+ d.RMonth,
                    d.RYear,
                    d.RMonth,
                    "Details"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult GetDetailList(JqGridRequest request, string year, string month, RefreshmentViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<RefreshmentViewModel> list = (from r in _pgmCommonService.PGMUnit.RefreshmentRepository.GetAll()
                                               join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on r.EmployeeId equals emp.Id
                                               where r.RYear.ToString() == year && r.RMonth.ToString() == month
                                               && emp.ZoneInfoId == LoggedUserZoneInfoId
                                               //emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId

                                               select new RefreshmentViewModel
                                               {
                                                   Id = r.Id,
                                                   EmpID = emp.EmpID,
                                                   EmployeeId = emp.Id,
                                                   EmployeeName = emp.FullName,
                                                   Designation = emp.DesignationName,
                                                   RYear = r.RYear,
                                                   RMonth = r.RMonth,
                                                   PerDayAmount = Convert.ToDecimal(r.PerDayAmount),
                                                   TotalDays = Convert.ToDecimal(r.TotalDays),
                                                   RevenueStamp = Convert.ToDecimal(r.RevenueStamp),
                                                   NetPayable = Convert.ToDecimal(r.NetPayable),
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
                    d.RYear,
                    d.RMonth,
                    d.PerDayAmount,
                    d.TotalDays,
                    d.RevenueStamp,
                    d.NetPayable,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new RefreshmentViewModel();
            model.ZoneIdDuringRefreshment = LoggedUserZoneInfoId;

            populateDropdown(model);
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(RefreshmentViewModel model)
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
                    _pgmCommonService.PGMUnit.RefreshmentRepository.Add(entity);
                    _pgmCommonService.PGMUnit.RefreshmentRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    return RedirectToAction("GoToDetails", new { idYearMonth = model.RYear + "-" + model.RMonth, type = "success" });
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
            var model = _pgmCommonService.PGMUnit.RefreshmentRepository.GetByID(Id).ToViewModel();

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

            model.EmpID = emp == null ? "" : emp.EmpID;
            model.EmployeeName = emp.FullName;
            model.DesignationId = emp.DesignationId;
            model.Designation = emp.DesignationName;
            model.AccountNo = emp.BankAccountNo;

            populateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(RefreshmentViewModel model)
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

                    _pgmCommonService.PGMUnit.RefreshmentRepository.Update(entity);
                    _pgmCommonService.PGMUnit.RefreshmentRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    //return RedirectToAction("Index");
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
                _pgmCommonService.PGMUnit.RefreshmentRepository.Delete(id);
                _pgmCommonService.PGMUnit.RefreshmentRepository.SaveChanges();

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
            var model = new RefreshmentViewModel();

            string[] yearMonth = idYearMonth.Split('-');
            model.RYear = yearMonth[0];
            model.RMonth = yearMonth[1];

            if (!string.IsNullOrEmpty(type) && type == "success")
            {
                model.IsError = 0;
                model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }


            return View("DetailsList", model);
        }

        #endregion

        #region Other

        private string GetBusinessLogicValidation(RefreshmentViewModel model)
        {
            if (model.strMode == "Create")
            {
                var r = _pgmCommonService.PGMUnit.RefreshmentRepository.Get
                (t => t.EmployeeId == model.EmployeeId
                && t.RYear == model.RYear
                && t.RMonth == model.RMonth).ToList();

                if (r != null && r.Count() != 0)
                {
                    return "Refreshment already exist for this month of this employee";
                }
            }
            else if (model.strMode == "Edit")
            {
                var r = _pgmCommonService.PGMUnit.RefreshmentRepository.Get
                (e => e.EmployeeId == model.EmployeeId
                && e.RYear == model.RYear
                && e.RMonth == model.RMonth && e.Id != model.Id).ToList();

                if (r != null && r.Count() != 0)
                {
                    return "Refreshment already exist for this month of this employee";
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

        private void populateDropdown(RefreshmentViewModel model)
        {
            model.YearList = pGetYear();
            model.MonthList = pGetMonth();

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetAllEmployeeListForDDL();
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

        public ActionResult VoucherPosting(string year, string month, int EmployeeId)
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

            var obj = _pgmContext.PGM_uspVoucherPostingForRefreshment(year, month, EmployeeId, Convert.ToInt32(ZoneID)).FirstOrDefault();
            if (obj != null && obj.VouchrTypeId > 0 && obj.VoucherId > 0)
            {
                url = System.Configuration.ConfigurationManager.AppSettings["VPostingUrl"].ToString() + "/Account/LoginVoucherQ?userName=" + Username + "&password=" + password + "&ZoneID=" + ZoneID + "&FundControl=" + obj.FundControlId + "&VoucherType=" + obj.VouchrTypeId + "&VoucherTempId=" + obj.VoucherId;
            }

            return Json(new
            {
                redirectUrl = url
            });
        }

        public ActionResult PrepareVoucher(int id, string type)
        {
            var refreshment = _pgmCommonService.PGMUnit.RefreshmentRepository.GetByID(id);
            var model = refreshment.ToViewModel();

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(Convert.ToInt32(refreshment.EmployeeId));
            model.EmployeeId = emp.Id;
            model.EmpID = emp.EmpID;
            model.EmployeeName = emp.FullName;
            model.DesignationId = emp.DesignationId;
            model.Designation = emp.DesignationName;
            model.AccountNo = emp.BankAccountNo;

            model.strMode = "Edit";
            populateDropdown(model);
            if (type == "success")
            {
                model.IsError = 0;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
            }
            return View(model);
        }

        #endregion
    }
}