using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.ArrearAdjustment;
using PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PGM.Web.Areas.PGM.Models.SalaryStructure;


namespace PGM.Web.Areas.PGM.Controllers
{
    public class ArrearAdjustmentApprovalController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        
        #endregion

        #region Ctor

        public ArrearAdjustmentApprovalController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region Message Properties

        public string Message { get; set; }

        #endregion

        [NoCache]
        public ActionResult Index(string message, bool? isSuccess)
        {
            var model = new ArrearAdjustmentModel();
            PopulateDropdown(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public JsonResult GetList(JqGridRequest request, ArrearAdjustmentModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from ad in _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetAll()
                        join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on ad.EmployeeId equals emp.Id

                        where emp.ZoneInfoId == LoggedUserZoneInfoId && !ad.IsApproved
                        && (string.IsNullOrEmpty(model.EmployeeName) || model.EmployeeName == emp.FullName)
                        && (string.IsNullOrEmpty(model.EmpID) || model.EmpID == emp.EmpID)
                        select new ArrearAdjustmentModel
                        {
                            Id = ad.Id,
                            EmployeeId = ad.EmployeeId,
                            EmpID = emp.EmpID,
                            EmployeeName = emp.FullName,
                            DesignationName = emp.DesignationName,
                            OrderDate = ad.OrderDate,
                            EffectiveDate = ad.EffectiveDate,
                            AdjustmentType = ad.AdjustmentType
                        }).DistinctBy(q => q.EmployeeId).ToList();

            totalRecords = list.Count;
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            #region Sorting

            if (request.SortingName == "EffectiveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDate).ToList();
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

            if (request.SortingName == "FullName")
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId), new List<object>()
                {
                    d.Id,
                    d.EmpID,
                    d.EmployeeName,
                    d.DesignationName,
                    d.OrderDate,
                    d.EffectiveDate,
                    d.AdjustmentType
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var arrear = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.Get(q => q.Id == id).FirstOrDefault();
            var model = new ArrearAdjustmentModel();
            var employeeInfo = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(arrear.EmployeeId);
            model.EmpID = employeeInfo.EmpID;
            model.EmployeeId = employeeInfo.Id;
            model.EmployeeName = employeeInfo.FullName;
            model.DesignationId = employeeInfo.DesignationId;
            model.DesignationName = employeeInfo.DesignationName;
            model.IsAdjustWithSalary = true;

            PopulateDropdown(model);

            model.SalaryStructureDetail = new List<SalaryStructureDetailsModel>();

            var pendingAdjustmentList = (from adj in _pgmCommonService.PGMUnit
                                                    .ArrearAdjustmentRepository
                                                    .Get(q => q.EmployeeId == model.EmployeeId).ToList()
                                         select new ArrearAdjustmentListViewModel
                                         {
                                             Id = adj.Id,
                                             IUser = adj.IUser,
                                             IDate = adj.IDate,
                                             ArrearFromDate = adj.ArrearFromDate,
                                             ArrearToDate = adj.ArrearToDate,
                                             OrderDate = adj.OrderDate,
                                             ArrearType = adj.AdjustmentType,
                                             EffectiveDate = adj.EffectiveDate == null ? adj.OrderDate : (DateTime)adj.EffectiveDate,
                                             IsApproved = adj.IsApproved,
                                             ApprovalStatus = adj.IsApproved ? "Approved" : "Pending",
                                         }).OrderBy(q => q.EffectiveDate).ToList();

            foreach (var item in pendingAdjustmentList)
            {
                var adjModel = new ArrearAdjustmentListViewModel();
                adjModel.Id = item.Id;
                adjModel.IUser = item.IUser;
                adjModel.IDate = item.IDate;
                adjModel.ArrearFromDate = item.ArrearFromDate;
                adjModel.ArrearToDate = item.ArrearToDate;
                adjModel.OrderDate = item.OrderDate;
                adjModel.ArrearType = item.ArrearType;
                adjModel.EffectiveDate = item.EffectiveDate;
                adjModel.IsApproved = item.IsApproved;
                adjModel.ApprovalStatus = item.ApprovalStatus;
                model.ArrearAdjustmentViewModelList.Add(adjModel);
            }
            return View(model);
        }

        public ActionResult ApproveArrearAdjustment(int arrearAdjustmentId)
        {
            string message = string.Empty;

            try
            {
                var entity = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(arrearAdjustmentId);
                if (entity != null)
                {
                    entity.IsApproved = true;

                    if (!Common.GetBoolean(entity.IsAdjustWithSalary))
                    {
                        entity.IsAdjustmentPaid = true;
                    }

                    _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.Update(entity);
                    _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.SaveChanges();
                    message = "Success";
                }
            }
            catch (Exception ex)
            {
                message = "Failed";
                CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
            return Json(new
            {
                Message = message
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetBusinessLogicValidation(ArrearAdjustmentModel model)
        {
            if (model.IsAdjustWithSalary == true)
            {
                if ((model.AdjustmentYear == null || model.AdjustmentYear.Length == 0)
                    || (model.AdjustmentYear == null || model.AdjustmentMonth.Length == 0))
                {
                    return "Please select adjusment year and month.";
                }
            }

            return string.Empty;
        }

        public ActionResult GenerateSalaryStructure(string selectedIdString)
        {
            var model = new ArrearAdjustmentModel();
            var salaryStructureModelList = new List<SalaryStructureDetailsModel>();

            int selectedId = 0;
            int.TryParse(selectedIdString, out selectedId);

            var itemList = _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Get(q => q.ArrearAdjustmentId == selectedId).DefaultIfEmpty().OfType<PGM_ArrearAdjustmentDetail>().ToList();
            if (itemList != null && itemList.Count > 0)
            {
                var arrearMaster = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(selectedId);
                foreach (var item in itemList)
                {
                    var detail = new SalaryStructureDetailsModel
                    {
                        HeadId = (int)item.HeadId,
                        DisplayHeadName = item.HeadName,
                        EmployeeId = arrearMaster.EmployeeId,
                        AmountType = item.AmountType,
                        Amount = item.Amount,
                        HeadType = item.HeadType,
                        BasedOn = item.BasedOn,
                        TotalAmount = item.TotalAmount
                    };
                    salaryStructureModelList.Add(detail);
                }
            }
            else
            {
                var list = _pgmCommonService.PGMUnit.FunctionRepository.GetSalaryStructureForArrearAdjustment(selectedIdString);
                foreach (var item in list)
                {
                    var detail = new SalaryStructureDetailsModel
                    {
                        HeadId = (int)item.HeadId,
                        DisplayHeadName = item.HeadName,
                        EmployeeId = (int)item.EmployeeId,
                        AmountType = item.AmountType,
                        Amount = item.Amount,
                        HeadType = item.HeadType,
                        BasedOn = item.BasedOn,
                        TotalAmount = item.TotalAmount
                    };
                    salaryStructureModelList.Add(detail);
                }
            }

            model.SalaryStructureDetail = salaryStructureModelList;
            return PartialView("_ArrearAddjustDetail", model);
        }

        public ActionResult GetAdjustmentMasterInfo(string selectedIdString)
        {
            int selectedId = 0;
            int.TryParse(selectedIdString, out selectedId);
            var arrear = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(selectedId);
            if (arrear != null)
            {
                return Json(new
                {
                    IsArrearFound = true,
                    IsAdjustWithSalary = arrear.IsAdjustWithSalary,
                    PaymentDate = arrear.PaymentDate,
                    AdjustmentMonth = arrear.AdjustmentMonth,
                    AdjustmentYear = arrear.AdjustmentYear,
                    Remarks = arrear.Remarks
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    IsArrearFound = false,
                    AdjustmentMonth = string.Empty,
                    AdjustmentYear = string.Empty,
                    Remarks = string.Empty
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private void PopulateDropdown(ArrearAdjustmentModel model)
        {
            if (model != null && model.EmployeeId != 0)
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                model.EmpID = emp.EmpID;
                model.EmployeeName = emp.FullName;
                model.DesignationName = emp.DesignationName;
            }

            model.YearList = Common.PopulateYearList();
            model.MonthList = Common.PopulateMonthList();
        }

    }

}
