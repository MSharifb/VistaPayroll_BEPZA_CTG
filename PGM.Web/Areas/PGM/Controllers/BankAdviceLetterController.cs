using DAL.PGM;
using DAL.PGM.CustomEntities;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.BankAdviceLetter;
using PGM.Web.Areas.PGM.Models.BankAdviceLetterSubjectBody;
using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

/*
Revision History (RH):
		SL	    : 01
		Author	: AMN
		Date	: 2015-Jul-14
        SCR     : ERP_BEPZA_PGM_SCR.doc (SCR#38)
		Desc.	: Bank advice slip did not update correctly
		---------
*/

namespace PGM.Web.Areas.PGM.Controllers
{
    #region Revision History
    /*
        Revision History (RH):
		SL	    : 01
		Author	: Shirin
		Date	: 2015-May-03
        SCR     : ERP_BEPZA_PGM_SCR.doc (SCR#32)
		Desc	: Calculate Total amount brfore Add and Update
		---------
    */
    #endregion

    [NoCache]
    public class BankAdviceLetterController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private readonly PGMMonthlySalaryService _monthlySalaryService;
        private readonly PgmBonusService _pgmBonusService;
        private readonly PGMEntities _pgmContext;

        #endregion

        #region Ctor


        public BankAdviceLetterController(PGMCommonService pgmCommonservice, PGMMonthlySalaryService monthlySalaryService, PgmBonusService pgmBonusService, PGMEntities pgmContext)
        {
            this._pgmContext = pgmContext;
            this._pgmCommonService = pgmCommonservice;
            this._monthlySalaryService = monthlySalaryService;
            this._pgmBonusService = pgmBonusService;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }
        static string LSubject = string.Empty;

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, BankAdviceLetterSearchModel model)
        {

            string filterExpression = String.Empty;
            int totalRecords = 0;
            dynamic lst = null;

            if (model.LetterType == null)
            {
                model.LetterType = "Salary";
                var list = _pgmCommonService.GetBankAdviceLetterSearchList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false,
                     model.LetterType,
                     model.SalaryYear,
                     model.SalaryMonth,
                     model.ReferenceNo,
                     model.BankName).Where(e => e.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();

                #region Sorting
                if (request.SortingName == "LetterType")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.LetterType).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.LetterType).ToList();
                    }
                }

                if (request.SortingName == "SelectedLetterType")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.SelectedLetterType).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.SelectedLetterType).ToList();
                    }
                }

                if (request.SortingName == "SalaryYear")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.SalaryYear).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.SalaryYear).ToList();
                    }
                }
                if (request.SortingName == "SalaryMonth")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.SalaryMonth).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.SalaryMonth).ToList();
                    }
                }

                if (request.SortingName == "ReferenceNo")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.ReferenceNo).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.ReferenceNo).ToList();
                    }
                }

                //if (request.SortingName == "LimitAmount")
                //{
                //    if (request.SortingOrder.ToString().ToLower() == "asc")
                //    {
                //        list = list.OrderBy(x => x.LimitAmount).ToList();
                //    }
                //    else
                //    {
                //        list = list.OrderByDescending(x => x.LimitAmount).ToList();
                //    }
                //}

                if (request.SortingName == "AccountNo")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.AccountNo).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.AccountNo).ToList();
                    }
                }

                if (request.SortingName == "BankName")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.BankName).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.BankName).ToList();
                    }
                }

                #endregion
                lst = list;

            }
            else
            {

                var list = _pgmCommonService.GetBankAdviceLetterSearchList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false,
                       model.LetterType, model.SalaryYear, model.SalaryMonth, model.ReferenceNo, model.BankName).Where(e => e.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();

                #region Sorting
                if (request.SortingName == "LetterType")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.LetterType).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.LetterType).ToList();
                    }
                }

                if (request.SortingName == "SelectedLetterType")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.SelectedLetterType).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.SelectedLetterType).ToList();
                    }
                }

                if (request.SortingName == "SalaryYear")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.SalaryYear).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.SalaryYear).ToList();
                    }
                }
                if (request.SortingName == "SalaryMonth")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.SalaryMonth).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.SalaryMonth).ToList();
                    }
                }

                if (request.SortingName == "ReferenceNo")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.ReferenceNo).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.ReferenceNo).ToList();
                    }
                }

                //if (request.SortingName == "LimitAmount")
                //{
                //    if (request.SortingOrder.ToString().ToLower() == "asc")
                //    {
                //        list = list.OrderBy(x => x.LimitAmount).ToList();
                //    }
                //    else
                //    {
                //        list = list.OrderByDescending(x => x.LimitAmount).ToList();
                //    }
                //}

                if (request.SortingName == "AccountNo")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.AccountNo).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.AccountNo).ToList();
                    }
                }

                if (request.SortingName == "BankName")
                {
                    if (request.SortingOrder.ToString().ToLower() == "asc")
                    {
                        list = list.OrderBy(x => x.BankName).ToList();
                    }
                    else
                    {
                        list = list.OrderByDescending(x => x.BankName).ToList();
                    }
                }

                #endregion
                lst = list;
            }


            totalRecords = _pgmCommonService.GetBankAdviceLetterSearchList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true,
                    model.LetterType, model.SalaryYear, model.SalaryMonth, model.ReferenceNo, model.BankName).Count();



            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };


            foreach (var d in lst)
            {

                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.LetterType,
                    "emptyName",
                    d.SalaryYear,
                    d.SalaryMonth,
                    d.ReferenceNo,
                    d.TotalAmount,
                    d.AccountNo,
                    d.BankName,
                    "GenerateLetter",
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Index(BankAdviceLetterViewModel model)
        {
            return View(model);
        }

        [NoCache]
        public RedirectResult GenerateBankAdviceLetter(string id)
        {
            return Redirect("~/Reports/PGM/viewers/BankAdviceLetterCommonView.aspx?id=" + id);
        }

        [NoCache]
        public ActionResult Create(string letterType)
        {
            BankAdviceLetterViewModel model = new BankAdviceLetterViewModel();


            model.SelectedLetterType = "Salary";
            model.DateofLetter = Common.CurrentDateTime;
            PopulateDropdown(model);

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(BankAdviceLetterViewModel model)
        {
            BankAdviceLetterBodyViewModel LetterTemplateBody = new BankAdviceLetterBodyViewModel();
            string errorMessage = string.Empty;
            model.IsError = 1;
            model.ZoneInfoId = LoggedUserZoneInfoId;
            //model.LetterTypeB = _pgmCommonservice.PGMUnit.Lett
            model.LetterType = model.SelectedLetterType;

            var letterTemplate = (from t in _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.GetAll()
                                  where t.LetterType == model.LetterType && t.BankId == model.BankId && t.ZoneInfoId == LoggedUserZoneInfoId
                                  select t).FirstOrDefault();

            if (model.LetterType == "Salary")
            {
                model.LetterTypeB = "বেতন ভাতা";
            }
            else if (model.LetterType == "Bonus")
            {
                var bonusInfo = _pgmCommonService.PGMUnit.BonusTypeRepository.GetAll().Where(x => x.Id == model.BonusTypeId).ToList();
                foreach (var bonus in bonusInfo)
                {
                    model.BonusType = bonus.BonusType;
                    model.LetterTypeB = bonus.BonusTypeB;
                }
            }

            errorMessage = GetBusinessLogicValidation(model);

            if (letterTemplate != null)
            {
                LetterTemplateBody.LetterSubject = GetReplacedTemplateVariableValue(letterTemplate.LetterSubject, model);
                LetterTemplateBody.Body1 = GetReplacedTemplateVariableValue(letterTemplate.Body1, model);
                LetterTemplateBody.Body1.PadLeft(8);
                if (LetterTemplateBody.Body2 != null)
                {
                    LetterTemplateBody.Body2 = GetReplacedTemplateVariableValue(letterTemplate.Body2, model);
                    LetterTemplateBody.Body2.PadLeft(8);
                }
                LetterTemplateBody.Enclosure = GetReplacedTemplateVariableValue(letterTemplate.Enclosure, model);
                LetterTemplateBody.Adressee = letterTemplate.Addressee;
                LetterTemplateBody.Salutation = letterTemplate.Salutation;
                LetterTemplateBody.Signatory1Id = letterTemplate.Signatory1Id;
                LetterTemplateBody.Signatory1Designation = letterTemplate.Signatory1Designation;
                LetterTemplateBody.Signatory2Id = letterTemplate.Signatory2Id;
                LetterTemplateBody.Signatory2Designation = letterTemplate.Signatory2Designation;
                LetterTemplateBody.Complementary = letterTemplate.Complementary;

                var pgm_LetterTemplateBody = LetterTemplateBody.ToEntity();

                #region Get Bank Letter Details info
                var dtllist = _monthlySalaryService.GetSalaryDetailInfoList(""
                                                                , "ID"
                                                                , "Asc"
                                                                , 0
                                                                , 20
                                                                , 1
                                                                , false
                                                                , String.Empty
                                                                , model.SalaryYear
                                                                , model.SalaryMonth
                                                                , 0
                                                                , 0
                                                                , null
                                                                , false
                                                                , LoggedUserZoneInfoId)
                                                     .ToList();
                #endregion

                BankAdviceLetterDetailModel bankAdLetterDtlModel = new BankAdviceLetterDetailModel();


                if (string.IsNullOrEmpty(errorMessage))
                {
                    var pgm_banlAdviceLetter = model.ToEntity();
                    pgm_banlAdviceLetter.IUser = User.Identity.Name;
                    pgm_banlAdviceLetter.IDate = Common.CurrentDateTime;
                    pgm_banlAdviceLetter.ZoneInfoId = LoggedUserZoneInfoId;

                    foreach (var item in dtllist)
                    {
                        bankAdLetterDtlModel.BankLetterId = pgm_banlAdviceLetter.Id;
                        bankAdLetterDtlModel.EmployeeId = item.EmployeeId;
                        bankAdLetterDtlModel.AccountNo = item.AccountNo;
                        bankAdLetterDtlModel.NetPayable = item.NetPay;

                        var pgm_banlAdviceLetterDetails = bankAdLetterDtlModel.ToEntity();
                        _pgmCommonService.PGMUnit.BankAdviceLetterDetails.Add(pgm_banlAdviceLetterDetails);
                    }

                    try
                    {
                        _pgmCommonService.PGMUnit.BankAdviceLetters.Add(pgm_banlAdviceLetter);
                        _pgmCommonService.PGMUnit.BankAdviceLetterBodyTextRepository.Add(pgm_LetterTemplateBody);

                        _pgmCommonService.PGMUnit.BankAdviceLetters.SaveChanges();

                        model.IsError = 0;
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    catch (Exception ex)
                    {
                        PopulateDropdown(model);

                        model.IsError = 1;
                        model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    }
                }
                else
                {
                    model.IsError = 1;
                    model.ErrMsg = errorMessage;
                }
            }
            else
            {
                model.ErrMsg = "Must be setup Bank Advice Letter Template of Letter type first.Then try.";
            }

            PopulateDropdown(model);

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(Int64 id)
        {
            var entity = _pgmCommonService.PGMUnit.BankAdviceLetters.GetByID(id);
            var model = entity.ToModel();
            model.SelectedLetterType = entity.LetterType;
            model.LetterType = entity.LetterType;
            model.TotalAmount = entity.TotalAmount;
            model.AccountNo = entity.AccountNo;
            model.BankId = entity.BankId;
            model.BranchId = Convert.ToInt32(entity.BranchId != null ? entity.BranchId : 0);
            model.ReferenceNo = entity.ReferenceNo;
            model.DateofLetter = entity.DateofLetter;
            if (model.SelectedLetterType == "Bonus")
            {
                model.BonusType = entity.BonusType;
                model.BonusTypeId = _pgmCommonService.PGMUnit.BonusTypeRepository.GetAll().Where(b => b.BonusType == model.BonusType).Select(s => s.Id).FirstOrDefault();
            }

            PrepareModelEdit(model);

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(BankAdviceLetterViewModel model)
        {
            var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { x.Key, x.Value.Errors })
                        .ToArray();
            BankAdviceLetterBodyViewModel LetterTemplateBody = new BankAdviceLetterBodyViewModel();
            model.LetterType = model.SelectedLetterType;
            /*RH#01*/
            model.ErrMsg = GetBusinessLogicValidation(model);
            /*End RH#01*/

            if (ModelState.IsValid && string.IsNullOrEmpty(model.ErrMsg))
            {
                model.IsError = 1;
                model.LetterType = model.SelectedLetterType;

                var letterTemplate = (from t in _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.GetAll()
                                      where t.LetterType == model.LetterType && t.BankId == model.BankId && t.ZoneInfoId == LoggedUserZoneInfoId
                                      select t).FirstOrDefault();

                if (model.LetterType == "Salary")
                {
                    model.LetterTypeB = "বেতন ভাতা";
                }
                else if (model.LetterType == "Bonus")
                {
                    var bonusInfo = _pgmCommonService.PGMUnit.BonusTypeRepository.GetAll().Where(x => x.Id == model.BonusTypeId).ToList();
                    foreach (var bonus in bonusInfo)
                    {
                        model.BonusType = bonus.BonusType;
                        model.LetterTypeB = bonus.BonusTypeB;
                    }
                }
                if (letterTemplate != null)
                {
                    LetterTemplateBody.LetterSubject = GetReplacedTemplateVariableValue(letterTemplate.LetterSubject, model);
                    LetterTemplateBody.Body1 = GetReplacedTemplateVariableValue(letterTemplate.Body1, model);
                    if (letterTemplate.Body2 != null)
                    {
                        LetterTemplateBody.Body2 = GetReplacedTemplateVariableValue(letterTemplate.Body2, model);
                    }
                    LetterTemplateBody.Enclosure = GetReplacedTemplateVariableValue(letterTemplate.Enclosure, model);
                    LetterTemplateBody.Adressee = letterTemplate.Addressee;
                    LetterTemplateBody.Salutation = letterTemplate.Salutation;
                    LetterTemplateBody.Signatory1Id = letterTemplate.Signatory1Id;
                    LetterTemplateBody.Signatory1Designation = letterTemplate.Signatory1Designation;
                    LetterTemplateBody.Signatory2Id = letterTemplate.Signatory2Id;
                    LetterTemplateBody.Signatory2Designation = letterTemplate.Signatory2Designation;
                    LetterTemplateBody.Complementary = letterTemplate.Complementary;
                }
                var pgm_LetterTemplateBody = LetterTemplateBody.ToEntity();


                #region Get Bank Letter Details info
                var dtllist = _monthlySalaryService
                    .GetSalaryDetailInfoList(""
                            , "ID"
                            , "Asc"
                            , 0
                            , 20
                            , 1
                            , false
                            , String.Empty
                            , model.SalaryYear
                            , model.SalaryMonth
                            , 0
                            , 0
                            , null
                            , false
                            , LoggedUserZoneInfoId)
                    .ToList();
                #endregion

                List<BankAdviceLetterDetailModel> BankLetterDtlList = new List<BankAdviceLetterDetailModel>();
                try
                {
                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;
                    entity.ZoneInfoId = LoggedUserZoneInfoId;

                    foreach (var item in dtllist)
                    {
                        var list = new BankAdviceLetterDetailModel
                        {
                            BankLetterId = entity.Id,
                            EmployeeId = item.EmployeeId,
                            AccountNo = item.AccountNo,
                            NetPayable = item.NetPay
                        };
                        BankLetterDtlList.Add(list);
                    }

                    var navigationList = new Dictionary<Type, ArrayList>();

                    int del = _pgmCommonService.PGMUnit.FunctionRepository.DeleteBankLetterBody(Convert.ToInt64(entity.Id));
                    if (del == 0)
                    {
                        _pgmCommonService.PGMUnit.BankAdviceLetters.Update(entity, "Id", navigationList, "Id");
                        _pgmCommonService.PGMUnit.BankAdviceLetters.SaveChanges();

                        pgm_LetterTemplateBody.BankLetterId = entity.Id;
                        _pgmCommonService.PGMUnit.BankAdviceLetterBodyTextRepository.Add(pgm_LetterTemplateBody);
                        _pgmCommonService.PGMUnit.BankAdviceLetterBodyTextRepository.SaveChanges();
                    }

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
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }

            PopulateDropdown(model);
            PrepareModelEdit(model);

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Delete(int id)
        {
            int rollbackResult = 0;

            if (id > 0)
            {
                try
                {
                    rollbackResult = _pgmCommonService.PGMUnit.FunctionRepository.DeleteBankLetter(id, User.Identity.Name);
                    if (rollbackResult == 0)
                    {
                        return Json(new
                        {
                            Success = 1,
                            Message = ErrorMessages.DeleteSuccessful
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {

                        return Json(new
                        {
                            Success = 0,
                            Message = ErrorMessages.DeleteSuccessful
                        }, JsonRequestBehavior.AllowGet);
                    }
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
            else
            {
                return Json(new
                {
                    Success = 0,
                    Message = ErrorMessages.DeleteFailed
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [NoCache]
        public ActionResult GetYearList()
        {
            var model = new BankAdviceLetterViewModel();
            model.YearList = Common.PopulateYearList().ToList();
            return View("_Select", model.YearList);
        }

        [NoCache]
        public ActionResult GetMonthList()
        {
            var monthList = Common.PopulateMonthList().ToList();
            return View("_Select", monthList);
        }

        [NoCache]
        public ActionResult GetBankList(string letterType)
        {
            //var BankName = (from BA in _pgmCommonService.PGMUnit.BankAccount.GetAll()
            //                join BBA in _pgmCommonService.PGMUnit.BankDetails.GetAll() on BA.BankAccountId equals BBA.Id
            //                join BBM in _pgmCommonService.PGMUnit.BankMaster.GetAll() on BBA.BankBranchMapId equals BBM.Id
            //                join B in _pgmCommonService.PGMUnit.BankNameRepository.GetAll() on BBM.BankId equals B.Id
            //                //where BA.AccountFor==letterType
            //                select new
            //                {
            //                    BankId = B.Id,
            //                    BankName = B.Name
            //                }).DistinctBy(x => x.BankId).ToList();

            //return Json(BankName.Select(y => new SelectListItem()
            //{
            //    Text = y.BankName,
            //    Value = y.BankId.ToString()
            //}).ToList());

            return HttpNotFound();
        }

        [NoCache]
        public ActionResult GetBankAccNo(int? id, string LetterType)
        {
            BankAdviceLetterViewModel model = new BankAdviceLetterViewModel();

            var bankAccNo = (from BA in _pgmCommonService.PGMUnit.BankAccount.GetAll()
                             join BBA in _pgmCommonService.PGMUnit.AccBankAccountRepository.GetAll() on BA.BankAccountId equals BBA.id
                             where BBA.bankId == id && BA.AccountFor == LetterType
                             select BBA).ToList();

            return Json(bankAccNo.Select(x => new SelectListItem()
            {
                Text = x.accountNumber,
                Value = x.accountNumber.ToString()
            }).ToList());
        }

        [NoCache]
        public JsonResult GetBranchNbankAddress(string LetterType, int? bankId, string accountNo)
        {
            var obj = (from BA in _pgmCommonService.PGMUnit.BankAccount.GetAll()
                       join BBA in _pgmCommonService.PGMUnit.AccBankAccountRepository.GetAll() on BA.BankAccountId equals BBA.id
                       join BB in _pgmCommonService.PGMUnit.AccBankBranchRepository.GetAll() on BBA.bankBranchId equals BB.id
                       where BB.bankId == bankId
                       //&& BA.AccountFor == LetterType
                       && BBA.accountNumber == accountNo
                       select new { BB.id, BB.address }).FirstOrDefault();

            if (obj != null)
            {
                return Json(new
                {
                    BranchId = obj.id,
                    BankAddress = obj.address
                });
            }
            else
            {
                return Json(new
                {
                    BranchId = 0
                });
            }
        }

        [NoCache]
        public ActionResult GetCorrespondingYearList(string letterType)
        {
            var model = new BankAdviceLetterViewModel();
            if (letterType == "Salary")
            {
                model.YearList = (from s in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll() select s).DistinctBy(x => x.SalaryYear).Select(y => new SelectListItem()
                {
                    Text = y.SalaryYear,
                    Value = y.SalaryYear.ToString()
                }).ToList();
            }
            else
            {
                model.YearList = (from b in _pgmCommonService.PGMUnit.BonusMasterRepository.GetAll() select b).DistinctBy(x => x.BonusYear).Select(z => new SelectListItem()
                {
                    Text = z.BonusYear,
                    Value = z.BonusYear.ToString()
                }).ToList();
            }
            return Json(model.YearList);
        }

        [NoCache]
        public ActionResult GetCorrespondingMonthList(string letterType, string year)
        {
            var model = new BankAdviceLetterViewModel();
            if (letterType == "Salary")
            {
                model.MonthList = (from s in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll() where s.SalaryYear == year select s).DistinctBy(x => x.SalaryMonth).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).Select(y => new SelectListItem()
                {
                    Text = y.SalaryMonth,
                    Value = y.SalaryMonth.ToString()
                }).ToList();
            }
            else
            {
                model.MonthList = (from b in _pgmCommonService.PGMUnit.BonusMasterRepository.GetAll() where b.BonusYear == year select b).DistinctBy(x => x.BonusMonth).OrderBy(x => Convert.ToDateTime(x.BonusYear + "-" + x.BonusMonth + "-01")).Select(z => new SelectListItem()
                {
                    Text = z.BonusMonth,
                    Value = z.BonusMonth.ToString()
                }).ToList();
            }
            return Json(model.MonthList);
        }

        #endregion

        #region Utilities

        [NoCache]
        public string GetBusinessLogicValidation(BankAdviceLetterViewModel model)
        {
            string errorMessage = string.Empty;
            if (model.LetterType == "Salary")
            {

                var list = _monthlySalaryService.GetSalaryInfoList(
                                             ""
                                            , null
                                            , null
                                            , 0
                                            , 20
                                            , 1
                                            , false
                                            , model.SalaryYear
                                            , model.SalaryMonth
                                            , LoggedUserZoneInfoId)
                                    .OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01"))
                                    .ToList();

                errorMessage = string.Empty;
                if (list.Count == 0)
                {
                    errorMessage = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
            }
            else
            {
                var list = _pgmBonusService.GetBonusMasterData(
                                                                ""
                                                                , "ID"
                                                                , "Asc"
                                                                , 0
                                                                , 20
                                                                , 1
                                                                , false
                                                                , model.SalaryYear
                                                                , model.SalaryMonth
                                                                , LoggedUserZoneInfoId)
                                                                .OrderBy(x => Convert.ToDateTime(x.BonusYear + "-" + x.BonusMonth + "-01"))
                                                                .ToList();

                errorMessage = string.Empty;
                if (list.Count == 0)
                {
                    errorMessage = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
            }
            return errorMessage;
        }

        [NoCache]
        private void PopulateDropdown(BankAdviceLetterViewModel model)
        {
            model.YearList = Common.PopulateYearList().DistinctBy(x => x.Value).ToList();

            var BankName = (from BA in _pgmCommonService.PGMUnit.BankAccount.GetAll()
                            join BBA in _pgmCommonService.PGMUnit.AccBankAccountRepository.GetAll() on BA.BankAccountId equals BBA.id
                            join B in _pgmCommonService.PGMUnit.AccBankRepository.GetAll() on BBA.bankId equals B.id
                            where BA.ZoneInfoId == LoggedUserZoneInfoId
                            select new
                            {
                                BankId = B.id,
                                BankName = B.bankName
                            }).DistinctBy(x => x.BankId).OrderBy(x => x.BankName).ToList();

            model.BankList = BankName
                .Select(y => new SelectListItem()
                {
                    Text = y.BankName,
                    Value = y.BankId.ToString()
                }).ToList();

            model.AccountNoList = (from BA in _pgmCommonService.PGMUnit.BankAccount.GetAll()
                                   join BBA in _pgmCommonService.PGMUnit.AccBankAccountRepository.GetAll() on BA.BankAccountId equals BBA.id
                                   where BBA.bankId == model.BankId
                                   where BA.ZoneInfoId == LoggedUserZoneInfoId
                                   select new SelectListItem()
                                   {
                                       Text = BBA.accountNumber,
                                       Value = BBA.accountNumber.ToString()
                                   }).Distinct().OrderBy(x => x.Text).ToList();

            model.MonthList = Common.PopulateMonthList().DistinctBy(x => x.Value).ToList();

            model.SignatoryNameList = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList().Where(e => e.ZoneInfoId == LoggedUserZoneInfoId).OrderBy(x => x.FullName).DistinctBy(x => x.FullName).Select(y => new SelectListItem()
            {
                Text = y.FullName,
                Value = y.Id.ToString()
            }).ToList();

            model.BonusTypeList = _pgmCommonService.PGMUnit.BonusTypeRepository.GetAll().Select(y => new SelectListItem()
            {
                Text = y.BonusType,
                Value = y.Id.ToString()
            }).ToList();
        }

        [NoCache]
        private void PrepareModelEdit(BankAdviceLetterViewModel model)
        {
            model.YearList = Common.PopulateYearList().DistinctBy(x => x.Value).ToList();

            model.MonthList = Common.PopulateMonthList().DistinctBy(x => x.Value).ToList();

            model.AccountNoList = (from BA in _pgmCommonService.PGMUnit.BankAccount.GetAll()
                                   join BBA in _pgmCommonService.PGMUnit.AccBankAccountRepository.GetAll() on BA.BankAccountId equals BBA.id
                                   where BBA.bankId == model.BankId
                                   && BBA.ZoneInfoId == LoggedUserZoneInfoId
                                   //&& BA.AccountFor==model.LetterType
                                   select new SelectListItem()
                                   {
                                       Text = BBA.accountNumber,
                                       Value = BBA.accountNumber.ToString()
                                   }).Distinct().OrderBy(x => x.Text).ToList();


            var BankName = (from BA in _pgmCommonService.PGMUnit.BankAccount.GetAll()
                            join BBA in _pgmCommonService.PGMUnit.AccBankAccountRepository.GetAll() on BA.BankAccountId equals BBA.id
                            join B in _pgmCommonService.PGMUnit.AccBankRepository.GetAll() on BBA.bankId equals B.id
                            //where BA.AccountFor == model.LetterType

                            select new
                            {
                                BankId = B.id,
                                BankName = B.bankName
                            }).DistinctBy(x => x.BankId).OrderBy(x => x.BankName).ToList();

            model.BankList = BankName
                .Select(y => new SelectListItem()
                {
                    Text = y.BankName,
                    Value = y.BankId.ToString()
                }).ToList();

            model.BonusTypeList = _pgmCommonService.PGMUnit.BonusTypeRepository.GetAll().Select(y => new SelectListItem()
            {
                Text = y.BonusType,
                Value = y.Id.ToString()
            }).ToList();

        }

        [NoCache]
        private string GetReplacedTemplateVariableValue(string varName, BankAdviceLetterViewModel model)
        {
            string replacedString = varName;
            int TotalAmount = Convert.ToInt32(Math.Round(model.TotalAmount, 2));

            replacedString = replacedString.Replace("@@Year", model.SalaryYear);
            replacedString = replacedString.Replace("@@Month", model.SalaryMonth);
            replacedString = replacedString.Replace("@@TypeOfLetterB", model.LetterTypeB);
            replacedString = replacedString.Replace("@@TypeOfLetter", model.LetterType);
            replacedString = replacedString.Replace("@@TotalAmount", TotalAmount.ToString());
            replacedString = replacedString.Replace("@@BonusType", model.BonusType);
            replacedString = replacedString.Replace("@@DebitBankAccount", model.AccountNo);
            replacedString = replacedString.Replace("@@InWords", NumberToText(TotalAmount));
            replacedString = replacedString.Replace("@@FiscalYear", model.SalaryYear);
            return replacedString;
        }

        [NoCache]
        private static string NumberToText(int number)
        {
            if (number == 0) return "";

            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";

            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }

            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };

            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };

            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };

            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }

            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens

                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");

                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        public string ConvertEnglishDigitToBangla(int number)
        {
            string bengaliNumber = string.Concat(number.ToString().Select(c => (char)('\u09E6' + c - '0')));
            return bengaliNumber;
        }

        #endregion

        #region Autocomplete Employee--------------
        public JsonResult AutoCompleteEmployeeList(string term)
        {
            var result = (from r in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                          where r.EmployeeInitial.ToLower().StartsWith(term.ToLower()) && r.DateofInactive == null && r.ZoneInfoId == LoggedUserZoneInfoId
                          select new { r.Id, r.FullName, r.EmployeeInitial }).Distinct().OrderBy(x => x.EmployeeInitial);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [NoCache]
        public JsonResult GetEmployeeInfo(int employeeId)
        {
            string msg = string.Empty;
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(employeeId);
            if (emp != null && emp.DateofInactive != null)
            {
                msg = "InactiveEmployee";
            }

            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    if (emp != null)
                    {
                        return Json(new
                        {
                            EmpId = emp.Id,
                            EmpID = emp.EmpID,
                            EmployeeName = emp.FullName,
                            EmployeeDesignation = emp.DesignationName
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                    return Json(new { Result = false });
                }
            }
            else
            {
                return Json(new { Result = msg });
            }
        }
        #endregion


    }
}
