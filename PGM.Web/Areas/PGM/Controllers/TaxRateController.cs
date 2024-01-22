using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.TaxRate;
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
    public class TaxRateController : Controller
    {
        #region Fields

        private readonly PGMCommonService _pgmCommonservice;

        #endregion

        #region Constructor

        public TaxRateController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }

        #endregion

        #region Actions

        public ViewResult Index()
        {
            var model = new TaxRateModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, TaxRateModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<TaxRateModel> list = (from tr in _pgmCommonservice.PGMUnit.TaxRateMasterRepository.GetAll()
                                       select new TaxRateModel()
                                       {
                                           Id = tr.Id,
                                           IncomeYear = tr.IncomeYear,
                                           AssessmentYear = tr.AssessmentYear,
                                           AssesseeTypeId = tr.AssesseeTypeId,
                                           NumberOfSlab = tr.NumberOfSlab
                                       }).OrderBy(o => o.IncomeYear).ToList();

            totalRecords = list == null ? 0 : list.Count;

            if (request.Searching)
            {
                if (request.Searching)
                {
                    if (model.IncomeYear != null && model.IncomeYear != null)
                    {
                        list = list.Where(x => x.IncomeYear == model.IncomeYear).ToList();
                    }

                    if (!string.IsNullOrEmpty(model.AssessmentYear))
                    {
                        list = list.Where(x => x.AssessmentYear == model.AssessmentYear).ToList();
                    }
                    if (model.AssesseeTypeId > 0)
                    {
                        list = list.Where(x => x.AssesseeTypeId == model.AssesseeTypeId).ToList();
                    }
                }
            }

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

            if (request.SortingName == "AssesseeTypeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.AssesseeTypeId).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.AssesseeTypeId).ToList();
                }
            }

            if (request.SortingName == "NumberOfSlab")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.NumberOfSlab).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.NumberOfSlab).ToList();
                }
            }
            #endregion

            var typeList = ApplicableForList();

            foreach (var d in list)
            {
                string AssesseeType = string.Empty;
                foreach (var t in typeList)
                {
                    if (Convert.ToInt32(t.Value) == d.AssesseeTypeId)
                    {
                        AssesseeType = t.Text;
                    }
                }
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.IncomeYear,
                    d.AssessmentYear,
                    d.AssesseeTypeId,
                    AssesseeType,
                    d.NumberOfSlab,       
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
            PGM_TaxRateRule pgm_taxrate = _pgmCommonservice.PGMUnit.TaxRateMasterRepository.GetByID(id);
            return View(pgm_taxrate);
        }

        public ActionResult Create()
        {
            TaxRateModel model = new TaxRateModel();
            PrepareModel(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(TaxRateModel model)
        {
            try
            {
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {

                    var pgm_taxrate = model.ToEntity();
                    // add detail rebate rule
                    foreach (var c in model.TaxRateDetail)
                    {
                        pgm_taxrate.PGM_TaxRateRuleDetail.Add(new PGM_TaxRateRuleDetail
                        {
                            TaxRateId = pgm_taxrate.Id,
                            SlabNo = c.SlabNo,
                            LowerLimit = c.LowerLimit,
                            UpperLimit = c.UpperLimit,
                            TRate = c.TRate,
                        });
                    }
                    pgm_taxrate = GetInsertUserAuditInfo(pgm_taxrate);

                    errorList = GetBusinessLogicValidation(pgm_taxrate, model);
                    if (errorList.Count == 0)
                    {
                        _pgmCommonservice.PGMUnit.TaxRateMasterRepository.Add(pgm_taxrate);
                        _pgmCommonservice.PGMUnit.TaxRateMasterRepository.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = Common.ErrorListToString(errorList);

                }


            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
            }

            PrepareModel(model);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.TaxRateMasterRepository.GetByID(id, "Id");

            PGM_TaxRateRule pgm_taxrate = _pgmCommonservice.PGMUnit.TaxRateMasterRepository.GetByID(id);

            TaxRateModel model = pgm_taxrate.ToModel();

            if (pgm_taxrate.PGM_TaxRateRuleDetail != null)
            {
                model.TaxRateDetail = new Collection<TaxRateDetailModel>();
                foreach (var detail in pgm_taxrate.PGM_TaxRateRuleDetail)
                {
                    model.TaxRateDetail.Add(detail.ToModel());
                }

                PrepareModel(model);
                model.Mode = CrudeAction.Edit;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TaxRateModel model)
        {
            try
            {
                List<string> errorList = new List<string>();
                if (ModelState.IsValid)
                {

                    PGM_TaxRateRule pgm_taxrate = model.ToEntity();
                    ArrayList lstDetail = new ArrayList();

                    pgm_taxrate.EUser = User.Identity.Name;
                    pgm_taxrate.EDate = Common.CurrentDateTime;

                    if (model.TaxRateDetail != null)
                    {
                        foreach (var detail in model.TaxRateDetail)
                        {
                            PGM_TaxRateRuleDetail _detail = detail.ToEntity();
                            _detail.TaxRateId = pgm_taxrate.Id;

                            // if old item then reflection will retrive old IUser & IDate
                            _detail.IUser = User.Identity.Name;
                            _detail.IDate = DateTime.Now;
                            _detail.EDate = DateTime.Now;
                            _detail.EUser = User.Identity.Name;
                            lstDetail.Add(_detail);
                        }
                    }
                    Dictionary<Type, ArrayList> NavigationList = new Dictionary<Type, ArrayList>();
                    NavigationList.Add(typeof(PGM_TaxRateRuleDetail), lstDetail);
                    errorList = GetBusinessLogicValidation(pgm_taxrate, model);
                    if (errorList.Count == 0)
                    {
                        _pgmCommonservice.PGMUnit.TaxRateMasterRepository.Update(pgm_taxrate, NavigationList);
                        _pgmCommonservice.PGMUnit.TaxRateMasterRepository.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                if (errorList.Count() > 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = Common.ErrorListToString(errorList);
                }

            }
            catch (Exception ex)
            {
                model.IsError = 1;
                model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
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
                List<Type> allTypes = new List<Type> { typeof(PGM_TaxRateRuleDetail) };
                _pgmCommonservice.PGMUnit.TaxRateMasterRepository.Delete(id, allTypes);
                _pgmCommonservice.PGMUnit.TaxRateMasterRepository.SaveChanges();
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

        private PGM_TaxRateRule GetInsertUserAuditInfo(PGM_TaxRateRule taxrate)
        {
            taxrate.IUser = User.Identity.Name;
            taxrate.IDate = DateTime.Now;

            foreach (var child in taxrate.PGM_TaxRateRuleDetail)
            {
                child.IUser = User.Identity.Name;
                child.IDate = DateTime.Now;
            }
            return taxrate;
        }
        private PGM_TaxRateRule GetEditUserAuditInfo(PGM_TaxRateRule taxrate)
        {
            taxrate.EUser = User.Identity.Name;
            taxrate.EDate = DateTime.Now;

            foreach (var child in taxrate.PGM_TaxRateRuleDetail)
            {
                child.EUser = User.Identity.Name;
                child.EDate = DateTime.Now;
            }
            return taxrate;
        }

        private void PrepareModel(TaxRateModel model)
        {
            model.IncomeYearList = IncomeYearList();
            model.ApplicableForList = ApplicableForList();

        }

        public List<string> GetBusinessLogicValidation(PGM_TaxRateRule taxrate, TaxRateModel model)
        {
            List<string> errorMessage = new List<string>();

            if (taxrate.NumberOfSlab != model.TaxRateDetail.Count())
            {
                errorMessage.Add("Number of record in tax rate detail must be equal to number of slab.");
            }
            if (taxrate.NumberOfSlab < 0)
            {
                errorMessage.Add("Number Of slab must be greater than zero.");
            }

            decimal PreviousRate = 0;
            decimal PreviousHigherLimit = 0;
            int cnt = 0;

            foreach (var detail in model.TaxRateDetail)
            {
                cnt++;
                //in First slab- lower limit should be zero
                //Upper limit must be higher than lower limit
                if (cnt == 1 && detail.LowerLimit != 0)
                {
                    errorMessage.Add("In first slab, lower limit must be started from zero.");
                    break;
                }

                //Lower Limit must be started from previouse upper limit
                if (detail.LowerLimit != (PreviousHigherLimit + 1) && cnt != 1)
                {
                    errorMessage.Add("Lower limit must be started after higher limit of previous slab.");
                    break;
                }

                //Upper limit must be higher than lower limit
                if (detail.LowerLimit >= detail.UpperLimit)
                {
                    errorMessage.Add("Upper limit must be higher than the lower.");
                    break;
                }

                //Tax rate must be higher than previous record
                if (detail.TRate <= PreviousRate && cnt != 1)
                {
                    errorMessage.Add("Invalid tax rate!");
                    break;
                }
                PreviousRate = detail.TRate;
                PreviousHigherLimit = detail.UpperLimit;
            }

            return errorMessage;

        }

        private IList<SelectListItem> ApplicableForList()
        {
            IList<SelectListItem> SexList = new List<SelectListItem>();
            Dictionary<int, string> assesseeTypeList = Common.GetEnumAsDictionary<PGMEnum.TaxAssesseeType>();
            foreach (KeyValuePair<int, string> item in assesseeTypeList)
            {
                SexList.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }

            return SexList;
        }

        public ActionResult GetApplicableForList()
        {
            return PartialView("Select", ApplicableForList());
        }

        private IList<SelectListItem> IncomeYearList()
        {
            IList<SelectListItem> IncomeYear = new List<SelectListItem>();
            for (int i = Common.CurrentDateTime.Year + 1; i >= 2000; i--)
            {
                var iyFormat = (i - 1) + "-" + i;
                IncomeYear.Add(new SelectListItem() { Text = iyFormat, Value = iyFormat });
            }
            return IncomeYear;
        }
        public ActionResult GetIncomeYearList()
        {
            return PartialView("Select", IncomeYearList());
        }

        public ActionResult GetAssessmentYearList()
        {
            IList<SelectListItem> AssessmentYear = new List<SelectListItem>();
            for (int i = Common.CurrentDateTime.Year + 1; i >= 2000; i--)
            {
                var iyFormat = (i - 1) + "-" + i;
                AssessmentYear.Add(new SelectListItem() { Text = iyFormat, Value = iyFormat });
            }
            return PartialView("Select", AssessmentYear);
        }

        #endregion Others
    }
}
