using Domain.PGM;
using PGM.Web.Areas.PGM.Models.NightBill;
using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class NightBillController : BaseController
    {
        private readonly PGMCommonService _pgmCommonService;

        public NightBillController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }
        // GET: PGM/NightBill
        public ActionResult Index()
        {
            var model = new NightBillPaymentViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, NightBillPaymentViewModel model)
        {

            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<NightBillPaymentViewModel> list = (from nb in _pgmCommonService.PGMUnit.NightBillPaymentMasterRepository.GetAll()
                                                    join dep in _pgmCommonService.PGMUnit.DivisionRepository.GetAll() on nb.DepartmentId equals dep.Id
                                                    //join empHis in _prmCommonservice.PRMUnit.FunctionRepository.GetEmployeeFromHistory(null, null, DateTime.Now, LoggedUserZoneInfoId, null, null, null, null, null, null, null, null, null) on nb.Empl equals empHis.EmployeeId

                                                    where
                                                    //nb.ZoneInfoId == LoggedUserZoneInfoId
                                                    (model.BillMonth == null || model.BillMonth == "0" || model.BillMonth == nb.BillMonth)
                                                    && (model.BillYear == null || model.BillYear == 0 || model.BillYear == nb.BillYear)
                                                    && (model.DepartmentId == null || model.DepartmentId == 0 || model.DepartmentId == nb.DepartmentId)
                                                    select new NightBillPaymentViewModel
                                                    {
                                                        Id = nb.Id,
                                                        BillMonth = nb.BillMonth,
                                                        BillYear = nb.BillYear,
                                                        DepartmentName = dep.Name,
                                                        DepartmentId = nb.DepartmentId,
                                                        PaymentDate = nb.PaymentDate
                                                    }
                                                ).OrderBy(x => x.Id).ToList();
            if (request.Searching)
            {

                if ((model.PaymentDate != null && model.PaymentDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.PaymentDate == model.PaymentDate).ToList();
                }

            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "PaymentDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PaymentDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PaymentDate).ToList();
                }
            }


            if (request.SortingName == "DepartmentName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DepartmentName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DepartmentName).ToList();
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
                    d.DepartmentName,
                    d.BillMonth ,
                    d.BillYear ,
                    d.DepartmentId,
                    d.PaymentDate.ToString("dd/MM/yyyy"),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new NightBillPaymentViewModel();
            model.PaymentDate = DateTime.Now;
            populateDropdown(model);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(NightBillPaymentViewModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    var entity = model.ToEntity();
                    //entity.ZoneInfoId = LoggedUserZoneInfoId;
                    if (model.NightBillPaymentDetail.Count() > 0)
                    {
                        if (entity.Id <= 0)
                        {

                            _pgmCommonService.PGMUnit.NightBillPaymentMasterRepository.Add(entity);
                            _pgmCommonService.PGMUnit.NightBillPaymentMasterRepository.SaveChanges();
                        }
                        else
                        {
                            foreach (var item in model.NightBillPaymentDetail.Select(x => x.ToEntity()))
                            {
                                if (item.Id > 0)
                                {
                                    _pgmCommonService.PGMUnit.NightBillPaymentDetailsRepository.Update(item);
                                }
                            }
                            _pgmCommonService.PGMUnit.NightBillPaymentDetailsRepository.SaveChanges();
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Message = "Night Bill Payment List Empty";
                    }
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

            model.ErrMsg = Message;
            populateDropdown(model);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        public ActionResult Edit(int? Id)
        {
            var model = _pgmCommonService.PGMUnit.NightBillPaymentMasterRepository.GetByID(Id).ToModel();
            if (model != null)
            {
                var list = _pgmCommonService.PGMUnit.NightBillPaymentDetailsRepository.GetAll().Where(e => e.BillId == Id).ToList().Select(x => x.ToModel());
                //model.NightBillPaymentDetail = list.ToList();

                List<NightBillPaymentDetailViewModel> detail = (from emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                                                                where list.Select(x => x.EmployeeId).Contains(emp.Id)
                                                                select new NightBillPaymentDetailViewModel
                                                                {
                                                                    EmployeeId = emp.Id,
                                                                    EmployeeName = emp.FullName,
                                                                    Designation = emp.DesignationName,
                                                                    ICNo = emp.EmpID
                                                                }
                                                               ).OrderBy(x => x.EmployeeId).ToList();
                foreach (var item in list)
                {
                    var empObj = detail.Where(e => e.EmployeeId == item.EmployeeId).FirstOrDefault();
                    item.EmployeeName = empObj.EmployeeName;
                    item.Designation = empObj.Designation;
                    item.ICNo = empObj.ICNo;
                    model.NightBillPaymentDetail.Add(item);
                }

            }

            populateDropdown(model);
            model.Mode = CrudeAction.Edit;
            return View("Create", model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _pgmCommonService.PGMUnit.NightBillPaymentMasterRepository.GetByID(id);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(DAL.PGM.PGM_NightBillPaymentDetail) };

                _pgmCommonService.PGMUnit.NightBillPaymentMasterRepository.Delete(entity.Id, allTypes);

                _pgmCommonService.PGMUnit.NightBillPaymentMasterRepository.SaveChanges();

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


        public PartialViewResult AddDetail(int? Id, string billMonth, int? billYear)
        {
            NightBillPaymentViewModel model = new NightBillPaymentViewModel();
            var list = _pgmCommonService.GetNightBillEmployeeList(Id
                                                                , UtilCommon.GetMonthNo(billMonth)
                                                                , billYear)
                                            .Select(x => x.ToModel());

            if (list != null)
            {
                List<NightBillPaymentDetailViewModel> detail =
                (from emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                 where emp.ZoneInfoId == LoggedUserZoneInfoId && list.Select(x => x.EmployeeId).Contains(emp.Id)
                 select new NightBillPaymentDetailViewModel
                 {
                     EmployeeId = emp.Id,
                     EmployeeName = emp.FullName,
                     Designation = emp.DesignationName,
                     ICNo = emp.EmpID
                 }
                ).OrderBy(x => x.EmployeeId).ToList();

                if (detail != null)
                {
                    NightBillPaymentDetailViewModel empObj = null;
                    foreach (var item in list)
                    {
                        empObj = detail.FirstOrDefault(e => e.EmployeeId == item.EmployeeId);
                        if (empObj != null)
                        {
                            item.EmployeeName = empObj.EmployeeName;
                            item.Designation = empObj.Designation;
                            item.ICNo = empObj.ICNo;
                            model.NightBillPaymentDetail.Add(item);
                        }
                    }
                }
            }
            return PartialView("_MasterDetail", model);
        }

        private void populateDropdown(NightBillPaymentViewModel model)
        {
            #region Head
            var List = _pgmCommonService.PGMUnit.DivisionRepository.GetAll().OrderBy(x => x.Name).ToList();
            model.DepartmentList = Common.PopulateDDLList(List);
            model.MonthList = Common.PopulateMonthList();
            model.YearList = Common.PopulateYearList();
            #endregion

        }

        [NoCache]
        public ActionResult GetDepartment()
        {
            var divisions = _pgmCommonService.PGMUnit.DivisionRepository.GetAll().Where(e => e.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.Name).ToList();

            return PartialView("Select", Common.PopulateDDLList(divisions));
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

    }
}