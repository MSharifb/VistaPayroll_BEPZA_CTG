using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.TaxWithheld;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class TaxWithheldController : BaseController
    {
        #region Fields
        
        private static string sYear;
        private static string sMonth;
        private static int sEmpID;

        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor

        public TaxWithheldController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            var model = new TaxWithheldViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TaxWithheldViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from tr in _pgmCommonService.PGMUnit.TaxWithheldRepository.GetAll()
                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on tr.EmployeeId equals emp.Id

                        where emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId

                        select new TaxWithheldViewModel()
                        {
                            Id = tr.Id,
                            EmployeeId = tr.EmployeeId,
                            TaxMonth = tr.TaxMonth,
                            TaxYear = tr.TaxYear,
                            Reason = tr.Reason,
                            EmpID = emp.EmpID,
                            EmployeeName = emp.FullName,
                            EmployeeDesignation = emp.DesignationName,
                            Remarks = tr.Remarks
                        }).OrderBy(x => Convert.ToDateTime(x.TaxYear + "-" + x.TaxMonth + "-01")).ToList();

            //var list = _pgmCommonservice.PGMUnit.TaxWithheldRepository.GetAll().ToList();

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
                    list = list.OrderBy(x => x.TaxYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxYear).ToList();
                }
            }

            if (request.SortingName == "TaxMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxMonth).ToList();
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

            if (request.SortingName == "EmployeeDesignation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeDesignation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeDesignation).ToList();
                }
            }

            if (request.SortingName == "Reason")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Reason).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Reason).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.TaxYear,
                    d.TaxMonth,
                    d.EmpID,
                    d.EmployeeName,
                    d.EmployeeDesignation,
                    d.Reason,
                    d.Remarks,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public PartialViewResult AddDetail()
        {
            return PartialView("_Detail");
        }

        public ActionResult Create()
        {
            TaxWithheldViewModel model = new TaxWithheldViewModel();

            PrepareModel(model);

            return View(model);
        }


        [HttpPost]
        public ActionResult Create(TaxWithheldViewModel model)
        {
            List<string> errorList = new List<string>();

            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity = GetInsertUserAuditInfo(entity);

                    errorList = GetBusinessLogicValidation(entity);
                    if (errorList.Count == 0)
                    {
                        _pgmCommonService.PGMUnit.TaxWithheldRepository.Add(entity);
                        _pgmCommonService.PGMUnit.TaxWithheldRepository.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = Common.ErrorListToString(errorList);
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            PrepareModel(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonService.PGMUnit.TaxWithheldRepository.GetByID(id, "Id");

            var entityTaxWithheld = _pgmCommonService.PGMUnit.TaxWithheldRepository.GetByID(id);
            sYear = entityTaxWithheld.TaxYear;
            sMonth = entityTaxWithheld.TaxMonth;
            sEmpID = entityTaxWithheld.EmployeeId;

            var model = entityTaxWithheld.ToModel();
            PrepareModel(model);
            model.Mode = CrudeAction.Edit;

            if (model.EmployeeId != 0)
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

                model.EmpID = emp.EmpID;
                model.EmployeeName = emp.FullName;
                model.EmployeeDesignation = emp.DesignationName;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TaxWithheldViewModel model)
        {
            string errorList = string.Empty;
            var entity = model.ToEntity();
            ArrayList lstDetail = new ArrayList();

            entity.EUser = User.Identity.Name;
            entity.EDate = Common.CurrentDateTime;

            errorList = GetBusinessLogicValidationForEdit(sYear, sMonth, sEmpID);

            try
            {
                if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
                {
                    _pgmCommonService.PGMUnit.TaxWithheldRepository.Update(entity);
                    _pgmCommonService.PGMUnit.TaxWithheldRepository.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = errorList;
                }
            }

            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }
            PrepareModel(model);

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = string.Empty;
            var OAWithheld = _pgmCommonService.PGMUnit.TaxWithheldRepository.GetByID(id);
            errMsg = GetBusinessLogicValidationForEdit(OAWithheld.TaxYear, OAWithheld.TaxMonth, OAWithheld.EmployeeId);

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _pgmCommonService.PGMUnit.TaxWithheldRepository.Delete(id);
                    _pgmCommonService.PGMUnit.TaxWithheldRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                    ModelState.AddModelError("Error", errMsg);
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion Action

        #region Others

        private PGM_TaxWithheld GetInsertUserAuditInfo(PGM_TaxWithheld entity)
        {
            entity.IUser = User.Identity.Name;
            entity.IDate = DateTime.Now;

            return entity;
        }
        private PGM_TaxWithheld GetEditUserAuditInfo(PGM_TaxWithheld entity)
        {
            entity.EUser = User.Identity.Name;
            entity.EDate = DateTime.Now;

            return entity;
        }

        private void PrepareModel(TaxWithheldViewModel model)
        {
            model.TaxYearList = TaxYearList();
            model.TaxMonthList = TaxMonthList();

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeListForDDL(LoggedUserZoneInfoId);
            model.EmployeeList = Common.PopulateEmployeeDDL(emp);
        }

        private List<string> GetBusinessLogicValidation(PGM_TaxWithheld entity)
        {
            List<string> errorMessage = new List<string>();

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(entity.TaxMonth) || entity.TaxMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(entity.TaxYear) || entity.TaxYear == tr.SalaryYear)
                               && (entity.EmployeeId == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;

            if (totalRecords > 0)
            {
                errorMessage.Add("Salary has been processed for the month. Please try another month.");
            }

            return errorMessage;

        }

        private string GetBusinessLogicValidationForEdit(string spYear, string spMonth, int spEmpID)
        {
            string errorMessage = string.Empty;

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(spMonth) || spMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(spYear) || spYear == tr.SalaryYear)
                               && (spEmpID == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;

            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for the month. Update/Delete is not acceptable.";
            }

            return errorMessage;
        }


        private IList<SelectListItem> TaxMonthList()
        {
            var TaxMonth = new List<SelectListItem>();
            TaxMonth.Add(new SelectListItem() { Text = "January", Value = "January" });
            TaxMonth.Add(new SelectListItem() { Text = "February", Value = "February" });
            TaxMonth.Add(new SelectListItem() { Text = "March", Value = "March" });
            TaxMonth.Add(new SelectListItem() { Text = "April", Value = "April" });
            TaxMonth.Add(new SelectListItem() { Text = "May", Value = "May" });
            TaxMonth.Add(new SelectListItem() { Text = "June", Value = "June" });

            TaxMonth.Add(new SelectListItem() { Text = "July", Value = "July" });
            TaxMonth.Add(new SelectListItem() { Text = "August", Value = "August" });
            TaxMonth.Add(new SelectListItem() { Text = "September", Value = "September" });
            TaxMonth.Add(new SelectListItem() { Text = "October", Value = "October" });
            TaxMonth.Add(new SelectListItem() { Text = "November", Value = "November" });
            TaxMonth.Add(new SelectListItem() { Text = "December", Value = "December" });

            return TaxMonth;
        }

        public ActionResult GetTaxMonthList()
        {
            var TaxMonth = new Dictionary<string, string>();
            TaxMonth.Add("January", "January");
            TaxMonth.Add("February", "February");
            TaxMonth.Add("March", "March");
            TaxMonth.Add("April", "April");
            TaxMonth.Add("May", "May");
            TaxMonth.Add("June", "June");

            TaxMonth.Add("July", "July");
            TaxMonth.Add("August", "August");
            TaxMonth.Add("September", "September");
            TaxMonth.Add("October", "October");
            TaxMonth.Add("November", "November");
            TaxMonth.Add("December", "December");

            ViewBag.TaxMonthList = TaxMonth;
            return PartialView("Select", TaxMonth);
        }

        private IList<SelectListItem> TaxYearList()
        {
            var TaxYear = new List<SelectListItem>();
            TaxYear = Common.PopulateYearList().DistinctBy(x => x.Value).ToList();
            return TaxYear;
        }
        public ActionResult GetTaxYearList()
        {
            var TaxYear = new Dictionary<string, string>();
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                var iyFormat = i.ToString();
                TaxYear.Add(iyFormat, iyFormat);
            }

            ViewBag.IncomeYearList = TaxYear;
            return PartialView("Select", TaxYear);
        }

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

            return PartialView("_SelectDesignation", Designation);
        }

        #endregion Others
    }
}
