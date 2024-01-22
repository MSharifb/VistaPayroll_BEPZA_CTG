using Domain.PGM;
using PGM.Web.Areas.PGM.Models.TaxRule;
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
    public class TaxRuleController : Controller
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Ctor

        public TaxRuleController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonservice = pgmCommonservice;
        }

        #endregion

        #region Properties

        public string Message { get; set; }

        #endregion

        #region Actions

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, TaxRuleModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<TaxRuleModel> list = (from tr in _pgmCommonservice.PGMUnit.TaxRule.GetAll()
                                       where (string.IsNullOrEmpty(model.IncomeYear) || model.IncomeYear == tr.IncomeYear)
                                       && (string.IsNullOrEmpty(model.AssessmentYear) || model.AssessmentYear == tr.AssessmentYear)

                                       select new TaxRuleModel()
                                       {
                                           Id = tr.Id,
                                           IncomeYear = tr.IncomeYear,
                                           AssessmentYear = tr.AssessmentYear,

                                           InvestmentRate = Convert.ToDecimal(tr.InvestmentRate),
                                           MaximumInvestment = Convert.ToDecimal(tr.MaximumInvestment),
                                           ExcessIncomePercent = Convert.ToDecimal(tr.ExcessIncomePercent),
                                           SpecialRebateRate = Convert.ToDecimal(tr.SpecialRebateRate),
                                           TaxablePercentage = Convert.ToDecimal(tr.TaxablePercentage),
                                           TaxFreeAmountForHavingChildWithDisability = Convert.ToDecimal(tr.TaxFreeAmountForHavingChildWithDisability)
                                       }).ToList();

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            #region Sorting
            if (request.SortingName == "ID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Id).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Id).ToList();
                }
            }

            if (request.SortingName == "IncomeYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IncomeYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IncomeYear).ToList();
                }
            }

            if (request.SortingName == "AssessmentYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssessmentYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssessmentYear).ToList();
                }
            }

            if (request.SortingName == "InvestmentRate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.InvestmentRate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.InvestmentRate).ToList();
                }
            }

            if (request.SortingName == "MaximumInvestment")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.MaximumInvestment).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.MaximumInvestment).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,

                    d.IncomeYear,
                    d.AssessmentYear,

                    d.InvestmentRate,
                    d.MaximumInvestment,
                    d.ExcessIncomePercent,
                    d.SpecialRebateRate,
                    d.TaxablePercentage,
                    d.TaxFreeAmountForHavingChildWithDisability,

                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            var model = new TaxRuleModel();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new TaxRuleModel();
            PrepareModel(model);
            model.Mode = CrudeAction.Create;
            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(TaxRuleModel model)
        {
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    var entity = model.ToEntity();
                    _pgmCommonservice.PGMUnit.TaxRule.Add(entity);
                    _pgmCommonservice.PGMUnit.TaxRule.SaveChanges();
                    Message = ErrorMessages.InsertSuccessful;

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                Message = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.TaxRule.GetByID(id, "Id");

            var model = entity.ToModel();

            PrepareModel(model);
            model.Mode = CrudeAction.Edit;

            return View("_CreateOrEdit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(TaxRuleModel model)
        {
            Message = CheckBusinessRule(model);
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    var entity = model.ToEntity();
                    _pgmCommonservice.PGMUnit.TaxRule.Update(entity, "Id");
                    _pgmCommonservice.PGMUnit.TaxRule.SaveChanges();
                    Message = ErrorMessages.UpdateSuccessful;

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                Message = string.IsNullOrEmpty(Message) ? ErrorMessages.UpdateFailed : Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _pgmCommonservice.PGMUnit.TaxRule.Delete(id);
                _pgmCommonservice.PGMUnit.TaxRule.SaveChanges();

                return Json(new
                {
                    Success = 1,
                    Message = ErrorMessages.DeleteSuccessful
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);

                return Json(new
                {
                    Success = 0,
                    Message = errMsg
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Util
        private void PrepareModel(TaxRuleModel model)
        {
            model.IncomeYearList = IncomeYearList();
        }

        private IList<SelectListItem> IncomeYearList()
        {
            return Common.PopulateIncomeYearList();
        }

        [NoCache]
        public ActionResult GetIncomeYearList()
        {
            return View("_Select", IncomeYearList());
        }

        [NoCache]
        public ActionResult GetAssessmentYearList()
        {
            return View("_Select", IncomeYearList());
        }

        private string CheckBusinessRule(TaxRuleModel model)
        {
            return Message;
        }
        #endregion

    }
}
