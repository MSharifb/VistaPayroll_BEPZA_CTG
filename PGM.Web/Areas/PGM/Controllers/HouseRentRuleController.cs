using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.HouseRentRule;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class HouseRentRuleController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor

        public HouseRentRuleController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonservice = pgmCommonservice;
        }
        #endregion

        #region Actions

        public ViewResult Index()
        {
            var model = new HouseRentRuleModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, HouseRentRuleModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<HouseRentRuleModel> list = (from hr in _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.GetAll()
                                             select new HouseRentRuleModel()
                                             {
                                                 Id = hr.Id,
                                                 SalaryHeadId = hr.SalaryHeadId,
                                                 SalaryHead = hr.PRM_SalaryHead == null ? "" : hr.PRM_SalaryHead.HeadName,

                                                 SalaryScaleId = hr.SalaryScaleId,
                                                 SalaryScale = hr.PRM_SalaryScale == null ? "" : hr.PRM_SalaryScale.SalaryScaleName,

                                                 RegionId = hr.RegionId,
                                                 Region = hr.PRM_Region == null ? "" : hr.PRM_Region.Name,

                                                 EffectiveDateFrom = hr.EffectiveDateFrom,
                                                 EffectiveDateTo = hr.EffectiveDateTo,
                                             }).ToList();

            if (model.SalaryScaleId != 0)
            {
                list = list.Where(t => t.SalaryScaleId == model.SalaryScaleId).ToList();
            }

            if (model.RegionId != 0)
            {
                list = list.Where(t => t.RegionId == model.RegionId).ToList();
            }

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

            if (request.SortingName == "SalaryHead")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryHead).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryHead).ToList();
                }
            }

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

            if (request.SortingName == "EffectiveDateFrom")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDateFrom).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDateFrom).ToList();
                }
            }

            if (request.SortingName == "EffectiveDateTo")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDateTo).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDateTo).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,

                    d.SalaryHead,

                    d.SalaryScale,

                    d.Region,

                    Convert.ToDateTime(d.EffectiveDateFrom).ToString(DateAndTime.GlobalDateFormat),
                    Convert.ToDateTime(d.EffectiveDateTo).ToString(DateAndTime.GlobalDateFormat),
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };
        }

        public PartialViewResult AddDetail()
        {
            return PartialView("_Detail");
        }

        public ViewResult Details(int id)
        {
            var list = _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.GetByID(id);

            return View(list);
        }

        public ActionResult Create()
        {
            var model = new HouseRentRuleModel();
            model.Mode = CrudeAction.Create;
            PrepareModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(HouseRentRuleModel model)
        {
            try
            {
                model.Mode = CrudeAction.Create;
                List<string> errorList = new List<string>();

                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    entity = GetInsertUserAuditInfo(entity);

                    errorList = GetBusinessLogicValidation(model);

                    if (errorList.Count == 0)
                    {
                        _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.Add(entity);
                        _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.SaveChanges();
                        //return RedirectToAction("Index");
                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                        model.errClass = "success";
                        return View("Index", model);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = Common.ErrorListToString(errorList);
                }
            }
            catch
            {
                //model.IsError = 1;
                //model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);

                model.errClass = "failed";
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.InsertFailed);
            }

            PrepareModel(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.GetByID(id);

            var model = entity.ToModel();

            model.Mode = CrudeAction.Edit;
            if (entity.PGM_HouseRentRuleDetail != null)
            {
                model.HouseRentRuleDetail = new Collection<HouseRentRuleDetailModel>();
                foreach (var detail in entity.PGM_HouseRentRuleDetail)
                {
                    model.HouseRentRuleDetail.Add(detail.ToModel());
                    model.NumberOfSlab += 1;
                }

                PrepareModel(model);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HouseRentRuleModel model)
        {
            try
            {
                model.Mode = CrudeAction.Edit;
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity();
                    ArrayList lstDetail = new ArrayList();

                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    if (model.HouseRentRuleDetail != null)
                    {
                        foreach (var item in model.HouseRentRuleDetail)
                        {
                            var _detail = item.ToEntity();
                            _detail.HouseRentRuleId = entity.Id;

                            // if old item then reflection will retrive old IUser & IDate
                            _detail.IUser = User.Identity.Name;
                            _detail.IDate = DateTime.Now;

                            _detail.EUser = User.Identity.Name;
                            _detail.EDate = DateTime.Now;
                            lstDetail.Add(_detail);
                        }
                    }

                    var NavigationList = new Dictionary<Type, ArrayList>();
                    NavigationList.Add(typeof(PGM_HouseRentRuleDetail), lstDetail);
                    errorList = GetBusinessLogicValidation(model);

                    if (errorList.Count == 0)
                    {
                        _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.Update(entity, NavigationList);
                        _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.SaveChanges();

                        model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        model.errClass = "success";
                        return View("Index", model);
                    }
                }

                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = Common.ErrorListToString(errorList);
                }
            }
            catch
            {
                model.IsError = 1;
                model.ErrMsg = Common.GetCommomMessage(CommonMessage.UpdateFailed);
            }

            PrepareModel(model);
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(PGM_HouseRentRuleDetail) };
                _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.Delete(id, allTypes);
                _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.SaveChanges();
                result = true;
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

        #endregion Action

        #region Others

        private PGM_HouseRentRule GetInsertUserAuditInfo(PGM_HouseRentRule entity)
        {
            entity.IUser = User.Identity.Name;
            entity.IDate = DateTime.Now;

            foreach (var child in entity.PGM_HouseRentRuleDetail)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;
            }
            return entity;
        }

        private PGM_HouseRentRule GetEditUserAuditInfo(PGM_HouseRentRule entity)
        {
            entity.EUser = User.Identity.Name;
            entity.EDate = DateTime.Now;

            foreach (var child in entity.PGM_HouseRentRuleDetail)
            {
                child.EUser = User.Identity.Name;
                child.EDate = DateTime.Now;
            }
            return entity;
        }

        private void PrepareModel(HouseRentRuleModel model)
        {
            model.SalaryScaleList = FillSalaryScale();
            model.SalaryHeadList = FillSalaryHead();
            model.RegionList = FillRegion();
            model.BaseOnList = Common.PopulateBaseOn();
        }

        public List<string> GetBusinessLogicValidation(HouseRentRuleModel model)
        {
            List<string> errorMessage = new List<string>();

            var q = from hrr in _pgmCommonservice.PGMUnit.HouseRentRuleRepositoty.GetAll()
                    where hrr.EffectiveDateFrom <= model.EffectiveDateFrom && model.EffectiveDateFrom <= hrr.EffectiveDateTo
                           && (hrr.EffectiveDateFrom <= model.EffectiveDateTo && model.EffectiveDateTo <= hrr.EffectiveDateTo)
                           && hrr.SalaryScaleId == model.SalaryScaleId
                           && hrr.RegionId == model.RegionId
                    select hrr;

            if (model.Mode == CrudeAction.Edit)
            {
                q = q.Where(t => t.Id != model.Id);
            }

            if (q.Count() > 0)
            {
                errorMessage.Add("Same Salary scale, region and date range have another rule");
            }

            if (model.EffectiveDateFrom > model.EffectiveDateTo)
            {
                errorMessage.Add("To date must be greater then from date");
            }

            //if (entity.NumberOfSlab != entity.PGM_TaxRateDetail.Count())
            //{
            //    errorMessage.Add("Number of record in tax rate detail must be equal to number of slab.");
            //}
            //if (entity.NumberOfSlab < 0)
            //{
            //    errorMessage.Add("Number Of slab must be greater than zero.");
            //}

            //decimal PreviousRate = 0;
            //decimal PreviousHigherLimit = 0;
            //int cnt = 0;

            //foreach (var detail in entity.PGM_TaxRateDetail)
            //{
            //    cnt++;
            //    //in First slab- lower limit should be zero
            //    //Upper limit must be higher than lower limit
            //    if (cnt==1 && detail.LowerLimit !=0)
            //    {
            //        errorMessage.Add("In first slab, lower limit must be started from zero.");
            //        break;
            //    }

            //    //Lower Limit must be started from previouse upper limit
            //    if (detail.LowerLimit != (PreviousHigherLimit+1) && cnt != 1)
            //    {
            //        errorMessage.Add("Lower limit must be started after higher limit of previous slab.");
            //        break;
            //    }

            //    //Upper limit must be higher than lower limit
            //    if (detail.LowerLimit >= detail.UpperLimit)
            //    {
            //        errorMessage.Add("Upper limit must be higher than the lower.");
            //        break;
            //    }

            //    //Tax rate must be higher than previous record
            //    if (detail.TRate <= PreviousRate && cnt!=1)
            //    {
            //        errorMessage.Add("Invalid tax rate!");
            //        break;
            //    }
            //    PreviousRate = detail.TRate;
            //    PreviousHigherLimit = detail.UpperLimit;
            //}

            return errorMessage;
        }

        public ActionResult GetApplicableForList()
        {
            Dictionary<string, string> GetApplicableFor = new Dictionary<string, string>();
            GetApplicableFor.Add("Male", "Male");
            GetApplicableFor.Add("Female", "Female");
            ViewBag.ApplicableForList = GetApplicableFor;
            return PartialView("Select", GetApplicableFor);
        }

        private IList<SelectListItem> FillSalaryScale()
        {
            var itemList = new List<SelectListItem>();

            var list = _pgmCommonservice.PGMUnit.SalaryScaleRepository.GetAll();

            if (list != null)
            {
                foreach (var item in list)
                {
                    itemList.Add(new SelectListItem() { Text = item.SalaryScaleName, Value = item.Id.ToString() });
                }
            }

            return itemList;
        }

        private IList<SelectListItem> FillSalaryHead()
        {
            string ht = PGMEnum.SalaryHeadType.Addition.ToString();
            var itemList = new List<SelectListItem>();

            var list = _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(t => t.HeadType.Equals(ht));

            foreach (var item in list)
            {
                itemList.Add(new SelectListItem() { Text = item.HeadName, Value = item.Id.ToString() });
            }

            return itemList;
        }

        private IList<SelectListItem> FillRegion()
        {
            var itemList = new List<SelectListItem>();

            var list = _pgmCommonservice.PGMUnit.RegionRepository.GetAll();

            foreach (var item in list)
            {
                itemList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }

            return itemList;
        }

        [NoCache]
        public ActionResult GetSalaryHeadSearch()
        {
            var list = _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(t => t.HeadType == "Deduction").OrderBy(x => x.HeadName).ToList();

            return PartialView("_Select", Common.PopulateSalaryHeadDDL(list));
        }

        [NoCache]
        public ActionResult GetSalaryScaleSearch()
        {
            var list = _pgmCommonservice.PGMUnit.SalaryScaleRepository.GetAll().OrderBy(x => x.DateOfEffective).ToList();

            return PartialView("_Select", Common.PopulateSalaryScaleDDL(list));
        }

        [NoCache]
        public ActionResult GetRegionSearch()
        {
            var list = _pgmCommonservice.PGMUnit.RegionRepository.GetAll().OrderBy(x => x.Name).ToList();

            return PartialView("_Select", Common.PopulateDDLList(list));
        }

        //public ActionResult FillSalaryScaleSearch()
        //{
        //    Dictionary<string, string> IncomeYear = new Dictionary<string, string>();
        //    for (int i = Common.CurrentDateTime.Year + 1; i >= 2000; i--)
        //    {
        //        var iyFormat = (i - 1) + "-" + i;
        //        IncomeYear.Add(iyFormat, iyFormat);
        //    }

        //    ViewBag.IncomeYearList = IncomeYear;
        //    return PartialView("Select", IncomeYear);
        //}

        //public ActionResult GetAssessmentYearList()
        //{
        //    Dictionary<string, string> IncomeYear = new Dictionary<string, string>();
        //    for (int i = Common.CurrentDateTime.Year+1; i >=2000; i--)
        //    {
        //        var iyFormat = (i - 1) + "-" + i;
        //        IncomeYear.Add(iyFormat, iyFormat);
        //    }

        //    return PartialView("Select", IncomeYear);
        //}

        #endregion Others

    }
}
