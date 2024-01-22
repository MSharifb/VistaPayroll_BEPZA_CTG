using DAL.PGM;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.OtherAdjustment;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class OtherAdjustmentController : BaseController
    {
        #region Fields
        private readonly PgmOtherAdjustmentService _pgmOtherAdjustmentService;
        #endregion

        #region Constructor

        public OtherAdjustmentController(PgmOtherAdjustmentService pgmOtherAdjustmentService)
        {
            this._pgmOtherAdjustmentService = pgmOtherAdjustmentService;
        }

        #endregion

        #region Actions

        [NoCache]
        public ViewResult Index()
        {
            var model = new OtherAdjustmentModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, OtherAdjustmentModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmOtherAdjustmentService.OtherAdjustmentListMaster(
                model.SalaryMonth,
                model.SalaryYear,
                LoggedUserZoneInfoId);

            if (!string.IsNullOrEmpty(model.HeadType) && !model.HeadType.Equals("All"))
            {
                list = list.Where(t => t.HeadType == model.HeadType).ToList();
            }

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
                    list = list.OrderBy(x => x.Id).ToList();
                else
                    list = list.OrderByDescending(x => x.Id).ToList();
            }

            if (request.SortingName == "SalaryYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryYear).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryYear).ToList();
            }

            if (request.SortingName == "SalaryMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryMonth).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryMonth).ToList();
            }

            if (request.SortingName == "EmpID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmpID).ToList();
                else
                    list = list.OrderByDescending(x => x.EmpID).ToList();
            }

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                else
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
            }

            if (request.SortingName == "EmployeeDesignation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmployeeDesignation).ToList();
                else
                    list = list.OrderByDescending(x => x.EmployeeDesignation).ToList();
            }

            if (request.SortingName == "SalaryHead")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryHead).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryHead).ToList();
            }

            if (request.SortingName == "Amount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.Amount).ToList();
                else
                    list = list.OrderByDescending(x => x.Amount).ToList();
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(d.SalaryYear + "-" + d.SalaryMonth, new List<object>()
                {
                    d.SalaryYear+"-"+ d.SalaryMonth,
                    d.SalaryYear,
                    d.SalaryMonth,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailList(JqGridRequest request, string year, string month, OtherAdjustmentModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmOtherAdjustmentService.OtherAdjustmentSearchList(
                month,
                year,
                model.EmpID,
                model.EmployeeName,
                model.EmployeeDesignation,
                LoggedUserZoneInfoId);

            if (!string.IsNullOrEmpty(model.HeadType) && !model.HeadType.Equals("All"))
            {
                list = list.Where(t => t.HeadType == model.HeadType).ToList();
            }

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
                    list = list.OrderBy(x => x.Id).ToList();
                else
                    list = list.OrderByDescending(x => x.Id).ToList();
            }

            if (request.SortingName == "SalaryYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryYear).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryYear).ToList();
            }

            if (request.SortingName == "SalaryMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryMonth).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryMonth).ToList();
            }

            if (request.SortingName == "EmpID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmpID).ToList();
                else
                    list = list.OrderByDescending(x => x.EmpID).ToList();
            }

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                else
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
            }

            if (request.SortingName == "EmployeeDesignation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmployeeDesignation).ToList();
                else
                    list = list.OrderByDescending(x => x.EmployeeDesignation).ToList();
            }

            if (request.SortingName == "SalaryHead")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryHead).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryHead).ToList();
            }

            if (request.SortingName == "Type")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.HeadType).ToList();
                else
                    list = list.OrderByDescending(x => x.HeadType).ToList();
            }

            if (request.SortingName == "Amount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.Amount).ToList();
                else
                    list = list.OrderByDescending(x => x.Amount).ToList();
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmpID,
                    d.EmployeeName,
                    d.EmployeeDesignation,
                    d.SalaryYear,
                    d.SalaryMonth,
                    d.HeadType,
                    d.SalaryHead,
                    d.Amount,

                    d.IsOverrideStructureAmount,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        [HttpPost]
        public PartialViewResult AddEmployee(OtherAdjustDeductEmployeesModel model)
        {
            if (model.Id != 0)
            {
                var emp = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeById(model.Id);

                model.EmpID = emp.EmpID;
                model.EmployeeName = emp.FullName;
                model.EmployeeDesignation = emp.DesignationName;
            }

            model.AdjustmentTypeList = AdjustmentTypeList();
            model.SalaryHeadList = SalaryHeadList(model.Type);

            return PartialView("_PartialDetail", model);
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new OtherAdjustmentModel();
            model.strMode = "Add";
            model.IsOverrideStructureAmount = true;
            PopulateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(OtherAdjustmentModel model)
        {
            model.strMode = "Add";
            string errorList = string.Empty;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    IList<PGM_OtherAdjustDeduct> entityList = CreateEntityList(model);

                    foreach (var item in entityList)
                    {
                        _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.Add(item);
                    }

                    _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.SaveChanges();
                    return RedirectToAction("GoToDetails", new { idYearMonth = model.SalaryYear + "-" + model.SalaryMonth });
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrMsg = errorList;
            }

            PopulateDropdown(model);

            foreach (var item in model.OtherEmployeeModel)
            {
                item.AdjustmentTypeList = AdjustmentTypeList();
                item.SalaryHeadList = SalaryHeadList(item.Type);
            }
            
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            PGM_OtherAdjustDeduct entity = _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.GetByID(id);

            var model = entity.ToModel();
            model.strMode = "Edit";
            PopulateDropdown(model);

            if (model.EmployeeId != 0)
            {
                var emp = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

                model.EmpID = emp.EmpID;
                model.EmployeeName = emp.FullName;
                model.EmployeeDesignation = emp.DesignationName;
            }

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(OtherAdjustmentModel model)
        {
            model.strMode = "Edit";
            string errorList = string.Empty;
            var entity = model.ToEntity();

            entity.EUser = User.Identity.Name;
            entity.EDate = Common.CurrentDateTime;

            ModelState.Remove("FromYear");
            ModelState.Remove("ToYear");
            ModelState.Remove("FromMonth");
            ModelState.Remove("ToMonth");
            ModelState.Remove("EmpID");
            ModelState.Remove("EmployeeName");
            ModelState.Remove("EmployeeDesignation");

            errorList = GetBusinessLogicValidationForEdit(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.Update(entity);
                    _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                    return View(model);
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrMsg = errorList;
            }
            
            PopulateDropdown(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = string.Empty;
            var entity = _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.GetByID(id);
            errMsg = GetBusinessLogicValidationForDelete(entity.ToModel());

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.Delete(id);
                    _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.SaveChanges();
                    result = true;
                }
                catch (UpdateException ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                    ModelState.AddModelError("Error", errMsg);
                    result = false;
                }
                catch
                {
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

        [NoCache]
        public ActionResult GoToDetails(string idYearMonth)
        {
            var model = new OtherAdjustmentModel();

            string[] yearMonth = idYearMonth.Split('-');
            model.SalaryYear = yearMonth[0];
            model.SalaryMonth = yearMonth[1];

            return View("DetailsList", model);
        }

        #endregion Action

        #region Others

        [NoCache]
        private IList<PGM_OtherAdjustDeduct> CreateEntityList(OtherAdjustmentModel model)
        {
            IList<PGM_OtherAdjustDeduct> list = new List<PGM_OtherAdjustDeduct>();

            var listYearMonth = GetListOfYearMonth(model);

            foreach (var emp in model.OtherEmployeeModel)
            {
                foreach (var item in listYearMonth)
                {
                    model.SalaryYear = item.Year;
                    model.SalaryMonth = item.Month;

                    model.EmployeeId = emp.Id;
                    model.HeadType = emp.Type;
                    model.SalaryHeadId = emp.SalaryHeadId;
                    model.Amount = emp.Amount;
                    model.AmountType = PGMEnum.AmountType.Fixed.ToString();
                    model.IsOverrideStructureAmount = emp.IsOverrideStructureAmount;

                    var entity = model.ToEntity();
                    list.Add(entity);
                }
            }

            return list;
        }

        [NoCache]
        private IList<YearMonth> GetListOfYearMonth(OtherAdjustmentModel model)
        {
            IList<YearMonth> listYearMonth = new List<YearMonth>();

            int intFromYear = Convert.ToInt32(model.FromYear);
            int intFromMonth = Convert.ToInt32(model.FromMonth);

            while (Convert.ToDateTime(intFromYear + "/" + intFromMonth + "/01") <= model.ToDate)
            {
                listYearMonth.Add(new YearMonth { Year = intFromYear.ToString(), Month = UtilCommon.GetMonthName(intFromMonth) });

                if (intFromMonth == 12)
                {
                    intFromMonth = 1;
                    intFromYear += 1;
                }
                else
                {
                    intFromMonth += 1;
                }
            }

            return listYearMonth;
        }

        [NoCache]
        private void PopulateDropdown(OtherAdjustmentModel model)
        {
            model.SalaryYearList = SalaryYearList();

            if (model.strMode == "Add")
            {
                model.SalaryMonthList = Common.PopulateMonthList2();
            }
            else
            {
                model.SalaryMonthList = Common.PopulateMonthList();
            }

            model.AdjustmentTypeList = AdjustmentTypeList();
            model.SalaryHeadList = SalaryHeadList(model.HeadType);

            //----------
            var emps = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeList().Select(q => new
            {
                ZoneInfoId = q.SalaryWithdrawFromZoneId,
                EmpID = q.EmpID,
                Id = q.Id,
                DateOfInactive = q.DateofInactive,
                DisplayText = q.FullName + " [" + q.EmpID + " ]"
            })
                .Where(x => x.ZoneInfoId == LoggedUserZoneInfoId && x.DateOfInactive == null).ToList();

            model.EmployeeList = emps.Select(e => new SelectListItem()
            {
                Text = e.DisplayText,
                Value = e.Id.ToString()
            }).ToList();
            //------------
        }

        [NoCache]
        public string GetBusinessLogicValidation(OtherAdjustmentModel model)
        {
            List<String> errorMessage = new List<string>();

            if (model.OtherEmployeeModel == null || !model.OtherEmployeeModel.Any())
            {
                return "Please add atleast an employee.";
            }

            if (Convert.ToDateTime(model.FromDate) > Convert.ToDateTime(model.ToDate))
            {
                return "From month must be less then to month.";
            }

            bool salaryFound = false;
            var listYearMonth = GetListOfYearMonth(model);
            foreach (var item in listYearMonth)
            {
                //var salaryexist = (from tr in _pgmCommonservice.PGMUnit.SalaryMasterRepository.GetAll()
                //                   where
                //                       model.FromDate > new DateTime(int.Parse(tr.SalaryYear), UtilCommon.GetMonthNo(tr.SalaryMonth), 1)
                //                       && model.ToDate < new DateTime(int.Parse(tr.SalaryYear), UtilCommon.GetMonthNo(tr.SalaryMonth), 1)

                //                   select tr.EmployeeId).ToList();
                //int totalRecords = salaryexist == null ? 0 : salaryexist.Count;

                var zoneWiseEmpList = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository
                    .GetEmployeeList(LoggedUserZoneInfoId, Convert.ToDateTime(item.Year + "-" + item.Month + "-01"));

                var salaryexist1 = (from tr in _pgmOtherAdjustmentService.PGMUnit.SalaryMasterRepository.GetAll()
                                    join empHis in zoneWiseEmpList on tr.EmployeeId equals empHis.Id
                                    where tr.SalaryYear == item.Year
                                    && tr.SalaryMonth == item.Month
                                    select tr.EmployeeId)
                                   .ToList();

                if (salaryexist1 != null && salaryexist1.Any())
                {
                    salaryFound = true;
                }

                bool duplicateEntryFound = false;
                StringBuilder duplicateEmpIds = new StringBuilder();
                foreach (var employeesModel in model.OtherEmployeeModel)
                {
                    var duplicate = _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.GetAll()
                        .Where(o => o.SalaryYear == item.Year
                                    && o.SalaryMonth == item.Month
                                    && o.HeadType == model.HeadType
                                    && o.SalaryHeadId == model.SalaryHeadId
                                    && o.EmployeeId == employeesModel.Id)
                        .ToList();

                    if (duplicate.Any())
                    {
                        duplicateEntryFound = true;
                        var emp = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeById(employeesModel.Id);
                        duplicateEmpIds.Append(emp.EmpID + "(" + item.Year + "/" + item.Month + "), ");
                    }
                }
                if (duplicateEntryFound)
                {
                    errorMessage.Add("Employee Id(s) - " + duplicateEmpIds.ToString() + " found duplicate!");
                }
            }

            if (salaryFound)
            {
                errorMessage.Add("Salary has been processed for the month. Adjustment is not acceptable.");
            }

            return Common.ErrorListToString(errorMessage);
        }

        [NoCache]
        private string GetBusinessLogicValidationForEdit(OtherAdjustmentModel model)
        {
            StringBuilder errorMessage = new StringBuilder();
            bool salaryFound = false;

            var zoneWiseEmpList = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository
                .GetEmployeeList(LoggedUserZoneInfoId, Convert.ToDateTime(model.SalaryYear + "-" + model.SalaryMonth + "-01"));

            var salaryexist1 = (from tr in _pgmOtherAdjustmentService.PGMUnit.SalaryMasterRepository.GetAll()
                                join empHis in zoneWiseEmpList on tr.EmployeeId equals empHis.Id
                                where tr.SalaryYear == model.SalaryYear
                                && tr.SalaryMonth == model.SalaryMonth
                                select tr.EmployeeId)
                               .ToList();

            if (salaryexist1 != null && salaryexist1.Any())
            {
                salaryFound = true;
            }

            var duplicate = _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.GetAll()
                .Where(o => o.SalaryYear == model.SalaryYear
                           && o.SalaryMonth == model.SalaryMonth
                           && o.HeadType == model.HeadType
                           && o.SalaryHeadId == model.SalaryHeadId
                           && o.EmployeeId == model.EmployeeId
                           && o.Id != model.Id)
                           .ToList();

            if (duplicate.Any())
            {
                errorMessage.AppendLine().Append("Duplicate Entry.");
            }

            if (salaryFound)
            {
                errorMessage.AppendLine().Append("Salary has been processed for this selected month. Edit is not acceptable.");
            }

            return errorMessage.ToString();
        }

        [NoCache]
        private string GetBusinessLogicValidationForDelete(OtherAdjustmentModel model)
        {
            StringBuilder errorMessage = new StringBuilder();
            bool salaryFound = false;

            var zoneWiseEmpList = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository
                .GetEmployeeList(LoggedUserZoneInfoId, Convert.ToDateTime(model.SalaryYear + "-" + model.SalaryMonth + "-01"));


            var salaryexist1 = (from tr in _pgmOtherAdjustmentService.PGMUnit.SalaryMasterRepository.GetAll()
                                join empHis in zoneWiseEmpList on tr.EmployeeId equals empHis.Id
                                where tr.SalaryYear == model.SalaryYear
                                && tr.SalaryMonth == model.SalaryMonth
                                select tr.EmployeeId)
                                   .ToList();

            if (salaryexist1 != null && salaryexist1.Any())
            {
                salaryFound = true;
            }

            if (salaryFound)
            {
                errorMessage.AppendLine().Append("Salary has been processed for the month. Cannot Delete this entry.");
            }

            return errorMessage.ToString();
        }

        #region Index Page Search

        [NoCache]
        public ActionResult GetSalaryMonthList()
        {
            var salaryMonth = new Dictionary<string, string>();

            foreach (var item in Common.PopulateMonthList())
            {
                salaryMonth.Add(item.Text, item.Value);
            }

            ViewBag.SalaryMonthList = salaryMonth;
            return PartialView("Select", salaryMonth);
        }

        [NoCache]
        public ActionResult GetSalaryYearList()
        {
            var SalaryYear = new Dictionary<string, string>();

            foreach (var item in Common.PopulateYearList())
            {
                SalaryYear.Add(item.Text, item.Value);
            }

            ViewBag.IncomeYearList = SalaryYear;
            return PartialView("Select", SalaryYear);
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {
            var Designation = new List<SelectListItem>();

            Designation = _pgmOtherAdjustmentService.PGMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name)
            .ToList()
            .Select(y => new SelectListItem()
            {
                Text = y.Name,
                Value = y.Id.ToString()
            }).ToList();

            return PartialView("_Select", Designation);
        }

        [NoCache]
        public ActionResult GetSalaryHeadList()
        {
            IList<SelectListItem> SalaryHead = new List<SelectListItem>();

            var list = _pgmOtherAdjustmentService.PGMUnit.SalaryHeadRepository.Get(x => x.IsGrossPayHead == false).OrderBy(x => x.HeadName).ToList();

            SalaryHead = Common.PopulateSalaryHeadDDL(list);

            return PartialView("_Select", SalaryHead);
        }
        #endregion

        [NoCache]
        private IList<SelectListItem> SalaryYearList()
        {
            var SalaryYear = new List<SelectListItem>();
            return Common.PopulateYearList().DistinctBy(x => x.Value).ToList();
        }

        [NoCache]
        private IList<SelectListItem> AdjustmentTypeList()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "Addition", Value = "Addition" });
            list.Add(new SelectListItem { Text = "Deduction", Value = "Deduction" });
            return list;
        }

        [NoCache]
        private IList<SelectListItem> SalaryHeadList(String headType)
        {
            IList<SelectListItem> SalaryHeadList = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(headType))
            {
                var list = _pgmOtherAdjustmentService.PGMUnit.SalaryHeadRepository.Get(x => x.HeadType == headType).OrderBy(x => x.HeadName).ToList();

                SalaryHeadList = Common.PopulateSalaryHeadDDL(list);
            }

            return SalaryHeadList;
        }

        [NoCache]
        public ActionResult GetSalaryHeadByHeadType(string pHeadType)
        {
            var list = _pgmOtherAdjustmentService.PGMUnit.SalaryHeadRepository.Get(t => t.HeadType == pHeadType).OrderBy(t => t.HeadName);

            return Json(
                new
                {
                    SalaryHeadList = list.Select(x => new { Id = x.Id, HeadName = x.HeadName })
                },
                JsonRequestBehavior.AllowGet
            );
        }

        #endregion Others
    }
}
