using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.Document;
using PGM.Web.Areas.PGM.Models.TaxAdvancedPaid;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class AdvancedPaidTaxController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private List<vwPGMEmploymentInfo> _emps;
        #endregion

        #region Constructor

        public AdvancedPaidTaxController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region GetList Actions
        public ActionResult Index(AdvancedTaxPaidViewModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, AdvancedTaxPaidViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            this._emps = _pgmCommonService
                .PGMUnit
                .FunctionRepository
                .GetEmployeeList()
                .Where(e => e.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId)
                .ToList();

            List<AdvancedTaxPaidViewModel> list = (from TP in _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetAll()
                                                   join Emp in _emps on TP.EmployeeId equals Emp.Id

                                                   where TP.EntityType == Convert.ToByte(PGMEnum.TaxEntityType.AdvanceTaxPaid)
                                                   select new AdvancedTaxPaidViewModel()
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
                decimal totalPaid = GetTotalPaid(d.Id);
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.EmployeeId,
                    d.IncomeYear,
                    d.AssessmentYear,
                    d.EmpId,
                    d.EmployeeName,
                    totalPaid,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }
        #endregion

        #region Create Actions
        public ActionResult Create()
        {

            AdvancedTaxPaidViewModel model = new AdvancedTaxPaidViewModel();
            model.strMode = "Create";
            PopulateList(model);
            return View(model);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddDetail(AdvancedTaxPaidViewModel model)
        {
            model.ErrMsg = string.Empty;
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

            AdvancedTaxPaidDetailViewModel detailModel = new AdvancedTaxPaidDetailViewModel()
            {
                EmpId = emp.EmpID,
                EmployeeName = model.EmployeeName,
                EmployeeId = model.EmployeeId,
                IncomeYear = model.IncomeYear,
                AssessmentYear = model.AssessmentYear,
                EntityEntryDate = DateTime.Now,
                EitityId = model.Id
            };

            detailModel.strMode = "Create";

            return View("_CreateOrEditDetail", detailModel);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(AdvancedTaxPaidDetailViewModel detailmodel)
        {
            AdvancedTaxPaidViewModel model = new AdvancedTaxPaidViewModel();

            if (ModelState.IsValid)
            {
                var IUser = User.Identity.Name;
                var IDate = DateTime.Now;

                try
                {
                    PGM_TaxOtherInvestAndAdvPaid master = new PGM_TaxOtherInvestAndAdvPaid();
                    if (detailmodel.EitityId != 0)
                    {
                        master = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetByID(detailmodel.EitityId);
                    }
                    else
                    {
                        master.IUser = IUser;
                        master.IDate = IDate;
                    }
                    master.EntityType = Convert.ToByte(PGMEnum.TaxEntityType.AdvanceTaxPaid);
                    master.IncomeYear = detailmodel.IncomeYear;
                    master.AssessmentYear = detailmodel.AssessmentYear;
                    master.EmployeeId = detailmodel.EmployeeId;

                    model = master.ToModel();
                    model.strMode = detailmodel.strMode;
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(detailmodel);
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        // Add Detail in master
                        detailmodel.IUser = IUser;
                        detailmodel.IDate = IDate;

                        PGM_TaxOtherInvestAndAdvPaidDetail advancePaid = detailmodel.ToEntity();
                        master.PGM_TaxOtherInvestAndAdvPaidDetail.Add(advancePaid);

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

                        foreach (var documentItem in detailmodel.AdvanceDetailDocumentList)
                        {
                            newDoc = new PGM_Document();
                            newDoc = documentItem.ToEntity();
                            _pgmCommonService.PGMUnit.DocumentRepository.Add(newDoc);
                            _pgmCommonService.PGMUnit.DocumentRepository.SaveChanges();

                            entityDoc = new PGM_EntityDocument
                            {
                                DocumentId = newDoc.Id,
                                EntityId = advancePaid.Id,
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
        #endregion

        #region Edit Actions
        [HttpGet]
        public ActionResult Edit(int id, string type)
        {
            var masterEntity = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetByID(id);

            var model = masterEntity.ToModel();
            model.strMode = "Edit";

            if (masterEntity.PGM_TaxOtherInvestAndAdvPaidDetail != null)
            {
                model.AdvanceDetailList = new List<AdvancedTaxPaidDetailViewModel>();
                foreach (var item in masterEntity.PGM_TaxOtherInvestAndAdvPaidDetail)
                {
                    model.AdvanceDetailList.Add(item.ToModel());
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

        [HttpPost]
        [NoCache]
        public JsonResult SaveMaster(AdvancedTaxPaidViewModel model)
        {
            if (model.Id != 0)
            {
                try
                {
                    var master = new PGM_TaxOtherInvestAndAdvPaid();

                    master = model.ToEntity();
                    //master.EntityType = Convert.ToByte(PGMEnum.DocumentType.TaxInvestment);
                    master.EUser = User.Identity.Name;
                    master.EntityType = Convert.ToByte(PGMEnum.TaxEntityType.AdvanceTaxPaid);
                    master.EDate = DateTime.Now;

                    _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.Update(master);
                    _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.SaveChanges();

                    model.IsError = 0;
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
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
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
            }

            ModelState.Clear();

            return Json(new
            {
                ErrClss = model.errClass,
                ErrMsg = model.ErrMsg
            });
        }

        [HttpGet]
        public ActionResult EditDetail(int id)
        {

            var detailModel = new AdvancedTaxPaidDetailViewModel();

            var detailEntity = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetByID(id);
            detailModel = detailEntity.ToModel();
            detailModel.ErrMsg = string.Empty;

            this._emps = _pgmCommonService
                .PGMUnit
                .FunctionRepository
                .GetEmployeeList()
                .Where(e => e.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId)
                .ToList();

            var taxPaidMaster = (from master in _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetAll()
                                 join emp in _emps on master.EmployeeId equals emp.Id

                                 where master.Id == detailEntity.EntityId
                                    && master.EntityType == Convert.ToByte(PGMEnum.TaxEntityType.AdvanceTaxPaid)

                                 select new AdvancedTaxPaidViewModel()
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
                AdvancedTaxPaidViewModel advancedTaxPaid = taxPaidMaster.FirstOrDefault();
                detailModel.EitityId = advancedTaxPaid.Id;
                detailModel.IncomeYear = advancedTaxPaid.IncomeYear;
                detailModel.AssessmentYear = advancedTaxPaid.AssessmentYear;
                detailModel.EmployeeId = advancedTaxPaid.EmployeeId;
                detailModel.EmpId = advancedTaxPaid.EmpId;
                detailModel.EmployeeName = advancedTaxPaid.EmployeeName;
            }

            // Get documents -----------
            var relatedDocs = _pgmCommonService.PGMUnit.EntityDocumentRepository.GetAll()
                                    .Where(m => m.EntityId == detailModel.Id).ToList();

            var docList = _pgmCommonService.PGMUnit.DocumentRepository.GetAll()
                                    .Where(d => d.DocumentTypeId == Convert.ToInt32(PGMEnum.DocumentType.TaxAdvancePaid)
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

                detailModel.AdvanceDetailDocumentList.Add(doc);
            }
            // -----

            detailModel.strMode = "EditDetail";
            return View("_CreateOrEditDetail", detailModel);
        }

        [HttpPost]
        public ActionResult EditDetail(AdvancedTaxPaidDetailViewModel detailmodel)
        {
            var masterModel = new AdvancedTaxPaidViewModel();
            masterModel.Id = detailmodel.EitityId;


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
                        var masterEntity = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetByID(detailmodel.EitityId);
                        masterEntity.IncomeYear = detailmodel.IncomeYear;
                        masterEntity.AssessmentYear = detailmodel.AssessmentYear;
                        masterEntity.EntityType = Convert.ToByte(PGMEnum.TaxEntityType.AdvanceTaxPaid);
                        masterEntity.EUser = User.Identity.Name;
                        masterEntity.EDate = DateTime.Now;

                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.Update(masterEntity);
                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.SaveChanges();

                        // Detail
                        var detailEntity = detailmodel.ToEntity();
                        detailEntity.EntityId = masterEntity.Id;
                        detailEntity.EUser = EUser;
                        detailEntity.EDate = EDate;

                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.Update(detailEntity);
                        _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.SaveChanges();

                        // Document
                        PGM_Document newDoc;
                        PGM_EntityDocument entityDoc;

                        foreach (var documentItem in detailmodel.AdvanceDetailDocumentList.Where(d => d.Id == 0))
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

                        masterModel = masterEntity.ToModel();
                        PopulateList(masterModel);
                        var detailToReturn =
                            _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetAll()
                            .Where(md => md.EntityId == masterModel.Id)
                            .ToList();
                        foreach (var item in detailToReturn)
                        {
                            masterModel.AdvanceDetailList.Add(item.ToModel());
                        }

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

            return View("Edit", masterModel);
        }
        #endregion

        #region Add new File Action
        [HttpPost]
        public ActionResult AddAttachment([Bind(Exclude = "Attachment")] AdvancedTaxPaidDetailViewModel detailModel)
        {
            HttpFileCollectionBase files = Request.Files;
            string name = string.Empty;
            byte[] fileData = null;

            DocumentViewModel docModel;
            HttpPostedFileBase file;
            int docSize, sizeInKB;

            foreach (string fileTagName in files)
            {
                docModel = new DocumentViewModel();

                file = Request.Files[fileTagName];
                if (file.ContentLength > 0)
                {
                    // it is important to assign id to 0 with new document
                    docModel.Id = 0;
                    docModel.DetailId = detailModel.Id;

                    // Due to the limit of the max for a int type, the largest file can be
                    // uploaded is 2147483647, which is very large anyway.

                    docSize = file.ContentLength;
                    sizeInKB = docSize / 1024;

                    docModel.DocumentSizeInMB = sizeInKB / 1024;
                    docModel.DocumentName = file.FileName;
                    docModel.DocumentExtension = file.ContentType;

                    fileData = new byte[docSize];
                    file.InputStream.Read(fileData, 0, docSize);
                    docModel.Attachment = fileData;
                    docModel.DocumentUploadDate = DateTime.Now;

                    docModel.DocumentTypeId = Convert.ToInt32(PGMEnum.DocumentType.TaxAdvancePaid);
                }
                detailModel.AdvanceDetailDocumentList.Add(docModel);
            }

            return PartialView("_Details", detailModel);
        }
        #endregion

        #region Delete Actions
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

                PGM_EntityDocument entityDoc =
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

        [NoCache]
        [HttpPost]
        public JsonResult DeleteDetail(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                PGM_TaxOtherInvestAndAdvPaidDetail detail =
                    _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.GetAll()
                        .FirstOrDefault(taxOD => taxOD.Id == id);

                if (detail != null)
                {
                    // Delete related document
                    IEnumerable<PGM_EntityDocument> documents =
                        _pgmCommonService.PGMUnit.EntityDocumentRepository.GetAll()
                            .Where(d => d.EntityId == detail.Id)
                            .ToList();

                    if (documents != null && documents.Any())
                    {
                        int docId = 0;
                        foreach (var doc in documents)
                        {
                            docId = doc.DocumentId;

                            _pgmCommonService.PGMUnit.EntityDocumentRepository.Delete(doc);
                            _pgmCommonService.PGMUnit.EntityDocumentRepository.SaveChanges();

                            _pgmCommonService.PGMUnit.DocumentRepository.Delete(docId);
                            _pgmCommonService.PGMUnit.DocumentRepository.SaveChanges();
                        }
                    }

                    // Delete Detail
                    _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.Delete(detail);
                    _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidDetailsRepository.SaveChanges();

                    result = true;
                }
                else
                {
                    return Json(new
                    {
                        Success = result,
                        Message = "No detail found for specified Id!"
                    });
                }
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
        #endregion

        #region Other Actions and Methos

        public string CheckingBusinessLogicValidation(AdvancedTaxPaidDetailViewModel model)
        {
            string message = string.Empty;

            if (model != null)
            {
                if (model.strMode == "Create" || model.strMode == "AddDetail")
                {
                    var master = _pgmCommonService.PGMUnit.TaxOtherInvestAndAdvPaidRepository.GetAll()
                                                          .Where(t => t.EntityType == Convert.ToInt32(PGMEnum.TaxEntityType.AdvanceTaxPaid)
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
                                                          .Where(t => t.EntityType == Convert.ToInt32(PGMEnum.TaxEntityType.AdvanceTaxPaid)
                                                          && t.EmployeeId == model.EmployeeId
                                                          && t.IncomeYear == model.IncomeYear && t.Id != model.EitityId).ToList();
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

        private void PopulateList(AdvancedTaxPaidViewModel model)
        {
            model.IncomeYearList = Common.PopulateIncomeYearList();
        }

        public decimal GetTotalPaid(int id)
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

        [NoCache]
        public JsonResult GetEmployeeInfo(AdvancedTaxPaidViewModel model)
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
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                    return Json(new { Result = false });
                }
            }
            else
            {
                return Json(new { Result = msg });
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
        public ActionResult GetIncomeYearList()
        {
            return PartialView("_Select", Common.PopulateIncomeYearList());
        }

        #endregion
    }
}