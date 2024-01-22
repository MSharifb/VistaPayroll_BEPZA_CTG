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
using System.Web.Script.Serialization;
using PGM.Web.Areas.PGM.Models.SalaryStructure;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class ArrearAdjustmentController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private static string message;
        #endregion

        #region Ctor

        public ArrearAdjustmentController(PGMCommonService pgmCommonservice)
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

                        where emp.ZoneInfoId == LoggedUserZoneInfoId
                        select new
                        {
                            EmployeeId = ad.EmployeeId,
                            EmpID = emp.EmpID,
                            FullName = emp.FullName,
                            Designation = emp.DesignationName,
                            Department = emp.DivisionName
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

            //if (request.SortingName == "ID")
            //{
            //    if (request.SortingOrder.ToString().ToLower() == "asc")
            //    {
            //        list = list.OrderBy(x => x.Id).ToList();
            //    }
            //    else
            //    {
            //        list = list.OrderByDescending(x => x.Id).ToList();
            //    }
            //}

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
                    list = list.OrderBy(x => x.FullName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FullName).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId), new List<object>()
                {
                    //d.Id,
                    d.EmployeeId,
                    d.EmpID,
                    d.FullName,
                    d.Department,
                    d.Designation
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new ArrearAdjustmentModel();
            model.IsAdjustWithSalary = true;
            PopulateDropdown(model);

            model.strMode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(ArrearAdjustmentModel model)
        {
            var x = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

            string errorList = string.Empty;
            model.IsError = 1;
            model.strMode = CrudeAction.AddNew;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    ApplyBusinessLogic(model);

                    model = CreateEntity(model);
                    var entity = model.ToEntity();

                    _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.Update(entity);

                    foreach (var item in model.ArrearAdjustmentDetailModelList)
                    {
                        var entityDetail = item.ToEntity();
                        _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Add(entityDetail);
                    }

                    _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return RedirectToAction("Index", new { message = model.Message, isSuccess = true });
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

            PopulateDropdown(model);

            if (model.IsError == 0)
                ViewBag.SuccessMessage = model.Message;
            else
                ViewBag.ErrorMessage = model.Message;

            return View(model);
        }

        private ArrearAdjustmentModel CreateEntity(ArrearAdjustmentModel model)
        {
            var NewModel = new ArrearAdjustmentModel();
            int ArrearAdjustmentId = model.ArrearAdjustmentId;

            var ExistingArrearAdjustmentEntity = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(ArrearAdjustmentId);
            var ExistingArrearAdjustmentModel = ExistingArrearAdjustmentEntity.ToModel();

            NewModel = ExistingArrearAdjustmentModel;

            NewModel.PaymentDate = model.PaymentDate;
            NewModel.IsAdjustWithSalary = model.IsAdjustWithSalary;
            NewModel.AdjustmentYear = model.AdjustmentYear;
            NewModel.AdjustmentMonth = model.AdjustmentMonth;
            //NewModel.IsAdjustmentPaid = true;
            NewModel.Remarks = model.Remarks;
            NewModel.ZoneInfoId = LoggedUserZoneInfoId;

            NewModel.ArrearAdjustmentDetailModelList = new List<ArrearAdjustmentDetailModel>();
            ArrearAdjustmentDetailModel ArrearAdjustmentDetailModel = null;
            foreach (var item in model.SalaryStructureDetail)
            {
                ArrearAdjustmentDetailModel = new ArrearAdjustmentDetailModel();
                ArrearAdjustmentDetailModel.ArrearAdjustmentId = ArrearAdjustmentId;
                ArrearAdjustmentDetailModel.HeadId = item.HeadId;
                ArrearAdjustmentDetailModel.HeadType = item.HeadType;
                ArrearAdjustmentDetailModel.HeadName = item.DisplayHeadName;

                ArrearAdjustmentDetailModel.AmountType = item.AmountType;
                ArrearAdjustmentDetailModel.BasedOn = "";// item.BasedOn;
                ArrearAdjustmentDetailModel.Amount = item.Amount;
                //ArrearAdjustmentDetailModel.TotalAmount = item.TotalAmount;

                NewModel.ArrearAdjustmentDetailModelList.Add(ArrearAdjustmentDetailModel);
            }

            return NewModel;
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var model = new ArrearAdjustmentModel();

            var employeeInfo = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(id);

            model.EmpID = employeeInfo.EmpID;
            model.EmployeeId = employeeInfo.Id;
            model.EmployeeName = employeeInfo.FullName;
            model.DesignationId = employeeInfo.DesignationId;
            model.DesignationName = employeeInfo.DesignationName;
            model.IsAdjustWithSalary = true;

            PopulateDropdown(model);

            model.SalaryStructureDetail = new List<SalaryStructureDetailsModel>();

            var pendingAdjustmentList = (from adj in _pgmCommonService.PGMUnit.ArrearAdjustmentRepository
                                             .Get(q => q.EmployeeId == id)
                                             .DefaultIfEmpty()
                                             .OfType<PGM_ArrearAdjustment>().ToList()
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
                                             PaymentStatus = adj.IsAdjustmentPaid == true ? "Paid" : "Unpaid",
                                             IsApproved = adj.IsApproved,
                                         }).DefaultIfEmpty().OrderBy(q => q.EffectiveDate).DefaultIfEmpty().OfType<ArrearAdjustmentListViewModel>().ToList();

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
                adjModel.PaymentStatus = item.PaymentStatus;
                adjModel.IsApproved = item.IsApproved;
                model.ArrearAdjustmentViewModelList.Add(adjModel);
            }

            return View(model);
        }

        public ActionResult DeleteArrearInformation(string id)
        {
            string message = string.Empty;
            string messageType = string.Empty;

            int selectedId = 0;
            int.TryParse(id, out selectedId);
            var adjustmentInfo = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(selectedId);
            if (adjustmentInfo != null)
            {
                int empStatusChangeId = (int)adjustmentInfo.EmpStatusChangeId;
                var statusChangeInfo = _pgmCommonService.PGMUnit.EmpStatusChangeRepository.GetByID(empStatusChangeId);
                if (statusChangeInfo != null)
                {
                    message = "Please delete employee " + adjustmentInfo.AdjustmentType;
                    messageType = "Error";
                }
                else
                {
                    var detailList = _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Get(q => q.ArrearAdjustmentId == selectedId).DefaultIfEmpty().OfType<PGM_ArrearAdjustmentDetail>().ToList();
                    foreach (var item in detailList)
                    {
                        _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Delete(item.Id);
                    }

                    _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.Delete(selectedId);
                    _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.SaveChanges();
                    _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.SaveChanges();

                    message = "Arrear adjustment has been deleted successfully.";
                    messageType = "Success";
                }
            }
            return Json(new
            {
                Message = message,
                type = messageType
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(ArrearAdjustmentModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.strMode = CrudeAction.Edit;
            errorList = GetBusinessLogicValidation(model);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    if (model.Id != 0)
                    {
                        var entity = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(model.Id);
                        entity.PaymentDate = model.IsAdjustWithSalary == false ? model.PaymentDate : null;
                        entity.IsAdjustWithSalary = model.IsAdjustWithSalary;
                        if (model.IsAdjustWithSalary)
                        {
                            entity.AdjustmentMonth = model.AdjustmentMonth;
                            entity.AdjustmentYear = model.AdjustmentYear;
                        }
                        else
                        {
                            entity.AdjustmentMonth = null;
                            entity.AdjustmentYear = null;
                        }
                        //entity.IsAdjustmentPaid = !model.IsAdjustWithSalary;
                        entity.Remarks = model.Remarks;
                        _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.Update(entity);

                        var adjustmentDetailList = _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Get(q => q.ArrearAdjustmentId == model.Id).DefaultIfEmpty().OfType<PGM_ArrearAdjustmentDetail>().ToList();

                        foreach (var item in model.SalaryStructureDetail)
                        {
                            var existingObj = adjustmentDetailList.Where(q => q.HeadId == item.HeadId).FirstOrDefault();
                            if (existingObj != null)
                            {
                                existingObj.Amount = (decimal)item.TotalAmount;
                                _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Update(existingObj);
                            }
                            else
                            {
                                var obj = new PGM_ArrearAdjustmentDetail
                                {
                                    ArrearAdjustmentId = model.Id,
                                    HeadId = item.HeadId,
                                    HeadName = item.DisplayHeadName,
                                    HeadType = item.HeadType,
                                    BasedOn = item.BasedOn == null ? string.Empty : item.BasedOn,
                                    AmountType = item.AmountType,
                                    Amount = (decimal)item.Amount,
                                    TotalAmount = (decimal)item.TotalAmount,
                                };
                                _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Add(obj);
                            }
                        }

                        _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.SaveChanges();
                        _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.SaveChanges();

                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        model.IsError = 0;

                        return RedirectToAction("Index", new { message = model.Message, isSuccess = true });
                    }
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.IsError = 1;
                model.Message = errorList;
            }

            model.strMode = CrudeAction.Edit;

            PopulateDropdown(model);
            foreach (var item in model.SalaryStructureDetail)
            {
                item.BasedOnList = PopulateBasedOn();
                item.HeadAmountTypeList = PopulateAmountType();
            }

            if (model.IsError == 0)
            {
                return View("Index", model);
            }

            ViewBag.ErrorMessage = model.Message;
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(int id)
        {
            bool isSuccessful = false;
            string errMsg = string.Empty;
            var entity = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(id);

            if (entity != null)
            {
                var model = entity.ToModel();
                model.strMode = CrudeAction.Delete;
                errMsg = GetBusinessLogicValidation(model);
            }

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    if (id != 0)
                    {
                        var detailList = _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Get(q => q.ArrearAdjustmentId == id).DefaultIfEmpty().OfType<PGM_ArrearAdjustmentDetail>().ToList();
                        foreach (var item in detailList)
                        {
                            _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.Delete(item.Id);
                        }

                        _pgmCommonService.PGMUnit.ArrearAdjustmentDetailRepository.SaveChanges();
                        _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.Delete(id);
                        _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.SaveChanges();

                        message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                        isSuccessful = true;
                    }
                }
                catch (Exception ex)
                {
                    isSuccessful = false;
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }
            else
            {
                isSuccessful = false;
            }

            return Json(new
            {
                Success = isSuccessful,
                Message = isSuccessful == true ? message : errMsg
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

        [NoCache]
        private string GetValidationChecking(ArrearAdjustmentModel model)
        {
            string errorMessage = string.Empty;

            return errorMessage;
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {
            var list = _pgmCommonService.PGMUnit.DesignationRepository.GetAll();

            var listDes = Common.PopulateDDLList(list);

            return PartialView("_Select", listDes);
        }

        [NoCache]
        public JsonResult AutoCompleteEmpInitaial(string term)
        {
            var result = (from r in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                          where r.EmployeeInitial.ToLower().Contains(term.ToLower())
                          select new { r.EmployeeInitial, r.FullName }).Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
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
                    detail.HeadAmountTypeList = PopulateAmountType();
                    detail.BasedOnList = PopulateBasedOn();
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
                    detail.HeadAmountTypeList = PopulateAmountType();
                    detail.BasedOnList = PopulateBasedOn();
                    salaryStructureModelList.Add(detail);
                }
            }

            model.SalaryStructureDetail = salaryStructureModelList;
            return PartialView("_ArrearAddjustDetail", model);
        }

        [NoCache]
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


        #region Utilities

        private IList<SelectListItem> PopulateAmountType()
        {
            var itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem { Text = "Fixed", Value = "Fixed" });
            itemList.Add(new SelectListItem { Text = "Percent", Value = "Percent" });
            return itemList;
        }

        private IList<SelectListItem> PopulateBasedOn()
        {
            var itemList = new List<SelectListItem>();
            itemList.Add(new SelectListItem { Text = "", Value = "" });
            itemList.Add(new SelectListItem { Text = "Basic", Value = "Basic" });
            itemList.Add(new SelectListItem { Text = "Gross", Value = "Gross" });
            return itemList;
        }

        private void PopulateDropdownList(SalaryStructureDetailsModel model)
        {
            model.HeadAmountTypeList = Common.PopulateAmountType().ToList();
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

        private void ApplyBusinessLogic(ArrearAdjustmentModel model)
        {

        }

        [HttpGet]
        public PartialViewResult GetArrearAdjustmentListByEmployeeId(int pEmployeeId)
        {
            var model = new ArrearAdjustmentModel();

            var list = (from a in _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.Get(t => t.EmployeeId == pEmployeeId).ToList()
                        select new ArrearAdjustmentModel()
                        {
                            Id = a.Id,
                            EmployeeId = a.EmployeeId,
                            DesignationId = a.EmployeeId,

                            OrderDate = Convert.ToDateTime(a.OrderDate),

                            AdjustmentType = a.AdjustmentType,
                            PaymentDate = a.PaymentDate,

                            ArrearFromDate = Convert.ToDateTime(a.ArrearFromDate),
                            ArrearToDate = Convert.ToDateTime(a.ArrearToDate),

                            IsAdjustWithSalary = Convert.ToBoolean(a.IsAdjustWithSalary),
                            AdjustmentYear = a.AdjustmentYear,
                            AdjustmentMonth = a.AdjustmentMonth,
                            IsAdjustmentPaid = Convert.ToBoolean(a.IsAdjustmentPaid),
                            Remarks = a.Remarks,

                            IUser = a.IUser,
                            IDate = a.IDate,
                            EUser = a.EUser,
                            EDate = a.EDate,

                        }).ToList();

            //model.ArrearAdjustmentModelList = list;
            return PartialView("_Details", model);
        }

        [HttpGet]
        public string GetArrearAdjustmentDDLListByEmployeeId(int pEmployeeId)
        {
            var listItems = new List<SelectListItem>();

            var items = (from entity in _pgmCommonService.PGMUnit.ArrearAdjustmentRepository
                            .Get(t => t.EmployeeId == pEmployeeId
                                && t.IsAdjustmentPaid == false)
                            .ToList()
                         select entity).OrderBy(o => o.Id).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var listItem = new SelectListItem { Text = item.AdjustmentType + " [" + item.OrderDate.ToString(DateAndTime.GlobalDateFormat) + "]", Value = item.Id.ToString() };
                    listItems.Add(listItem);
                }
            }

            return new JavaScriptSerializer().Serialize(listItems);
        }

        [HttpGet]
        public PartialViewResult SetArrearAdjustmentDetail(int pArrearAdjustmentId)
        {
            var salaryDetails = GetArrearAdjustmentDetail(pArrearAdjustmentId);

            var model = new ArrearAdjustmentModel();
            model.SalaryStructureDetail = new List<SalaryStructureDetailsModel>();
            model.SalaryStructureDetail = salaryDetails;

            return PartialView("_ArrearAddjustDetail", model);
        }

        private IList<Tuple<string, string>> GenerateSalaryYearMonth(DateTime from, DateTime to)
        {
            IList<Tuple<string, string>> yearMonth = new List<Tuple<string, string>>(50);

            while (from < to)
            {
                yearMonth.Add(new Tuple<string, string>(from.Year.ToString(), UtilCommon.GetMonthName(from.Month)));
                from = from.AddMonths(1);
            }

            return yearMonth;
        }

        private IList<SalaryStructureDetailsModel> GetArrearAdjustmentDetail(int pArrearAdjustmentId)
        {
            var salaryDetails = new List<SalaryStructureDetailsModel>();

            var arrarAdjustmentEntity = _pgmCommonService.PGMUnit.ArrearAdjustmentRepository.GetByID(pArrearAdjustmentId);
            string Addition = PGMEnum.SalaryHeadType.Addition.ToString();
            string Fixed = PRMEnum.FixedPercent.Fixed.ToString();

            // Find out already paid salary
            List<PGM_SalaryDetail> list_PGM_SalaryDetail_Old = new List<PGM_SalaryDetail>();
            IList<Tuple<string, string>> listArrearYearMonth = GenerateSalaryYearMonth(arrarAdjustmentEntity.ArrearFromDate, arrarAdjustmentEntity.ArrearToDate);

            foreach (var item in listArrearYearMonth)
            {
                var salary = (from sal in _pgmCommonService.PGMUnit.SalaryMasterRepository.Get(t => t.SalaryYear == item.Item1 && t.SalaryMonth == item.Item2 && t.EmployeeId == arrarAdjustmentEntity.EmployeeId)
                              join salDtl in _pgmCommonService.PGMUnit.SalaryDetailsRepository.Get(t => t.HeadType == Addition) on sal.Id equals salDtl.SalaryId
                              select new PGM_SalaryDetail
                              {
                                  HeadId = salDtl.HeadId,
                                  HeadType = salDtl.HeadType,
                                  HeadAmount = salDtl.HeadAmount,
                              }).ToList();

                list_PGM_SalaryDetail_Old.AddRange(salary);
            }

            var list_PGM_SalaryDetail_Old_GroupBy =
                list_PGM_SalaryDetail_Old
                    .GroupBy(t => t.HeadId)
                    .Select(s => new PGM_SalaryDetail
                    {
                        HeadId = s.First().HeadId,
                        HeadType = s.First().HeadType,
                        HeadAmount = s.Sum(a => a.HeadAmount),
                    }).ToList();
            //


            // Find out to be paid salary
            var list_PGM_SalaryDetail_New = new List<PGM_SalaryDetail>();
            int mutiplyFactor = listArrearYearMonth.Count();
            decimal BasicSalary = 0M, HeadAmount = 0M;

            var empSalaryStructure = _pgmCommonService.PGMUnit.FunctionRepository.GetSalaryInfoConsiderIncPro(arrarAdjustmentEntity.EmployeeId, arrarAdjustmentEntity.ArrearFromDate).Where(t => t.HeadType == Addition);
            foreach (var item in empSalaryStructure)
            {
                var salDtl = new PGM_SalaryDetail();
                salDtl.HeadId = item.HeadId;
                salDtl.HeadType = item.HeadType;

                BasicSalary = Convert.ToDecimal(item.BasicSalary);
                HeadAmount = Convert.ToDecimal(item.Amount);

                salDtl.HeadAmount = item.AmountType == Fixed ? HeadAmount : BasicSalary * HeadAmount / 100;
                salDtl.HeadAmount = salDtl.HeadAmount * mutiplyFactor;

                list_PGM_SalaryDetail_New.Add(salDtl);
            }

            //findout difference amount of each head
            foreach (var item in list_PGM_SalaryDetail_New)
            {
                var salaryDetailOld = list_PGM_SalaryDetail_Old_GroupBy.Where(t => t.HeadId == item.HeadId).SingleOrDefault();

                if (salaryDetailOld != null)
                {
                    item.HeadAmount = item.HeadAmount - salaryDetailOld.HeadAmount;
                }
            }


            var salaryHeadList = _pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll().ToList();
            salaryDetails = new List<SalaryStructureDetailsModel>();

            foreach (var item in salaryHeadList)
            {
                var childModel = new SalaryStructureDetailsModel();
                PopulateDropdownList(childModel);

                childModel.HeadId = item.Id;
                childModel.IsTaxable = item.IsTaxable;
                childModel.HeadType = item.HeadType;
                childModel.DisplayHeadName = item.HeadName;
                childModel.AmountType = item.AmountType;
                childModel.IsGrossPayHead = item.IsGrossPayHead;

                var salaryDetailNew = list_PGM_SalaryDetail_New.Where(t => t.HeadId == item.Id).SingleOrDefault();
                if (salaryDetailNew != null)
                {
                    childModel.Amount = salaryDetailNew.HeadAmount;
                    //childModel.TotalAmount = salaryDetailNew.TotalAmount;
                }
                else
                {
                    childModel.Amount = 0.0M;
                    //childModel.TotalAmount = 0.0M;
                }

                salaryDetails.Add(childModel);
            }

            return salaryDetails;
        }

        #endregion
    }

}
