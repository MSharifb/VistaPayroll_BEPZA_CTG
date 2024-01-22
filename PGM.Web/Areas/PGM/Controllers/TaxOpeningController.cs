using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.TaxOpening;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class TaxOpeningController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor

        public TaxOpeningController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            var model = new TaxOpeningModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TaxOpeningModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<TaxOpeningModel> list = (from tr in _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.GetAll()
                                          join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on tr.EmployeeId equals emp.Id

                                          where (string.IsNullOrEmpty(model.IncomeYear) || model.IncomeYear == tr.IncomeYear)
                                                && emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                                          && (string.IsNullOrEmpty(model.EmpID) || model.EmpID == emp.EmpID)
                                          && (string.IsNullOrEmpty(model.EmployeeName) || model.EmployeeName == emp.FullName)
                                          && (string.IsNullOrEmpty(model.EmployeeDesignation) || model.EmployeeDesignation == emp.DesignationId.ToString())

                                          select new TaxOpeningModel()
                                          {
                                              Id = tr.Id,
                                              EmployeeId = tr.EmployeeId,
                                              IncomeYear = tr.IncomeYear,
                                              EmpID = emp.EmpID,
                                              EmployeeName = emp.FullName,
                                              EmployeeDesignation = emp.DesignationName,
                                              TaxDeducted = tr.TaxDeducted
                                          }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            var response = new JqGridResponse()
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

            if (request.SortingName == "IncomeYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IncomeYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IncomeYear).ToList();
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

            if (request.SortingName == "TaxDeducted")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.TaxDeducted).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.TaxDeducted).ToList();
                }
            }

            #endregion
            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.IncomeYear,
                    d.EmpID,
                    d.EmployeeName,
                    d.EmployeeDesignation,
                    d.TaxDeducted,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public PartialViewResult AddDetail()
        {
            return PartialView("_Detail");
        }

        private TaxOpeningModel fillDetail(TaxOpeningModel model)
        {
            var taxdetail = _pgmCommonService.GetTaxOpeningDetail();

            if (taxdetail != null)
            {
                model.TaxOpeningDetailList = new Collection<TaxOpeningDetailModel>();

                foreach (var rec in taxdetail)
                {
                    var detail = new TaxOpeningDetailModel();

                    detail.IncomeHeadId = rec.IncomeHeadId;
                    detail.IncomeHead = rec.IncomeHead;
                    detail.IncomeAmount = rec.IncomeAmount;
                    detail.HeadSource = rec.HeadSource;

                    model.TaxOpeningDetailList.Add(detail);
                }
            }
            return model;
        }

        public ActionResult Create()
        {
            var model = new TaxOpeningModel();
            model.FromDate = DateTime.Now;
            model.ToDate = model.FromDate;

            model = fillDetail(model);
            PrepareModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(TaxOpeningModel model)
        {
            var errorList = new List<string>();
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (errorList.Count == 0))
            {
                try
                {
                    var entity = model.ToEntity();
                    foreach (var item in model.TaxOpeningDetailList.Select(x => x.ToEntity()))
                    {
                        entity.PGM_TaxOpeningDetail.Add(item);
                    }

                    entity = GetInsertUserAuditInfo(entity);

                    _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.Add(entity);
                    _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model = fillDetail(model);
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrMsg = Common.ErrorListToString(errorList);
            }

            PrepareModel(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.GetByID(id, "Id");

            var taxrate = _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.GetByID(id);

            var model = taxrate.ToModel();

            if (taxrate.PGM_TaxOpeningDetail != null)
            {
                model.TaxOpeningDetailList = new Collection<TaxOpeningDetailModel>();
                foreach (var detail in taxrate.PGM_TaxOpeningDetail)
                {
                    model.TaxOpeningDetailList.Add(detail.ToModel());
                }
                PrepareModel(model);
                model.Mode = CrudeAction.Edit;
            }

            string assessment = model.IncomeYear.ToString().Substring(0, 4);
            assessment = (Convert.ToInt16(assessment) + 1).ToString() + "-" + (Convert.ToInt16(assessment) + 2).ToString();
            model.AssessmentYear = assessment;

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
        public ActionResult Edit(TaxOpeningModel model)
        {
            var errorList = new List<string>();
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && (errorList.Count == 0))
            {
                try
                {
                    var entity = model.ToEntity();
                    var lstDetail = new ArrayList();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    if (model.TaxOpeningDetailList != null)
                    {
                        foreach (var detail in model.TaxOpeningDetailList)
                        {
                            var _detail = detail.ToEntity();
                            _detail.TaxOpeningId = entity.Id;

                            _detail.IUser = User.Identity.Name;
                            _detail.IDate = DateTime.Now;
                            _detail.EDate = DateTime.Now;
                            _detail.EUser = User.Identity.Name;
                            lstDetail.Add(_detail);
                        }
                    }
                    var NavigationList = new Dictionary<Type, ArrayList>();
                    NavigationList.Add(typeof(PGM_TaxOpeningDetail), lstDetail);

                    _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.Update(entity, NavigationList);
                    _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.SaveChanges();
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
                model.ErrMsg = Common.ErrorListToString(errorList);
            }

            PrepareModel(model);

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(PGM_TaxOpeningDetail) };

                _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.Delete(id, allTypes);
                _pgmCommonService.PGMUnit.TaxOpeningMasterRepository.SaveChanges();

                result = true;
            }
            catch (Exception ex)
            {
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                ModelState.AddModelError("Error", errMsg);
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

        private PGM_TaxOpening GetInsertUserAuditInfo(PGM_TaxOpening taxrate)
        {
            taxrate.IUser = User.Identity.Name;
            taxrate.IDate = DateTime.Now;

            foreach (var child in taxrate.PGM_TaxOpeningDetail)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;
            }
            return taxrate;
        }
        private PGM_TaxOpening GetEditUserAuditInfo(PGM_TaxOpening taxrate)
        {
            taxrate.EUser = User.Identity.Name;
            taxrate.EDate = DateTime.Now;

            foreach (var child in taxrate.PGM_TaxOpeningDetail)
            {
                child.EUser = User.Identity.Name;
                child.EDate = DateTime.Now;
            }
            return taxrate;
        }

        private void PrepareModel(TaxOpeningModel model)
        {
            model.IncomeYearList = IncomeYearList();

            var alreadyTaxProcessedEmps = _pgmCommonService.GetTaxProcessedEmployeeList();
            var activeEmps = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeListForDDL(LoggedUserZoneInfoId);

            var emp = activeEmps.Where(a => !alreadyTaxProcessedEmps.Select(t => t.EmployeeId).Contains(a.Id))
                .ToList();

            model.EmployeeList = Common.PopulateEmployeeDDL(emp);
        }

        public List<string> GetBusinessLogicValidation(TaxOpeningModel entity)
        {
            List<string> errorMessage = new List<string>();

            //Check whether from date and to date
            if (entity.FromDate > entity.ToDate)
            {
                errorMessage.Add("From date must be lower than the To date.");
            }

            //From and To Date must be between the Income Year
            DateTime dtIncomeFrom;
            DateTime dtIncomeTo;

            dtIncomeFrom = Convert.ToDateTime("01/July/" + entity.IncomeYear.Substring(0, 4));
            dtIncomeTo = dtIncomeFrom.AddYears(1).AddDays(-1);
            if (!(entity.FromDate >= dtIncomeFrom && entity.FromDate <= dtIncomeTo))
            {
                errorMessage.Add("Income From Date must be within the income year");
            }
            if (!(entity.ToDate >= dtIncomeFrom && entity.ToDate <= dtIncomeTo))
            {
                errorMessage.Add("Income To Date must be within the income year");
            }

            return errorMessage;

        }

        private IList<SelectListItem> IncomeYearList()
        {
            var IncomeYear = new List<SelectListItem>();
            IncomeYear = _pgmCommonService.PGMUnit.TaxRule.GetAll().DistinctBy(x => x.IncomeYear).Select(y => new SelectListItem()
            {
                Text = y.IncomeYear,
                Value = y.IncomeYear.ToString()
            }).ToList();

            return IncomeYear;
        }
        public ActionResult GetIncomeYearList()
        {
            var IncomeYear = new List<SelectListItem>();
            IncomeYear = _pgmCommonService.PGMUnit.TaxRule.GetAll().DistinctBy(x => x.IncomeYear).Select(y => new SelectListItem()
            {
                Text = y.IncomeYear,
                Value = y.IncomeYear.ToString()
            }).ToList();

            ViewBag.IncomeYearList = IncomeYear;
            return PartialView("_Select", IncomeYear);
        }

        public ActionResult GetDesignationList()
        {
            var Designation = new List<SelectListItem>();

            Designation = _pgmCommonService.PGMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList()
            .Select(y => new SelectListItem()
            {
                Text = y.Name,
                Value = y.Id.ToString()
            }).ToList();

            return PartialView("_Select", Designation);
        }

        #endregion Others
    }
}
