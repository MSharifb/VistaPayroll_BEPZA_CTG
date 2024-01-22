using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.HouseRentWaterBillDeduct;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class HouseRentWaterBillDeductController : BaseController
    {
        #region Fields
        private readonly string SalaryHeadType;
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor

        public HouseRentWaterBillDeductController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
            SalaryHeadType = PGMEnum.SalaryHeadType.Deduction.ToString();
        }

        #endregion

        #region Actions

        [NoCache]
        public ViewResult Index()
        {
            var model = new HouseRentWaterBillDeductModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, HouseRentWaterBillDeductModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<HouseRentWaterBillDeductModel> list = (from tr in _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.GetAll()
                                                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on tr.EmployeeId equals emp.Id

                                                        where (model.EmployeeName == null || model.EmployeeName == "" || emp.FullName.Contains(model.EmployeeName))
                                                        && emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId

                                                        select new HouseRentWaterBillDeductModel()
                                                        {
                                                            Id = tr.Id,
                                                            EmpID = emp.EmpID,
                                                            EmployeeName = emp.FullName,
                                                            EmployeeDesignation = emp.DesignationName,
                                                            DesignationId = emp.DesignationId,
                                                            EffectiveDateFrom = tr.EffectiveDateFrom,
                                                            EffectiveDateTo = tr.EffectiveDateTo,
                                                            HouseRentDeductionType = tr.HouseRentDeductionType,
                                                            HouseRentAmount = Convert.ToDecimal(tr.HouseRentAmount)
                                                        }).OrderBy(x => x.EffectiveDateFrom).ToList();

            #region Filter

            if (!String.IsNullOrEmpty(model.EmpID))
            {
                list = list.Where(t => t.EmployeeName.Contains(model.EmpID)).ToList();
            }

            // Date Range
            if (model.EffectiveDateFrom != null && model.EffectiveDateTo != null)
            {
                list = list.Where(t => t.EffectiveDateFrom >= model.EffectiveDateFrom && t.EffectiveDateTo <= model.EffectiveDateTo).ToList();
            }

            // Employee Name
            if (model.DesignationId != 0)
            {
                list = list.Where(t => t.DesignationId == model.DesignationId).ToList();
            }

            #endregion

            #region Paging

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            #endregion

            #region Sorting

            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.Id).ToList();
                else
                    list = list.OrderByDescending(x => x.Id).ToList();
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


            if (request.SortingName == "HouseRentAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.HouseRentAmount).ToList();
                else
                    list = list.OrderByDescending(x => x.HouseRentAmount).ToList();
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmpID,
                    d.EmployeeName,
                    Convert.ToDateTime(d.EffectiveDateFrom).ToString(DateAndTime.GlobalDateFormat),
                    Convert.ToDateTime(d.EffectiveDateTo).ToString(DateAndTime.GlobalDateFormat),
                    d.EmployeeDesignation,
                    d.HouseRentDeductionType,
                    d.HouseRentAmount,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public PartialViewResult AddDetail()
        {
            return PartialView("_Detail");
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new HouseRentWaterBillDeductModel();
            model.Mode = "Add";
            PrepareModel(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(HouseRentWaterBillDeductModel model)
        {
            model.Mode = "Add";
            string errorList = string.Empty;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    ApplyBusenessLogic(model);

                    var entity = model.ToEntity();
                    entity = SetInsertUserAuditInfo(entity);

                    _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.Add(entity);
                    _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.SaveChanges();

                    return RedirectToAction("Index");
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

            PrepareModel(model);

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(long id)
        {
            var entity = _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.GetByID64Bit(id);

            var model = entity.ToModel();
            model.Mode = CrudeAction.Edit;
            PrepareModel(model);

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
        [NoCache]
        public ActionResult Edit(HouseRentWaterBillDeductModel model)
        {
            model.Mode = CrudeAction.Edit;
            string errorList = string.Empty;

            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    ApplyBusenessLogic(model);

                    var entity = model.ToEntity();
                    entity = SetEditUserAuditInfo(entity);

                    _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.Update_64Bit(entity);
                    _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.SaveChanges();

                    return RedirectToAction("Index");
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

            PrepareModel(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(long id)
        {
            bool result;
            var entity = _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.GetByID64Bit(id);
            string errMsg = GetBusinessLogicValidation(entity.ToModel());

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.Delete_64Bit(id);
                    _pgmCommonService.PGMUnit.HouseRentWaterBillDeductRepositoty.SaveChanges();
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

        private void ApplyBusenessLogic(HouseRentWaterBillDeductModel model)
        {
            if (model.HouseRentDeductionType.Equals(PGMEnum.HouseRentDeductionType.HRA.ToString()))
            {
                model.HouseRentAmount = null;
            }
        }

        [NoCache]
        private PGM_HouseRentWaterBillDeduct SetInsertUserAuditInfo(PGM_HouseRentWaterBillDeduct entity)
        {
            entity.IUser = User.Identity.Name;
            entity.IDate = DateTime.Now;

            return entity;
        }

        [NoCache]
        private PGM_HouseRentWaterBillDeduct SetEditUserAuditInfo(PGM_HouseRentWaterBillDeduct entity)
        {
            entity.EUser = User.Identity.Name;
            entity.EDate = DateTime.Now;

            return entity;
        }

        [NoCache]
        private void PrepareModel(HouseRentWaterBillDeductModel model)
        {
            model.SalaryHeadList = SalaryHeadList(model);
        }

        [NoCache]
        public string GetBusinessLogicValidation(HouseRentWaterBillDeductModel model)
        {
            string errorMessage = string.Empty;

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where
                                   model.EffectiveDateFrom <= new DateTime(int.Parse(tr.SalaryYear), UtilCommon.GetMonthNo(tr.SalaryMonth), UtilCommon.GetMonthLastDay(tr.SalaryYear, tr.SalaryMonth))
                                   && model.EffectiveDateTo <= new DateTime(int.Parse(tr.SalaryYear), UtilCommon.GetMonthNo(tr.SalaryMonth), UtilCommon.GetMonthLastDay(tr.SalaryYear, tr.SalaryMonth))
                                   && (model.EmployeeId == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;
            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for effective date range. Create/Edit is not acceptable.";
                return errorMessage;
            }

            if (Convert.ToDateTime(model.EffectiveDateFrom) > Convert.ToDateTime(model.EffectiveDateTo))
            {
                errorMessage = "From date must be less then to date.";
                return errorMessage;
            }

            if (model.WaterBillAmount <= 0)
            {
                errorMessage = "Water Bill amount must be greater than zero.";
                return errorMessage;
            }

            if (model.HouseRentDeductionType.Equals(PGMEnum.HouseRentDeductionType.Fixed.ToString()) && (model.HouseRentAmount == null || model.HouseRentAmount <= 0))
            {
                errorMessage = "House rent amount must be greater than zero.";
                return errorMessage;
            }

            return errorMessage;
        }

        //private string GetBusinessLogicValidationForEdit(HouseRentWaterBillDeductModel model)
        //{
        //    string errorMessage = string.Empty;

        //    var salaryexist = (from tr in _pgmCommonservice.PGMUnit.SalaryMaster.GetAll()
        //                       where (string.IsNullOrEmpty(model.SalaryMonth) || model.SalaryMonth == tr.SalaryMonth)
        //                       && (string.IsNullOrEmpty(model.SalaryYear) || model.SalaryYear == tr.SalaryYear)
        //                       && (model.EmployeeId == tr.EmployeeId)
        //                       select tr.EmployeeId).ToList();

        //    int totalRecords = salaryexist == null ? 0 : salaryexist.Count;

        //    if (totalRecords > 0)
        //    {
        //        errorMessage = "Salary has been processed for the month. Update/Delete is not acceptable.";
        //    }

        //    return errorMessage;
        //}

        #region Index Page Search

        //[NoCache]
        //public ActionResult GetSalaryMonthList()
        //{
        //    var salaryMonth = new Dictionary<string, string>();

        //    foreach (var item in Common.PopulateMonthList())
        //    {
        //        salaryMonth.Add(item.Text, item.Value);
        //    }

        //    ViewBag.SalaryMonthList = salaryMonth;
        //    return PartialView("Select", salaryMonth);
        //}

        //[NoCache]
        //public ActionResult GetSalaryYearList()
        //{
        //    var SalaryYear = new Dictionary<string, string>();

        //    foreach (var item in Common.PopulateYearList())
        //    {
        //        SalaryYear.Add(item.Text, item.Value);
        //    }

        //    ViewBag.IncomeYearList = SalaryYear;
        //    return PartialView("Select", SalaryYear);
        //}

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

        [NoCache]
        public ActionResult GetSalaryHeadList()
        {
            IList<SelectListItem> SalaryHead = new List<SelectListItem>();

            var list = _pgmCommonService.PGMUnit.SalaryHeadRepository.Get(x => x.IsGrossPayHead == false).OrderBy(x => x.HeadName).ToList();

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
        private IList<SelectListItem> SalaryHeadList(HouseRentWaterBillDeductModel model)
        {
            IList<SelectListItem> SalaryHeadList = new List<SelectListItem>();

            var list = _pgmCommonService.PGMUnit.SalaryHeadRepository.Get(x => x.IsGrossPayHead == false && x.HeadType == SalaryHeadType).OrderBy(x => x.HeadName).ToList();

            SalaryHeadList = Common.PopulateSalaryHeadDDL(list);

            return SalaryHeadList;
        }

        public ActionResult GetSalaryHeadByHeadType(string pHeadType)
        {
            var list = _pgmCommonService.PGMUnit.SalaryHeadRepository.Get(t => t.HeadType == pHeadType && t.IsGrossPayHead == false).OrderBy(t => t.HeadName);

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
