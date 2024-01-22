using Domain.PGM;
using PGM.Web.Areas.PGM.Models.TaxRegion;
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
    public class TaxRegionRuleController : Controller
    {
        //
        // GET: /PGM/TaxRegionRule/
        private readonly PGMCommonService _pgmCommonservice;

        public TaxRegionRuleController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonservice = pgmCommonservice;
        }

        #region Action Methods
        // GET: PGM/ElectricBill
        public ActionResult Index()
        {
            var model = new TaxRegionRuleViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, TaxRegionRuleViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<TaxRegionRuleViewModel> list = (from region in _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.GetAll()
                                                 select new TaxRegionRuleViewModel
                                                 {
                                                     Id = region.Id,
                                                     RegionName = region.RegionName,
                                                     IsActive = region.IsActive
                                                 }
            ).OrderBy(x => x.Id).ToList();
            if (request.Searching)
            {
                if ((model.RegionName != null))
                {
                    list = list.Where(d => d.RegionName.Contains(model.RegionName)).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "RegionName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.RegionName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.RegionName).ToList();
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
                    d.RegionName ,
                    d.IsActive ,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new TaxRegionRuleViewModel();
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(TaxRegionRuleViewModel model)
        {
            string errorList = string.Empty;

            errorList = BusinessLogicValidation(model);
            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    var entity = model.ToEntity();
                    if (entity.Id > 0)
                    {
                        entity.EUser = User.Identity.Name;
                        entity.EDate = DateTime.Now;
                        _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.Update(entity);
                        _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.SaveChanges();

                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                    }
                    else
                    {
                        entity.IsActive = true;
                        _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.Add(entity);
                        _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.SaveChanges();

                        model.IsError = 0;
                        model.errClass = "success";
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    }
                    return View("Index", model);
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

            model.strMode = "Create";
            return View(model);
        }

        public ActionResult Edit(int? Id)
        {
            var model = _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.GetByID(Id).ToModel();
            model.strMode = "Edit";
            return View("Edit", model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.GetByID(id);

            try
            {
                _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.Delete(entity.Id);
                _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.SaveChanges();

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
        #endregion

        #region Private Methods
        private string BusinessLogicValidation(TaxRegionRuleViewModel model)
        {
            string massage = string.Empty;
            var regionList = _pgmCommonservice.PGMUnit.TaxRegionRuleRepository.GetAll().ToList();

            if (model.strMode == "Create")
            {
                regionList = regionList.Where(x => x.RegionName == model.RegionName).ToList();
                if (regionList.Count > 0)
                {
                    massage = "Duplicate Entry.";
                }
            }
            else
            {
                regionList = regionList.Where(x => x.RegionName == model.RegionName && x.Id != model.Id).ToList();
                if (regionList.Count > 0)
                {
                    massage = "Duplicate Entry.";
                }
            }
            return massage;
        }
        #endregion
    }
}