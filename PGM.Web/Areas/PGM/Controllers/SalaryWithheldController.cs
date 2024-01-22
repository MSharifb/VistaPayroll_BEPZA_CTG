using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.SalaryWithheld;
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
    public class SalaryWithheldController : BaseController
    {
        #region Fields

        
        private static string sYear;
        private static string sMonth;
        private static int sEmpID;

        private readonly PGMCommonService _pgmCommonService;

        #endregion

        #region Constructor

        public SalaryWithheldController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            var model = new SalaryWithheldModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SalaryWithheldModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from tr in _pgmCommonService.PGMUnit.SalaryWithheldRepository.GetAll()
                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on tr.EmployeeId equals emp.Id

                        where Common.GetInteger(emp.SalaryWithdrawFromZoneId) == LoggedUserZoneInfoId

                        select new SalaryWithheldModel()
                        {
                            Id = tr.Id,
                            EmployeeId = tr.EmployeeId,
                            SalaryMonth = tr.SalaryMonth,
                            SalaryYear = tr.SalaryYear,
                            Reason = tr.Reason,
                            EmpID = emp.EmpID,
                            EmployeeName = emp.FullName,
                            EmployeeDesignation = emp.DesignationName,
                            Remarks = tr.Remarks
                        }).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();

            //var list = _pgmCommonservice.PGMUnit.SalaryWithheldRepository.GetAll().ToList();

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
                    d.SalaryYear,
                    d.SalaryMonth,
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

        public ActionResult Create()
        {
            SalaryWithheldModel model = new SalaryWithheldModel();

            PrepareModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SalaryWithheldModel model)
        {
            List<string> errorList = new List<string>();
            model.IsError = 1;
            model.errClass = "failed";

            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();

                    entity = GetInsertUserAuditInfo(entity);

                    errorList = GetBusinessLogicValidation(entity);
                    if (errorList.Count == 0)
                    {
                        _pgmCommonService.PGMUnit.SalaryWithheldRepository.Add(entity);
                        _pgmCommonService.PGMUnit.SalaryWithheldRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        return RedirectToAction("Index", model);
                    }
                }
                if (errorList.Count() > 0)
                {
                    model.ErrMsg = Common.ErrorListToString(errorList);
                }
            }
            catch (Exception ex)
            {
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            PrepareModel(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonService.PGMUnit.SalaryWithheldRepository.GetByID(id, "Id");

            PGM_SalaryWithheld entitySalaryWithheld = _pgmCommonService.PGMUnit.SalaryWithheldRepository.GetByID(id);
            sYear = entitySalaryWithheld.SalaryYear;
            sMonth = entitySalaryWithheld.SalaryMonth;
            sEmpID = entitySalaryWithheld.EmployeeId;

            SalaryWithheldModel model = entitySalaryWithheld.ToModel();
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
        public ActionResult Edit(SalaryWithheldModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.errClass = "failed";

            PGM_SalaryWithheld entity = model.ToEntity();
            ArrayList lstDetail = new ArrayList();

            entity.EUser = User.Identity.Name;
            entity.EDate = Common.CurrentDateTime;

            errorList = GetBusinessLogicValidationForEdit(sYear, sMonth, sEmpID);

            try
            {
                if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
                {
                    _pgmCommonService.PGMUnit.SalaryWithheldRepository.Update(entity);
                    _pgmCommonService.PGMUnit.SalaryWithheldRepository.SaveChanges();

                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                    return RedirectToAction("Index");
                }
                else
                {
                    model.ErrMsg = errorList;
                }
            }
            catch (Exception ex)
            {
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
            var OAWithheld = _pgmCommonService.PGMUnit.SalaryWithheldRepository.GetByID(id);
            errMsg = GetBusinessLogicValidationForEdit(OAWithheld.SalaryYear, OAWithheld.SalaryMonth, OAWithheld.EmployeeId);

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _pgmCommonService.PGMUnit.SalaryWithheldRepository.Delete(id);
                    _pgmCommonService.PGMUnit.SalaryWithheldRepository.SaveChanges();
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

        private PGM_SalaryWithheld GetInsertUserAuditInfo(PGM_SalaryWithheld entity)
        {
            entity.IUser = User.Identity.Name;
            entity.IDate = DateTime.Now;

            return entity;
        }

        private PGM_SalaryWithheld GetEditUserAuditInfo(PGM_SalaryWithheld entity)
        {
            entity.EUser = User.Identity.Name;
            entity.EDate = DateTime.Now;

            return entity;
        }

        private void PrepareModel(SalaryWithheldModel model)
        {
            model.SalaryYearList = SalaryYearList();
            model.SalaryMonthList = SalaryMonthList();

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeListForDDL(LoggedUserZoneInfoId);
            model.EmployeeList = Common.PopulateEmployeeDDL(emp);
        }

        private List<string> GetBusinessLogicValidation(PGM_SalaryWithheld entity)
        {
            List<string> errorMessage = new List<string>();

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(entity.SalaryMonth) || entity.SalaryMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(entity.SalaryYear) || entity.SalaryYear == tr.SalaryYear)
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

        private IList<SelectListItem> SalaryMonthList()
        {
            IList<SelectListItem> SalaryMonth = new List<SelectListItem>();
            SalaryMonth.Add(new SelectListItem() { Text = "January", Value = "January" });
            SalaryMonth.Add(new SelectListItem() { Text = "February", Value = "February" });
            SalaryMonth.Add(new SelectListItem() { Text = "March", Value = "March" });
            SalaryMonth.Add(new SelectListItem() { Text = "April", Value = "April" });
            SalaryMonth.Add(new SelectListItem() { Text = "May", Value = "May" });
            SalaryMonth.Add(new SelectListItem() { Text = "June", Value = "June" });

            SalaryMonth.Add(new SelectListItem() { Text = "July", Value = "July" });
            SalaryMonth.Add(new SelectListItem() { Text = "August", Value = "August" });
            SalaryMonth.Add(new SelectListItem() { Text = "September", Value = "September" });
            SalaryMonth.Add(new SelectListItem() { Text = "October", Value = "October" });
            SalaryMonth.Add(new SelectListItem() { Text = "November", Value = "November" });
            SalaryMonth.Add(new SelectListItem() { Text = "December", Value = "December" });

            return SalaryMonth;
        }

        public ActionResult GetSalaryMonthList()
        {
            Dictionary<string, string> SalaryMonth = new Dictionary<string, string>();
            SalaryMonth.Add("January", "January");
            SalaryMonth.Add("February", "February");
            SalaryMonth.Add("March", "March");
            SalaryMonth.Add("April", "April");
            SalaryMonth.Add("May", "May");
            SalaryMonth.Add("June", "June");

            SalaryMonth.Add("July", "July");
            SalaryMonth.Add("August", "August");
            SalaryMonth.Add("September", "September");
            SalaryMonth.Add("October", "October");
            SalaryMonth.Add("November", "November");
            SalaryMonth.Add("December", "December");

            ViewBag.SalaryMonthList = SalaryMonth;
            return PartialView("Select", SalaryMonth);
        }

        private IList<SelectListItem> SalaryYearList()
        {
            IList<SelectListItem> SalaryYear = new List<SelectListItem>();
            SalaryYear = Common.PopulateYearList().DistinctBy(x => x.Value).ToList();
            return SalaryYear;
        }

        public ActionResult GetSalaryYearList()
        {
            Dictionary<string, string> SalaryYear = new Dictionary<string, string>();
            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                var iyFormat = i.ToString();
                SalaryYear.Add(iyFormat, iyFormat);
            }


            ViewBag.IncomeYearList = SalaryYear;
            return PartialView("Select", SalaryYear);
        }

        public ActionResult GetDesignationList()
        {
            IList<SelectListItem> Designation = new List<SelectListItem>();

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
