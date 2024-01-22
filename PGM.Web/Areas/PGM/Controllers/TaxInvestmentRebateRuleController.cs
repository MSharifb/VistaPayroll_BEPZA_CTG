using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.TaxInvestmentRebateRule;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class TaxInvestmentRebateRuleController : Controller
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor

        public TaxInvestmentRebateRuleController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion
        //
        // GET: /PGM/TaxInvestmentRebateRule/
        public ActionResult Index()
        {
            var model = new TaxInvestmentRebateRuleMasterViewModel();
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TaxInvestmentRebateRuleMasterViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<TaxInvestmentRebateRuleMasterViewModel> list = (from R in _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.GetAll()
                join RM in _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.GetAll() on R.Id equals RM.RebateRuleId
                select new TaxInvestmentRebateRuleMasterViewModel()
                {
                    Id = RM.Id,
                    RebateRuleId = R.Id,
                    IncomeYear = R.IncomeYear,
                    AssessmentYear = R.AssessmentYear,
                    SlabNo = RM.SlabNo,
                    LowerLimit = Convert.ToDecimal(RM.LowerLimit),
                    UpperLimit = Convert.ToDecimal(RM.UpperLimit)

                }).OrderBy(x => x.IncomeYear).ToList();

            totalRecords = list == null ? 0 : list.Count;

            if (request.Searching)
            {
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
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.RebateRuleId,
                    d.IncomeYear,
                    d.AssessmentYear,
                    
                    d.LowerLimit,
                    d.UpperLimit,
                    d.SlabNo,

                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        #region Create/Edit/Delete Actions
        public ActionResult Create()
        {
            TaxInvestmentRebateRuleMasterViewModel masterModel = new TaxInvestmentRebateRuleMasterViewModel();
            masterModel.strMode = "Create";
            PopulateList(masterModel);
            return View(masterModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(TaxInvestmentRebateRuleMasterViewModel masterModel)
        {
            var model = new TaxInvestmentRebateRuleViewModel();

            model.IsError = 1;
            model.errClass = "failed";

            if (ModelState.IsValid)
            {
                var IUser = User.Identity.Name;
                var IDate = DateTime.Now;

                try
                {
                    model.IncomeYear = masterModel.IncomeYear;
                    model.AssessmentYear = masterModel.AssessmentYear;
                    model.IUser = IUser;
                    model.IDate = IDate;
                    model.strMode = masterModel.strMode;

                    String errorList = CheckingBusinessLogicValidation(masterModel);

                    if (!String.IsNullOrEmpty(errorList))
                    {
                        masterModel.ErrMsg = errorList;
                        //PopulateList(masterModel);
                        //return View(masterModel);
                    }
                    else
                    {
                        int ruleId = 0;

                        // add rebate rule (if not exists)
                        var isRuleExist = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.GetAll()
                            .Any(r => r.IncomeYear == model.IncomeYear);
                        if (isRuleExist)
                        {
                            ruleId = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.GetAll()
                                .FirstOrDefault(r => r.IncomeYear == model.IncomeYear).Id;
                        }
                        else
                        {
                            var rule = model.ToEntity();
                            _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.Add(rule);
                            ruleId = rule.Id;
                        }

                        // add master rebate rule
                        masterModel.RebateRuleId = ruleId;
                        var masterEntity = masterModel.ToEntity();
                        _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.Add(masterEntity);

                        // add detail rebate rule
                        foreach (var c in masterModel.RuleDetail)
                        {
                            masterEntity.PGM_TaxInvestmentRebateRuleDetail.Add(new PGM_TaxInvestmentRebateRuleDetail
                            {
                                RebateRuleMasterId = masterEntity.Id,
                                SlabNo = c.SlabNo,
                                LowerLimit = c.LowerLimit,
                                UpperLimit = c.UpperLimit,
                                RebateRate = c.RebateRate,
                                IUser = IUser,
                                IDate = IDate
                            });
                        }

                        _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        return View("Index", model);
                    }
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsSuccessful = false;
            }

            PopulateList(masterModel);
            return View(masterModel);
        }

        [HttpGet]
        public ActionResult Edit(int id, string type)
        {
            var masterEntity = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.GetByID(id);
            var ruleMaster = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.GetByID(masterEntity.RebateRuleId);
            var model = masterEntity.ToModel();
            model.IncomeYear = ruleMaster.IncomeYear;
            model.AssessmentYear = ruleMaster.AssessmentYear;
            model.strMode = "Edit";

            if (masterEntity.PGM_TaxInvestmentRebateRuleDetail != null)
            {
                model.RuleDetail = new List<TaxInvestmentRebateRuleDetailViewModel>();
                foreach (var item in masterEntity.PGM_TaxInvestmentRebateRuleDetail)
                {
                    model.RuleDetail.Add(item.ToModel());
                }
            }

            if (type == "success")
            {
                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = ErrorMessages.UpdateSuccessful;
            }

            PopulateList(model);
            return View("Edit", model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(TaxInvestmentRebateRuleMasterViewModel masterModel)
        {
            var model = new TaxInvestmentRebateRuleViewModel();

            if (ModelState.IsValid)
            {
                try
                {
                    var rebateRule = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.GetAll()
                        .FirstOrDefault(w => w.Id == masterModel.RebateRuleId);
                    model = rebateRule.ToModel();
                    model.IncomeYear = masterModel.IncomeYear;
                    model.AssessmentYear = masterModel.AssessmentYear;
                    model.EUser = User.Identity.Name;
                    model.EDate = DateTime.Now;
                    model.strMode = masterModel.strMode;
                    model.IsError = 1;
                    model.errClass = "failed";

                    String errorList = CheckingBusinessLogicValidation(masterModel);

                    if (!String.IsNullOrEmpty(errorList))
                    {
                        masterModel.ErrMsg = errorList;
                        //PopulateList(masterModel);
                        //return View(masterModel);
                    }
                    else
                    {
                        // update rebate rule
                        var ruleEntity = model.ToEntity();
                        _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.Update(ruleEntity);
                        int ruleId = ruleEntity.Id;

                        // update master rebate rule
                        masterModel.RebateRuleId = ruleId;
                        var masterEntity = masterModel.ToEntity();
                        _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.Update(masterEntity);

                        // update detail rebate rule
                        foreach (var c in masterModel.RuleDetail)
                        {
                            var detail = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.GetAll()
                                .FirstOrDefault(w => w.Id == c.Id);

                            detail.SlabNo = c.SlabNo;
                            detail.LowerLimit = c.LowerLimit;
                            detail.UpperLimit = c.UpperLimit;
                            detail.RebateRate = c.RebateRate;
                            detail.EUser = User.Identity.Name;
                            detail.EDate = DateTime.Now;

                            _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.Update(detail);
                        }

                        _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                        return View("Index", model);
                    }
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsSuccessful = false;
            }

            PopulateList(masterModel);
            return View(masterModel);
        }

        [NoCache]
        [HttpPost]
        public JsonResult DeleteDetail(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                var detail =
                    _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.GetAll()
                        .FirstOrDefault(t => t.Id == id);

                if (detail != null)
                {
                    // Delete Detail
                    _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.Delete(detail);
                    _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.SaveChanges();

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
        public JsonResult DeleteConfirmed(int id) // MasterId
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                var rebateMaster = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.GetAll().FirstOrDefault(d => d.Id == id);
                int ruleId = 0;
                if (rebateMaster != null)
                {
                    ruleId = Common.GetInteger(rebateMaster.RebateRuleId);

                    var rebateDetail = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.GetAll().Where(x => x.RebateRuleMasterId == rebateMaster.Id);
                    if (rebateDetail.Any())
                    {
                        foreach (var itemDetail in rebateDetail)
                        {
                            _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.Delete(x => x.Id == itemDetail.Id);
                        }
                        _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleDetailRepository.SaveChanges();
                    }

                    _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.Delete(m => m.Id == id);
                    _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.SaveChanges();
                }

                if (!_pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.GetAll().Any(d => d.Id == id))
                {
                    _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.Delete(x => x.Id == ruleId);
                    _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.SaveChanges();
                }

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

        #region Other Actions
        private void PopulateList(TaxInvestmentRebateRuleMasterViewModel model)
        {
            model.IncomeYearList = Common.PopulateIncomeYearList();
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

        [HttpPost]
        public ActionResult AddRebateRuleDetail(TaxInvestmentRebateRuleDetailViewModel model)
        {
            return PartialView("_PartialDetail", model);
        }

        private string CheckingBusinessLogicValidation(TaxInvestmentRebateRuleMasterViewModel masterModel)
        {
            string message = string.Empty;

            var isRuleExists = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.GetAll()
                .Any(m => m.IncomeYear == masterModel.IncomeYear);
            if (isRuleExists)
            {
                var rebateRuleId = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleRepository.GetAll()
                    .FirstOrDefault(m => m.IncomeYear == masterModel.IncomeYear).Id;

                var rebateRuleMaster = _pgmCommonService.PGMUnit.TaxInvestmentRebateRuleMasterRepository.GetAll()
                    .Where(m => m.RebateRuleId == rebateRuleId);
                if (rebateRuleMaster != null && rebateRuleMaster.Count() > 0)
                {
                    bool dataExists = false;
                    foreach (var item in rebateRuleMaster)
                    {
                        if (masterModel.LowerLimit > item.LowerLimit && masterModel.UpperLimit > item.UpperLimit)
                        {
                        }
                        else
                        {
                            dataExists = true;
                            break;
                        }
                    }

                    if (dataExists) return "Information you enetered is already exists.";
                }

            }

            return message;
        }

        #endregion


    }
}