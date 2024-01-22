using Domain.PGM;
using PGM.Web.Areas.PGM.Models.ElectricBill;
using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class ElectricBillController : BaseController
    {
        private readonly PGMCommonService _pgmCommonService;

        public ElectricBillController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        // GET: PGM/ElectricBill
        public ActionResult Index()
        {
            var model = new ElectricBillViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, ElectricBillViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<ElectricBillViewModel> list = (from eb in _pgmCommonService.PGMUnit.ElectricBillRepository.GetAll()
                                                join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on eb.EmployeeId equals emp.Id

                                                where (string.IsNullOrEmpty(model.BillMonth) || model.BillMonth == eb.BillMonth)
                                                && (model.BillYear == 0 || model.BillYear == eb.BillYear)
                                                && (string.IsNullOrEmpty(model.ICNo) || model.ICNo == emp.EmpID)
                                                && (string.IsNullOrEmpty(model.EmployeeName) || emp.FullName.Contains(model.EmployeeName))
                                                && emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId

                                                select new ElectricBillViewModel
                                                {
                                                    Id = eb.Id,
                                                    ICNo = emp.EmpID,
                                                    EmployeeId = eb.EmployeeId,
                                                    EmployeeName = emp.FullName,
                                                    Designation = emp.DesignationName,
                                                    ElectricMeterNo = String.Empty,
                                                    BillMonth = eb.BillMonth,
                                                    BillYear = eb.BillYear,
                                                    TotalBill = eb.TotalBill,
                                                    PrepareDate = eb.PrepareDate
                                                }).OrderBy(x => x.Id).ToList();
            if (request.Searching)
            {
                if ((model.PrepareDate != null && model.PrepareDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.PrepareDate == model.PrepareDate).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "PrepareDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PrepareDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PrepareDate).ToList();
                }
            }

            if (request.SortingName == "ICNo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ICNo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ICNo).ToList();
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
            if (request.SortingName == "Designation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Designation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Designation).ToList();
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ICNo ,
                    d.EmployeeId ,
                    d.EmployeeName ,
                    d.Designation ,
                    d.ElectricMeterNo ,
                    d.BillMonth ,
                    d.BillYear ,
                    d.TotalBill ,
                    d.PrepareDate.ToString(),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new ElectricBillViewModel();
            model.PrepareDate = DateTime.Now;
            model.FromDate = DateTime.Now;
            model.ToDate = DateTime.Now;
            populateDropdown(model);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(ElectricBillViewModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    var entity = model.ToEntity();
                    //entity.ZoneInfoId = LoggedUserZoneInfoId;
                    if (entity.Id > 0)
                        _pgmCommonService.PGMUnit.ElectricBillRepository.Update(entity);
                    else
                        _pgmCommonService.PGMUnit.ElectricBillRepository.Add(entity);

                    _pgmCommonService.PGMUnit.ElectricBillRepository.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                Message = errorList;
            }

            populateDropdown(model);
            model.ErrMsg = Message;
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        public ActionResult Edit(int? Id)
        {
            var model = _pgmCommonService.PGMUnit.ElectricBillRepository.GetByID(Id).ToModel();
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            model.ICNo = emp.EmpID;
            model.EmployeeName = emp.FullName;
            model.Designation = emp.DesignationName;
            model.Department = emp.DivisionName;

            populateDropdown(model);
            model.Mode = CrudeAction.Edit;
            return View("Create", model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _pgmCommonService.PGMUnit.ElectricBillRepository.GetByID(id);

            try
            {
                _pgmCommonService.PGMUnit.ElectricBillRepository.Delete(entity.Id);
                _pgmCommonService.PGMUnit.ElectricBillRepository.SaveChanges();

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

        [NoCache]
        public ActionResult GetMonth()
        {
            return PartialView("Select", Common.PopulateMonthList());
        }

        [NoCache]
        public ActionResult GetYear()
        {
            return PartialView("Select", Common.PopulateYearList());
        }

        private void populateDropdown(ElectricBillViewModel model)
        {
            #region Head
            var List = _pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll().OrderBy(x => x.HeadName).ToList();
            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(List);
            model.MonthList = Common.PopulateMonthList();
            model.YearList = Common.PopulateYearList();
            #endregion

        }

    }
}