using Domain.PGM;
using PGM.Web.Areas.PGM.Models.TaxRegion;
using PGM.Web.Resources;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class RegionWiseMinTaxController : Controller
    {
        //
        // GET: /PGM/RegionWiseMinTax/
    
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor
        public RegionWiseMinTaxController(PGMCommonService pgmCommonService)
        {
            this._pgmCommonService = pgmCommonService;
        }
        #endregion

        #region Action
        public ActionResult Index()
        {
            var model = new RegionWiseMinTaxViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, RegionWiseMinTaxViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<RegionWiseMinTaxViewModel> list = (from unit in _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.GetAll()
                                                    join region in _pgmCommonService.PGMUnit.TaxRegionRuleRepository.GetAll() on unit.RegionId equals region.Id
                                                    select new RegionWiseMinTaxViewModel()
                                            {
                                                Id = unit.Id,
                                                RegionId = unit.RegionId,
                                                Region = region.RegionName,
                                                IncomeYear = unit.IncomeYear,
                                                AssessmentYear = unit.AssessmentYear,
                                                MinimumAmount = unit.MinimumAmount
                                            }).OrderBy(x => x.IncomeYear).ToList();

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "Region")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Region).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Region).ToList();
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

            #endregion

            if (request.Searching)
            {
                if (model.RegionId>0)
                {
                    list = list.Where(x => x.RegionId == model.RegionId).ToList();
                }
                if (!string.IsNullOrEmpty(model.AssessmentYear))
                {
                    list = list.Where(x => x.AssessmentYear.Trim().ToLower().Contains(model.AssessmentYear.Trim().ToLower())).ToList();
                }
                if (!string.IsNullOrEmpty(model.IncomeYear))
                {
                    list = list.Where(x => x.IncomeYear.Trim().ToLower().Contains(model.IncomeYear.Trim().ToLower())).ToList();
                }
            }

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
                  d.RegionId,
                  d.IncomeYear,
                  d.AssessmentYear,
                  d.Region,
                  d.MinimumAmount,
                  "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            RegionWiseMinTaxViewModel model = new RegionWiseMinTaxViewModel();
            model.strMode = "Create";
            PopulateDropDown(model);
            return View("Create", model);
        }


        [HttpPost]
        [NoCache]
        public ActionResult Create(RegionWiseMinTaxViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            var strMessage = CheckDuplicateEntry(model);
            model.ErrMsg = strMessage;
            if (!string.IsNullOrWhiteSpace(strMessage))
            {
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(strMessage))
            {
                var entity = model.ToEntity();
                try
                {
                    _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.Add(entity);
                    _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.SaveChanges();
                    model.IsError = 0;
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);

                    return RedirectToAction("Index",model);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        model.IsError = 0;
                        model.errClass = "failed";
                        model.ErrMsg = ErrorMessages.InsertFailed;
                    }

                }
            }
            PopulateDropDown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int Id)
        {
            var entity = _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.GetByID(Id);
            RegionWiseMinTaxViewModel model = entity.ToModel();
            model.strMode = "Edit";
            PopulateDropDown(model);
            return View("Edit", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(RegionWiseMinTaxViewModel model)
        {
            model.IsError = 1;
            model.ErrMsg = string.Empty;
            var strMessage = CheckDuplicateEntry(model);
            model.ErrMsg = strMessage;
            if (!string.IsNullOrWhiteSpace(strMessage))
            {
                model.errClass = "failed";
            }
            if (ModelState.IsValid && string.IsNullOrWhiteSpace(strMessage))
            {
                model.EUser = User.Identity.Name;
                model.EDate = DateTime.Now;
                var entity = model.ToEntity();
                try
                {
                    _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.Update(entity);
                    _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.SaveChanges();
                    model.errClass = "success";
                    model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                    return RedirectToAction("Index",model);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate"))
                    {
                        model.errClass = "failed";
                        model.ErrMsg = ErrorMessages.UniqueIndex;
                    }
                    else
                    {
                        model.errClass = "failed";
                        model.ErrMsg = ErrorMessages.UpdateFailed;
                    }
                    //model.errClass = "failed";
                    //model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            PopulateDropDown(model);
            return View(model);
        }
        
        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;
            var tempPeriod = _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.GetByID(id);
            try
            {
                _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.Delete(id);
                _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.SaveChanges();
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
            }, JsonRequestBehavior.AllowGet);
        }

        //private EstimationHeadViewModel RemoveSpace(EstimationHeadViewModel model, string headName)
        //{
        //    var str = Regex.Replace(headName, " {2,}", " ");
        //    model.HeadName = str;
        //    return model;
        //}

        private string CheckDuplicateEntry(RegionWiseMinTaxViewModel model)
        {
            string message = string.Empty;

            var list = _pgmCommonService.PGMUnit.TaxRegionWiseMinRuleRepository.GetAll().ToList();
            if(model.strMode == "Create")
            {
                list = list.Where(m => m.RegionId == model.RegionId && m.IncomeYear == model.IncomeYear).ToList();
                if(list.Count > 0)
                {
                    message = "Duplicate Entry.";
                }
            }
            else 
            {
                list = list.Where(m => m.RegionId == model.RegionId && m.IncomeYear == model.IncomeYear && m.Id != model.Id).ToList();
                if (list.Count > 0)
                {
                    message = "Duplicate Entry.";
                }
            }

            return message;
        }
        #endregion

        #region other

        public void PopulateDropDown(RegionWiseMinTaxViewModel model)
        {
            model.IncomeYearList = Common.PopulateIncomeYearList();
            model.RegionList = _pgmCommonService.PGMUnit.TaxRegionRuleRepository.GetAll().Select(r =>
                                new SelectListItem
                                {
                                    Text = r.RegionName,
                                    Value = r.Id.ToString()
                                }).ToList();

        }

        [NoCache]
        public ActionResult GetIncomeYearList()
        {
            return PartialView("_Select", Common.PopulateIncomeYearList());
        }

        [NoCache]
        public ActionResult GetAssessmentYearList()
        {
            return PartialView("_Select", Common.PopulateAssessmentYearList());
        }
         [NoCache]
        public ActionResult GetRegionList()
        {
            var RegionList = _pgmCommonService.PGMUnit.TaxRegionRuleRepository.GetAll().Select(r =>
                               new SelectListItem
                               {
                                   Text = r.RegionName,
                                   Value = r.Id.ToString()
                               }).ToList();
            return PartialView("_Select", RegionList);
        }
        [NoCache]
        public ActionResult GetAssessmentYear(string incomeYear)
         {
             incomeYear = incomeYear.Substring(0, 4);
             int year = Convert.ToInt32(incomeYear);
             string assessmentYear = (year + 1) + "-" + (year + 2);

             return Json(new
             {
                 result = assessmentYear
             }, JsonRequestBehavior.AllowGet);     
         }

        #endregion
    }
}