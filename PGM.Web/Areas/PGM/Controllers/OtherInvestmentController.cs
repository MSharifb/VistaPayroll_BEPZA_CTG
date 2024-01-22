using DAL.PGM;
using Domain.PGM;

using Utility;
using PGM.Web.Areas.PGM.Models.Document;
using PGM.Web.Areas.PGM.Models.TaxOtherInvestment;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Web;
using System.Web.Mvc;

using PGM.Web.Resources;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class OtherInvestmentController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor

        public OtherInvestmentController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion
        //
        // GET: /PGM/OtherInvestment/
        #region Action
        public ActionResult Index(TaxOtherInvestmentViewModel model)
        {
            ModelState.Clear();
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TaxOtherInvestmentViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from TP in _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetAll()
                        join Emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on TP.EmployeeId equals Emp.Id

                        where TP.EntityType == Convert.ToByte(PGMEnum.TaxEntityType.OtherInvestment)
                        && Emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId

                        select new TaxOtherInvestmentViewModel()
                        {
                            Id = TP.Id,
                            EmployeeId = Emp.Id,
                            IncomeYear = TP.IncomeYear,
                            AssessmentYear = TP.AssessmentYear,
                            EmpId = Emp.EmpID,
                            EmployeeName = Emp.FullName

                        }).OrderBy(x => x.IncomeYear).ToList();

            totalRecords = list == null ? 0 : list.Count;

            if (request.Searching)
            {
                if (viewModel.IncomeYear != null && viewModel.IncomeYear != null)
                {
                    list = list.Where(x => x.IncomeYear == viewModel.IncomeYear).ToList();
                }

                if (!string.IsNullOrEmpty(viewModel.AssessmentYear))
                {
                    list = list.Where(x => x.AssessmentYear.Trim().ToLower().Contains(viewModel.AssessmentYear.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(viewModel.EmpId))
                {
                    list = list.Where(x => x.EmpId.Contains(viewModel.EmpId) || x.EmployeeName.ToLower().Contains(viewModel.EmpId.Trim().ToLower())).ToList();
                }

            }
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling(totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            foreach (var d in list)
            {
                decimal totalAmount = GetTotalAmount(d.Id);
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmployeeId,
                    d.IncomeYear,
                    d.AssessmentYear,
                    d.EmpId,
                    d.EmployeeName,
                    totalAmount,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new TaxOtherInvestmentViewModel();
            model.strMode = "Create";
            PopulateList(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult AddDetail(TaxOtherInvestmentViewModel model)
        {
            model.ErrMsg = string.Empty;

            var detailModel = new TaxOtherInvestmentDetailViewModel();
            detailModel.EmpId = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId).EmpID;
            detailModel.EmployeeName = model.EmployeeName;
            detailModel.EmployeeId = model.EmployeeId;
            detailModel.IncomeYear = model.IncomeYear;
            detailModel.AssessmentYear = model.AssessmentYear;
            detailModel.EntityEntryDate = DateTime.Now;
            detailModel.EntityId = model.Id;
            GetInvestmentType(detailModel);
            detailModel.strMode = "Create";

            ModelState.Clear();
            return View("_CreateOrEditDetail", detailModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult Create(TaxOtherInvestmentDetailViewModel detailmodel)
        {
            var model = new TaxOtherInvestmentViewModel();

            if (ModelState.IsValid)
            {
                var IUser = User.Identity.Name;
                var IDate = DateTime.Now;

                try
                {
                    var master = new PGM_TaxOtherInvestAndAdvPaid();
                    if (detailmodel.EntityId != 0)
                    {
                        master = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetByID(detailmodel.EntityId);
                    }
                    else
                    {
                        master.IUser = IUser;
                        master.IDate = IDate;
                    }
                    master.EntityType = Convert.ToByte(PGMEnum.TaxEntityType.OtherInvestment);
                    master.IncomeYear = detailmodel.IncomeYear;
                    master.AssessmentYear = detailmodel.AssessmentYear;
                    master.EmployeeId = detailmodel.EmployeeId;

                    model = master.ToInvestmentModel();
                    model.strMode = detailmodel.strMode;
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(detailmodel);
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        // Add Detail in master
                        detailmodel.IUser = IUser;
                        detailmodel.IDate = IDate;

                        var otherInvestment = detailmodel.ToInvestmentEntity();
                        master.PGM_TaxOtherInvestAndAdvPaidDetail.Add(otherInvestment);

                        // Save master
                        if (master.Id != 0)
                        {
                            _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.Update(master);
                        }
                        else
                        {
                            _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.Add(master);
                        }
                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.SaveChanges();


                        // Save Document
                        PGM_Document newDoc;
                        PGM_EntityDocument entityDoc;

                        foreach (var documentItem in detailmodel.OtherInvestmentDocumentList)
                        {
                            newDoc = new PGM_Document();
                            newDoc = documentItem.ToEntity();
                            _pgmCommonService.PGMUnit.DocumentRepository.Add(newDoc);
                            _pgmCommonService.PGMUnit.DocumentRepository.SaveChanges();

                            entityDoc = new PGM_EntityDocument
                            {
                                DocumentId = newDoc.Id,
                                EntityId = otherInvestment.Id,
                                IUser = IUser,
                                IDate = IDate
                            };
                            _pgmCommonService.PGMUnit.EntityDocumentRepository.Add(entityDoc);
                            _pgmCommonService.PGMUnit.EntityDocumentRepository.SaveChanges();

                        }

                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        return RedirectToAction("Edit", new { id = master.Id, type = "Edit" });
                    }
                    else
                    {
                        detailmodel.IsError = 1;
                        detailmodel.ErrMsg = checkoutBusinessLogic;
                        GetInvestmentType(detailmodel);
                        return View("_CreateOrEditDetail", detailmodel);
                    }
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsSuccessful = false;
            }

            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public JsonResult SaveMaster(TaxOtherInvestmentViewModel model)
        {
            if (model.Id != 0)
            {
                try
                {
                    var master = new PGM_TaxOtherInvestAndAdvPaid();
                    master = model.ToInvestmentEntity();
                    master.EUser = User.Identity.Name;
                    master.EntityType = Convert.ToByte(PGMEnum.TaxEntityType.OtherInvestment);
                    master.EDate = DateTime.Now;

                    _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.Update(master);
                    _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.SaveChanges();

                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
            }

            ModelState.Clear();
            //return Json(new { errClss = model.errClass, errMsg = model.ErrMsg }, JsonRequestBehavior.AllowGet);
            return Json(new
            {
                ErrClss = model.errClass,
                ErrMsg = model.ErrMsg
            });
        }

        public ActionResult Edit(int id, string type)
        {
            var master = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetByID(id);

            var model = master.ToInvestmentModel();
            model.strMode = "Edit";

            if (master.PGM_TaxOtherInvestAndAdvPaidDetail != null)
            {
                model.OtherInvestmentDetailList = new List<TaxOtherInvestmentDetailViewModel>();

                foreach (var item in master.PGM_TaxOtherInvestAndAdvPaidDetail)
                {
                    var detail = new TaxOtherInvestmentDetailViewModel();
                    detail.Id = item.Id;
                    detail.EntityId = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetAll().Where(d => d.EntityId == model.Id).Select(s => Convert.ToInt32(s.EntityId)).FirstOrDefault();
                    detail.EntityEntryDate = Convert.ToDateTime(item.EntityEntryDate);
                    detail.EntityDescription = item.EntityDescription;
                    detail.EntityAmount = Convert.ToDecimal(item.EntityAmount);
                    detail.InvestmentTypeId = Convert.ToInt32(item.InvestmentTypeId);
                    model.OtherInvestmentDetailList.Add(detail);
                }
            }

            var employee = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            if (employee != null)
            {
                model.EmpId = employee.EmpID;
                model.EmployeeId = employee.Id;
                model.EmployeeName = employee.FullName;
            }

            if (type == "success")
            {
                model.IsError = 0;
                model.ErrMsg = ErrorMessages.UpdateSuccessful;
            }
            PopulateList(model);
            return View("Edit", model);
        }

        public ActionResult EditDetail(int id)
        {
            var detailModel = new TaxOtherInvestmentDetailViewModel();


            var detailEntity = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetByID(id);
            detailModel = detailEntity.ToInvestmentModel();
            detailModel.ErrMsg = string.Empty;

            var taxPaidMaster = (from master in _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetAll()
                                 join emp in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList() on master.EmployeeId equals emp.Id

                                 where master.Id == detailEntity.EntityId
                                    && master.EntityType == Convert.ToByte(PGMEnum.TaxEntityType.OtherInvestment)

                                 select new TaxOtherInvestmentViewModel()
                                 {
                                     Id = master.Id,
                                     EmployeeId = emp.Id,
                                     IncomeYear = master.IncomeYear,
                                     AssessmentYear = master.AssessmentYear,
                                     EmpId = emp.EmpID,
                                     EmployeeName = emp.FullName
                                 }).ToList();

            if (taxPaidMaster != null)
            {
                var otherInvestment = taxPaidMaster.FirstOrDefault();
                detailModel.EntityId = otherInvestment.Id;
                detailModel.IncomeYear = otherInvestment.IncomeYear;
                detailModel.AssessmentYear = otherInvestment.AssessmentYear;
                detailModel.EmployeeId = otherInvestment.EmployeeId;
                detailModel.EmpId = otherInvestment.EmpId;
                detailModel.EmployeeName = otherInvestment.EmployeeName;
            }

            // Get documents -----------
            var relatedDocs = _pgmCommonService.PGMUnit.EntityDocumentRepository.GetAll()
                                    .Where(m => m.EntityId == detailModel.Id).ToList();

            var docList = _pgmCommonService.PGMUnit.DocumentRepository.GetAll()
                                    .Where(d => d.DocumentTypeId == Convert.ToInt32(PGMEnum.DocumentType.TaxInvestment)
                                            && relatedDocs.Any(x => x.DocumentId == d.Id));

            DocumentViewModel doc;
            foreach (var item in docList)
            {
                doc = new DocumentViewModel();

                doc.DetailId = detailModel.Id;  // Entity Id
                doc.Id = item.Id;               // Document Id

                doc.DocumentName = item.DocumentName;
                doc.DocumentTypeId = item.DocumentTypeId;
                doc.DocumentExtension = item.DocumentExtension;
                doc.DocumentUploadDate = item.DocumentUploadDate;
                doc.Attachment = item.Attachment;

                detailModel.OtherInvestmentDocumentList.Add(doc);
            }

            // -----
            GetInvestmentType(detailModel);
            detailModel.strMode = "EditDetail";
            return View("_CreateOrEditDetail", detailModel);
        }

        [HttpPost]
        public ActionResult EditDetail(TaxOtherInvestmentDetailViewModel detailmodel)
        {
            var masterModel = new TaxOtherInvestmentViewModel();
            masterModel.Id = detailmodel.EntityId;


            if (ModelState.IsValid)
            {
                try
                {
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(detailmodel);

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        var EUser = User.Identity.Name;
                        var EDate = DateTime.Now;

                        // Master
                        var masterEntity = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetByID(detailmodel.EntityId);
                        masterEntity.IncomeYear = detailmodel.IncomeYear;
                        masterEntity.AssessmentYear = detailmodel.AssessmentYear;
                        masterEntity.EntityType = Convert.ToByte(PGMEnum.TaxEntityType.OtherInvestment);
                        masterEntity.EUser = User.Identity.Name;
                        masterEntity.EDate = DateTime.Now;

                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.Update(masterEntity);
                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.SaveChanges();

                        // Detail
                        var detailEntity = detailmodel.ToInvestmentEntity();
                        detailEntity.EntityId = masterEntity.Id;
                        detailEntity.EUser = EUser;
                        detailEntity.EDate = EDate;

                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.Update(detailEntity);
                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.SaveChanges();

                        // Document
                        PGM_Document newDoc;
                        PGM_EntityDocument entityDoc;

                        foreach (var documentItem in detailmodel.OtherInvestmentDocumentList.Where(d => d.Id == 0))
                        {
                            if (documentItem.Id == 0) // New Document
                            {
                                newDoc = new PGM_Document();
                                newDoc = documentItem.ToEntity();
                                _pgmCommonService.PGMUnit.DocumentRepository.Add(newDoc);
                                _pgmCommonService.PGMUnit.DocumentRepository.SaveChanges();

                                entityDoc = new PGM_EntityDocument
                                {
                                    DocumentId = newDoc.Id,
                                    EntityId = detailEntity.Id,
                                    IUser = EUser,
                                    IDate = EDate
                                };
                                _pgmCommonService.PGMUnit.EntityDocumentRepository.Add(entityDoc);
                                _pgmCommonService.PGMUnit.EntityDocumentRepository.SaveChanges();
                            }
                        }

                        masterModel.IsError = 0;
                        masterModel.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                        masterModel = masterEntity.ToInvestmentModel();
                        PopulateList(masterModel);
                        var detailToReturn =
                            _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetAll()
                            .Where(md => md.EntityId == masterModel.Id)
                            .ToList();
                        foreach (var item in detailToReturn)
                        {
                            masterModel.OtherInvestmentDetailList.Add(item.ToInvestmentModel());
                        }
                        masterModel.strMode = "Edit";
                        return View("Edit", masterModel);
                    }
                    else
                    {
                        detailmodel.IsError = 1;
                        detailmodel.ErrMsg = checkoutBusinessLogic;
                        return View("_CreateOrEditDetail", detailmodel);
                    }
                }
                catch (Exception ex)
                {
                    masterModel.IsError = 1;
                    masterModel.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                masterModel.IsSuccessful = false;
            }
            masterModel.strMode = "Edit";
            return View("Edit", masterModel);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                int entityId = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetAll().Where(d => d.EntityId == id).Select(s => s.Id).FirstOrDefault();
                if (entityId > 0)
                {
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.Delete(x => x.EntityId == entityId);
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.SaveChanges();
                    int docId = _pgmCommonService.PGMUnit.EntityDocumentRepository.GetAll().Where(e => e.EntityId == entityId).Select(s => s.DocumentId).FirstOrDefault();
                    if (docId > 0)
                    {
                        _pgmCommonService.PGMUnit.DocumentRepository.Delete(x => x.Id == docId);
                        _pgmCommonService.PGMUnit.DocumentRepository.SaveChanges();
                    }

                }
                _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.Delete(x => x.Id == entityId);
                _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.SaveChanges();
                _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.Delete(x => x.Id == id);
                _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.SaveChanges();

                result = true;
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                result = false;
                errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }
        [NoCache]
        [HttpPost]
        public JsonResult DeleteDocument(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Id cannot be empty."
                });
            }

            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                int docId = Convert.ToInt32(id.Split(',')[0]);
                int detailId = Convert.ToInt32(id.Split(',')[1]);
                if (docId == 0)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = "This doucment does not save yet."
                    });
                }


                var entityDoc =
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.GetAll()
                        .FirstOrDefault(d => d.DocumentId == docId && d.EntityId == detailId);

                if (entityDoc != null)
                {
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.Delete(entityDoc);
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.SaveChanges();

                    _pgmCommonService.PGMUnit.DocumentRepository.Delete(docId);
                    _pgmCommonService.PGMUnit.DocumentRepository.SaveChanges();

                    result = true;
                }
                else
                {
                    result = false;
                    errMsg = "Document not found in entity-document table!";
                }

            }
            catch (UpdateException ex)
            {
                try
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                    ModelState.AddModelError("Error", errMsg);
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            catch
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
        [HttpPost]
        public JsonResult DeleteDetail(int id)
        {

            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {

                var entityDoc =
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.GetAll()
                        .FirstOrDefault(d => d.DocumentId == id);
                if (entityDoc != null)
                {
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.Delete(entityDoc);
                    _pgmCommonService.PGMUnit.EntityDocumentRepository.SaveChanges();

                    _pgmCommonService.PGMUnit.DocumentRepository.Delete(id);
                    _pgmCommonService.PGMUnit.DocumentRepository.SaveChanges();

                    result = true;
                }
                else
                {
                    result = false;
                    errMsg = "Detail not found for specified id.";
                }

            }
            catch (Exception ex)
            {
                try
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                    ModelState.AddModelError("Error", errMsg);
                    result = false;
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion

        #region Other

        public string CheckingBusinessLogicValidation(TaxOtherInvestmentDetailViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                if (model.strMode == "Create" || model.strMode == "AddDetail")
                {
                    var master = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetAll()
                                                          .Where(t => t.EntityType == Convert.ToInt32(PGMEnum.TaxEntityType.OtherInvestment)
                                                          && t.EmployeeId == model.EmployeeId
                                                          && t.IncomeYear == model.IncomeYear).ToList();
                    if (master.Count > 0)
                    {
                        message = "Duplicate Entry.";
                    }
                }
                if (model.strMode == "EditDetail" || model.strMode == "Edit")
                {
                    var master = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetAll()
                                                          .Where(t => t.EntityType == Convert.ToInt32(PGMEnum.TaxEntityType.OtherInvestment)
                                                          && t.EmployeeId == model.EmployeeId
                                                          && t.IncomeYear == model.IncomeYear && t.Id != model.EntityId).ToList();
                    if (master.Count > 0)
                    {
                        message = "Duplicate Entry.";
                    }
                }
                if (model.EntityEntryDate != null)
                {
                    int year = model.EntityEntryDate.Value.Year;
                    int month = model.EntityEntryDate.Value.Month;
                    int incomeYear = Convert.ToInt32(model.IncomeYear.Substring(0, 4));
                    if (year >= incomeYear && year <= (incomeYear + 1))
                    {
                        if (year == incomeYear)
                        {
                            if (month >= 7 && month <= 12)
                            {
                                message = "";
                            }
                            else
                            {
                                message = "Payment month should be in the selected Income Year.";
                            }
                        }
                        else if (year == (incomeYear + 1))
                        {
                            if (month >= 1 && month <= 6)
                            {
                                message = "";
                            }
                            else
                            {
                                message = "Payment month should be in the selected Income Year.";
                            }
                        }
                    }
                    else
                    {
                        message = "Payment Date should be in the selected income year.";
                    }
                }
            }

            return message;
        }
        public decimal GetTotalAmount(int id)
        {
            decimal totalPaid = 0;
            var detailList = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetAll().Where(x => x.EntityId == id).ToList();
            if (detailList.Count > 0)
            {
                foreach (var d in detailList)
                {
                    totalPaid += d.EntityAmount == null ? 0 : d.EntityAmount.Value;
                }
            }

            return Math.Round(totalPaid, 2);
        }
        public void PopulateList(TaxOtherInvestmentViewModel model)
        {
            model.IncomeYearList = Common.PopulateIncomeYearList();
        }

        public void GetInvestmentType(TaxOtherInvestmentDetailViewModel model)
        {
            model.InvestmentTypeList = _pgmCommonService.PGMUnit.OtherInvestmentTypeRepository.GetAll().ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.InvestmentTypeName,
                    Value = y.Id.ToString()
                }).ToList();
        }
        private PGM_Document ToAttachFile(PGM_Document docDetail, HttpFileCollectionBase files)
        {
            foreach (string fileTagName in files)
            {
                HttpPostedFileBase file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.

                    int size = file.ContentLength;
                    int kb = size / 1024;
                    docDetail.DocumentSizeInMB = kb / 1024;
                    docDetail.DocumentName = file.FileName;
                    int position = docDetail.DocumentName.LastIndexOf("\\");
                    docDetail.DocumentExtension = file.ContentType;
                    byte[] fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                    docDetail.Attachment = fileData;
                    docDetail.DocumentUploadDate = DateTime.Now;
                    docDetail.DocumentTypeId = Convert.ToInt32(PGMEnum.DocumentType.TaxInvestment);
                }
            }

            return docDetail;
        }
        [HttpPost]
        public ActionResult AddAttachment([Bind(Exclude = "Attachment")] TaxOtherInvestmentDetailViewModel detail)
        {
            HttpFileCollectionBase files = Request.Files;
            string name = string.Empty;
            DocumentViewModel doc;
            HttpPostedFileBase file;
            byte[] fileData = null;
            int size;
            int kb;
            int position;
            if (detail.OtherInvestmentDocumentList == null)
                detail.OtherInvestmentDocumentList = new List<DocumentViewModel>();

            foreach (string fileTagName in files)
            {
                doc = new DocumentViewModel();
                file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.
                    size = file.ContentLength;
                    kb = size / 1024;
                    doc.DocumentSizeInMB = kb / 1024;
                    doc.DocumentName = file.FileName;
                    position = doc.DocumentName.LastIndexOf("\\");
                    doc.DocumentExtension = file.ContentType;
                    fileData = new byte[size];
                    file.InputStream.Read(fileData, 0, size);
                    doc.Attachment = fileData;
                    doc.DocumentUploadDate = DateTime.Now;
                    doc.DocumentTypeId = Convert.ToInt32(PGMEnum.DocumentType.TaxInvestment);
                }
                detail.OtherInvestmentDocumentList.Add(doc);
            }

            return PartialView("_Details", detail);
        }


        [NoCache]
        public JsonResult GetEmployeeInfo(TaxOtherInvestmentViewModel model)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                    if (emp != null)
                    {
                        return Json(new
                        {
                            EmpId = emp.EmpID + " - " + emp.FullName,
                            EmployeeId = emp.Id,
                            EmployeeName = emp.FullName

                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    return Json(new { Result = false });
                }
            }
            else
            {
                return Json(new
                {
                    Result = msg
                });
            }
        }

        [NoCache]
        public JsonResult GetAssessmentYear(string incomeYear)
        {
            incomeYear = incomeYear.Substring(0, 4);
            int year = Convert.ToInt32(incomeYear);
            string assessmentYear = (year + 1) + "-" + (year + 2);

            return Json(new
            {
                assessmentYear = assessmentYear
            });
        }
        [NoCache]
        public ActionResult CreateInvestmentType()
        {
            InvestmentTypeViewModel model = new InvestmentTypeViewModel();
            return View("CreateInvestmentType", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public JsonResult CreateInvestmentType(InvestmentTypeViewModel model)
        {
            bool success = false;
            PGM_TaxOtherInvestmentType master = new PGM_TaxOtherInvestmentType();
            master = model.ToEntity();
            if (ModelState.IsValid)
            {
                try
                {
                    _pgmCommonService.PGMUnit.OtherInvestmentTypeRepository.Add(master);
                    _pgmCommonService.PGMUnit.OtherInvestmentTypeRepository.SaveChanges();

                    success = true;
                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch (Exception ex)
                {
                    success = false;
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }

            return Json(new
            {
                Success = success,
                Message = model.ErrMsg
            });
        }

        public ActionResult GetIncomeYearList()
        {
            return PartialView("Select", Common.PopulateIncomeYearList());
        }

        #endregion
    }
}