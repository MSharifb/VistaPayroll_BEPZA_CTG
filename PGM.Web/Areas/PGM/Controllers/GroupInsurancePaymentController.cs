using Domain.PGM;
using PGM.Web.Areas.PGM.Models.GroupInsurancePayment;
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
    public class GroupInsurancePaymentController : BaseController
    {
        private readonly PGMCommonService _pgmCommonService;

        public GroupInsurancePaymentController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        // GET: PGM/GroupInsurancePayment
        public ActionResult Index()
        {
            var model = new GroupInsurancePaymentViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, GroupInsurancePaymentViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<GroupInsurancePaymentViewModel> list = (from HM in _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.GetAll()
                                                         join gt in _pgmCommonService.PGMUnit.GroupInsurancePaymentTypeRepository.GetAll() on HM.PaymentTypeId equals gt.Id
                                                         join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on HM.EmployeeId equals emp.Id

                                                         where emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                                                         && (model.ICNo == null || model.ICNo == "" || emp.EmpID == model.ICNo)
                                                         && (model.OrderNo == null || model.OrderNo == "" || model.OrderNo == HM.OrderNo)

                                                         select new GroupInsurancePaymentViewModel
                                                         {
                                                             Id = HM.Id,
                                                             EmployeeName = emp.FullName,
                                                             ICNo = emp.EmpID,
                                                             OrderNo = HM.OrderNo,
                                                             PaymentType = gt.PaymentType,
                                                             PaymentAmount = gt.PaymentAmount,
                                                             PaymentDate = HM.PaymentDate
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

            if (request.SortingName == "PaymentType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PaymentType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PaymentType).ToList();
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
                    d.EmployeeName,
                    d.ICNo,
                    d.OrderNo,
                    d.PaymentType,
                    d.PaymentAmount,
                    d.PaymentDate.ToString("dd/MM/yyyy"),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new GroupInsurancePaymentViewModel();
            model.PaymentDate = DateTime.Now;
            populateDropdown(model);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(GroupInsurancePaymentViewModel model)
        {
            string errorList = string.Empty;

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    var entity = model.ToEntity();
                    //entity.ZoneInfoId = LoggedUserZoneInfoId;
                    if (entity.Id > 0)
                        _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.Update(entity);
                    else
                        _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.Add(entity);

                    _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.SaveChanges();

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

            model.Mode = CrudeAction.Create;
            return View(model);
        }

        public ActionResult Edit(int? Id)
        {
            var model = _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.GetByID(Id).ToModel();

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList().Where(e => e.Id == model.EmployeeId).FirstOrDefault();
            var paymentType = _pgmCommonService.PGMUnit.GroupInsurancePaymentTypeRepository.GetAll().Where(e => e.Id == model.PaymentTypeId).FirstOrDefault();
            model.ICNo = emp.EmpID;
            model.EmployeeName = emp.FullName;
            model.Designation = emp.DesignationName;
            model.PaymentAmount = paymentType.PaymentAmount;

            populateDropdown(model);
            model.Mode = CrudeAction.Edit;
            return View("Create", model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.GetByID(id);

            try
            {
                _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.Delete(entity.Id);
                _pgmCommonService.PGMUnit.GroupInsurancePaymentRepository.SaveChanges();

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
        private void populateDropdown(GroupInsurancePaymentViewModel model)
        {
            #region Head
            var List = _pgmCommonService.PGMUnit.GroupInsurancePaymentTypeRepository.GetAll().OrderBy(x => x.PaymentType).ToList();
            model.PaymentTypeList = Common.PopulatePaymentTypeDDL(List);
            #endregion

        }

        public JsonResult GetPaymentType(int typeId)
        {
            var paymentType = _pgmCommonService.PGMUnit.GroupInsurancePaymentTypeRepository.GetAll().Where(e => e.Id == typeId).FirstOrDefault();
            return Json(new
            {
                PaymentAmount = paymentType.PaymentAmount
            }, JsonRequestBehavior.AllowGet);
        }
    }
}