using DAL.PGM;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.TaxExemptionRule;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class TaxExemptionRuleController : Controller
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Constructor

        public TaxExemptionRuleController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion
        //
        // GET: /PGM/TaxExemptionRule/
        #region Action
        public ActionResult Index()
        {
            var model = new TaxExemptionRuleViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, TaxExemptionRuleViewModel viewModel, FormCollection form)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<TaxExemptionRuleViewModel> list = (from TE in _pgmCommonService.PGMUnit.TaxExemptionRuleRepository.GetAll()
                                                    select new TaxExemptionRuleViewModel()
                                                    {
                                                        Id = TE.Id,
                                                        IncomeYear = TE.IncomeYear,
                                                        AssessmentYear = TE.AssessmentYear,
                                                        IsActive = Convert.ToBoolean(TE.IsActive)
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
                    d.IncomeYear,
                    d.AssessmentYear,
                    d.IsActive,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            TaxExemptionRuleViewModel model = new TaxExemptionRuleViewModel();
            model.strMode = "Create";
            PopulateList(model);

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(TaxExemptionRuleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.IUser = User.Identity.Name;
                        model.IDate = DateTime.Now;
                        model.IsActive = true;
                        var master = model.ToEntity();

                        foreach (var detail in model.DetailList)
                        {
                            master.PGM_TaxExemptionRuleDetail.Add(new PGM_TaxExemptionRuleDetail
                            {
                                //ExemptionId = master.Id,
                                HeadId = detail.HeadId,
                                TaxExemptionBasisOn = detail.TaxExemptionBasisOn,
                                IsPercentageOnBasis = detail.IsPercentageOnBasis,
                                TaxableInPercentage = detail.TaxableInPercentage,
                                HaveYearlyExemptionLimit = detail.HaveYearlyExemptionLimit,
                                YearlyExemptionLimitAmount = detail.YearlyExemptionLimitAmount,

                                IUser = User.Identity.Name,
                                IDate = DateTime.Now
                            });
                        }

                        _pgmCommonService.PGMUnit.TaxExemptionRuleRepository.Add(master);
                        _pgmCommonService.PGMUnit.TaxExemptionRuleRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        return RedirectToAction("Edit", new { id = master.Id, type = "success" });
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.errClass = "failed";
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

        [NoCache]
        public ActionResult Edit(int id, string type)
        {
            PGM_TaxExemptionRule master = _pgmCommonService.PGMUnit.TaxExemptionRuleRepository.GetByID(id);
            TaxExemptionRuleViewModel model = master.ToModel();
            model.strMode = "Edit";


            if (master != null)
            {
                if (master.PGM_TaxExemptionRuleDetail != null)
                {
                    model.DetailList = new List<TaxExemptionRuleDetailViewModel>();
                    TaxExemptionRuleDetailViewModel detailModel = null;
                    Byte basisOn;
                    foreach (var detail in master.PGM_TaxExemptionRuleDetail)
                    {
                        detailModel = detail.ToModel();
                        detailModel.HeadName = detail.PRM_SalaryHead.HeadName;

                        basisOn = Convert.ToByte(detailModel.TaxExemptionBasisOn);
                        detailModel.TaxExemptionBasis = ((PGMEnum.CalculationBasedOn)basisOn).ToString();
                        detailModel.ExemptionId = master.Id;

                        model.DetailList.Add(detailModel);
                    }
                }
            }
            else
            {
                model.IsError = 1;
                model.errClass = "failed";
                model.ErrMsg = ErrorMessages.DataNotFound;
            }


            if (type == "success")
            {
                model.IsError = 0;
                model.errClass = "success";
                model.ErrMsg = ErrorMessages.UpdateSuccessful;
            }

            PopulateList(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(TaxExemptionRuleViewModel model)
        {
            var checkoutBusinessLogic = string.Empty;

            try
            {
                checkoutBusinessLogic = CheckingBusinessLogicValidation(model);

                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(checkoutBusinessLogic))
                    {
                        model.EUser = User.Identity.Name;
                        model.EDate = DateTime.Now;

                        var master = model.ToEntity();
                        ArrayList arrtyList = new ArrayList();

                        if (model.DetailList != null)
                        {
                            foreach (var detail in model.DetailList)
                            {
                                if (detail.Id == 0)
                                {
                                    var child = new PGM_TaxExemptionRuleDetail()
                                    {
                                        ExemptionId = detail.ExemptionId,
                                        HeadId = detail.HeadId,
                                        TaxExemptionBasisOn = detail.TaxExemptionBasisOn,
                                        IsPercentageOnBasis = detail.IsPercentageOnBasis,
                                        TaxableInPercentage = detail.TaxableInPercentage,
                                        HaveYearlyExemptionLimit = detail.HaveYearlyExemptionLimit,
                                        YearlyExemptionLimitAmount = detail.YearlyExemptionLimitAmount,

                                        EUser = model.EUser,
                                        EDate = model.EDate
                                    };
                                    _pgmCommonService.PGMUnit.TaxExemptionRuleDetailRepository.Add(child);
                                }
                            }
                            _pgmCommonService.PGMUnit.TaxExemptionRuleDetailRepository.SaveChanges();

                            model.IsError = 0;
                            model.errClass = "success";
                            model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                            return RedirectToAction("Edit", new { id = master.Id, type = "success" });
                        }
                    }
                    else
                    {
                        model.IsError = 1;
                        model.errClass = "failed";
                        model.ErrMsg = checkoutBusinessLogic;
                    }
                }
                else
                {
                    model.IsError = 1;
                    model.errClass = "failed";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
                }
            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.errClass = "failed";
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    model.ErrMsg = ErrorMessages.UniqueIndex;
                }
                else
                {
                    model.ErrMsg = ErrorMessages.UpdateFailed;
                }
            }

            PopulateList(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _pgmCommonService.PGMUnit.TaxExemptionRuleDetailRepository.Delete(x => x.ExemptionId == id);
                _pgmCommonService.PGMUnit.TaxExemptionRuleDetailRepository.SaveChanges();

                _pgmCommonService.PGMUnit.TaxExemptionRuleRepository.Delete(id);
                _pgmCommonService.PGMUnit.TaxExemptionRuleRepository.SaveChanges();
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

        [HttpPost, ActionName("DeleteDetail")]
        public JsonResult DeleteDetailConfirmed(int id)
        {
            bool result = false;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                _pgmCommonService.PGMUnit.TaxExemptionRuleDetailRepository.Delete(id);
                _pgmCommonService.PGMUnit.TaxExemptionRuleDetailRepository.SaveChanges();
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


        public ActionResult AddDetail(TaxExemptionRuleDetailViewModel model)
        {
            var master = new TaxExemptionRuleViewModel();

            master.DetailList = new List<TaxExemptionRuleDetailViewModel>();
            master.DetailList.Add(model);
            model.ErrMsg = CheckingBusinessLogicValidation(master);
            if (model.ErrMsg != "")
            {
                return Json(new
                {
                    Success = true,
                    Message = model.ErrMsg
                }, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_MasterDetail", master);
        }

        #endregion

        #region Other

        private string CheckingBusinessLogicValidation(TaxExemptionRuleViewModel model)
        {
            string message = string.Empty;
            if (model.strMode == "Create")
            {
                var master = _pgmCommonService.PGMUnit.TaxExemptionRuleRepository.GetAll()
                                                      //.Join(_pgmCommonService.PGMUnit.TaxExemptionRuleDetailRepository.GetAll(),r=>r.Id,rd=>rd.ExemptionId,(r,rd)=>new{r,rd})
                                                      .Where(t => t.IncomeYear == model.IncomeYear).ToList();
                if (master.Count > 0)
                {
                    message = "Duplicate Entry.";
                }
            }

            return message;
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

        private void PopulateList(TaxExemptionRuleViewModel model)
        {
            model.IncomeYearList = Common.PopulateIncomeYearList();
            model.HeadList = _pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll()
                .Where(h => h.HeadType == "Addition").ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.HeadName,
                    Value = y.Id.ToString()
                }).ToList();

            Dictionary<int, string> assesseeList = Common.GetEnumAsDictionary<PGMEnum.CalculationBasedOn>();
            foreach (KeyValuePair<int, string> item in assesseeList)
            {
                model.ExemptionBasisList.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
        }

        #endregion
    }
}