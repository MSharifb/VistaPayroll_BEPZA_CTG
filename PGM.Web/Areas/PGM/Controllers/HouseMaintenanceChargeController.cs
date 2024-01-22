using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.HouseMaintenanceCharge;
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
    public class HouseMaintenanceChargeController : Controller
    {
        private readonly PGMCommonService _pgmCommonservice;

        public HouseMaintenanceChargeController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonservice = pgmCommonservice;
        }

        // GET: PGM/HouseMaintenanceCharge
        public ActionResult Index()
        {
            var model = new HouseMaintenanceChargeViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, HouseMaintenanceChargeViewModel model)
        {

            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<HouseMaintenanceChargeViewModel> list = (from HM in _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.GetAll()
                                                          join SH in _pgmCommonservice.PGMUnit.SalaryHeadRepository.GetAll() on HM.SalaryHeadId equals SH.Id
                                                          select new HouseMaintenanceChargeViewModel
                                                          {
                                                              Id = HM.Id,
                                                              SalaryHeadId = HM.SalaryHeadId,
                                                              SalaryHeadName = SH.HeadName,
                                                              EffectiveDate = HM.EffectiveDate
                                                          }
                                                          ).OrderBy(x => x.Id).ToList();
            if (request.Searching)
            {

                if ((model.EffectiveDate != null && model.EffectiveDate != DateTime.MinValue))
                {
                    list = list.Where(d => d.EffectiveDate == model.EffectiveDate).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "EffectiveDate")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EffectiveDate).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EffectiveDate).ToList();
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
                    d.SalaryHeadId,
                    d.SalaryHeadName,
                    d.EffectiveDate.ToString("dd/MM/yyyy"),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new HouseMaintenanceChargeViewModel();
            model.EffectiveDate = DateTime.Now;
            populateDropdown(model);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(HouseMaintenanceChargeViewModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)) && ValidateBusiness(model, out errorList))
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.PGM_HouseMaintenanceChargeDetail = null;
                    if (entity.Id > 0)
                    {
                        _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.Update(entity);
                    }
                    else
                        _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.Add(entity);

                    _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.SaveChanges();

                    foreach (var item in model.HouseMaintenanceDetail.Select(x => x.ToEntity()))
                    {
                        if (item.Id > 0)
                        {
                            var maintenanceDetail = _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.GetByID(item.Id);
                            maintenanceDetail.FromGradeId = item.FromGradeId;
                            maintenanceDetail.ToGradeId = item.ToGradeId;
                            maintenanceDetail.AmountPercent = item.AmountPercent;
                            _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.Update(item);

                        }
                        else
                        {
                            item.HouseMaintenanceId = entity.Id;
                            _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.Add(item);
                        }
                    }
                    _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                Message = errorList;
                model.ErrMsg = Message;
                model.IsError = 1;
            }
            if (model.HouseMaintenanceDetail != null)
            {
                var ChModel = model.HouseMaintenanceDetail.FirstOrDefault();
                populateChildDropdown(ChModel);
                foreach (var item in model.HouseMaintenanceDetail)
                {
                    item.GradeList = ChModel.GradeList;
                }
            }

            populateDropdown(model);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        private bool ValidateBusiness(HouseMaintenanceChargeViewModel model, out string errorList)
        {
            var GradeList = _pgmCommonservice.GetLatestJobGrade();
            foreach (var item in model.HouseMaintenanceDetail)
            {
                var douplicate = model.HouseMaintenanceDetail.Where(e => e.FromGradeId == item.FromGradeId || e.ToGradeId == item.ToGradeId).ToList();
                if (douplicate != null)
                    if (douplicate.Count() > 1)
                    {
                        errorList = "Grade Range Douplicate Assign!";
                        return false;
                    }
                    else
                    {
                        foreach (var innerRange in model.HouseMaintenanceDetail)
                        {
                            var innerRangeToGrade = Convert.ToInt32(GradeList.Where(e => e.Id == innerRange.ToGradeId).FirstOrDefault().GradeName);
                            var innerRangeFromGrade = Convert.ToInt32(GradeList.Where(e => e.Id == innerRange.FromGradeId).FirstOrDefault().GradeName);
                            var outerRangeToGrade = Convert.ToInt32(GradeList.Where(e => e.Id == item.ToGradeId).FirstOrDefault().GradeName);
                            var outerRangeFromGrade = Convert.ToInt32(GradeList.Where(e => e.Id == item.ToGradeId).FirstOrDefault().GradeName);

                            if (innerRangeFromGrade <= outerRangeToGrade && outerRangeFromGrade < innerRangeToGrade)
                            {
                                errorList = "Grade Range Overlapping";
                                return false;
                            }
                        }
                    }
            }
            errorList = "";
            return true;
        }

        public ActionResult Edit(int? Id)
        {
            var model = _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.GetByID(Id).ToModel();
            if (model != null)
            {
                var list = _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.GetAll().Where(e => e.HouseMaintenanceId == Id).ToList().Select(x => x.ToModel());
                if (list != null)
                {
                    var ChModel = list.FirstOrDefault();
                    populateChildDropdown(ChModel);
                    foreach (var item in list)
                    {
                        item.GradeList = ChModel.GradeList;
                        model.HouseMaintenanceDetail.Add(item);
                    }

                }
            }
            populateDropdown(model);
            model.Mode = CrudeAction.Edit;
            return View("Create", model);
        }

        public PartialViewResult AddDetail(int? id)
        {
            HouseMaintenanceChargeDetailViewModel model = new HouseMaintenanceChargeDetailViewModel();
            populateChildDropdown(model);
            return PartialView("_Detail", model);
        }

        [HttpPost]
        public ActionResult DeleteDetail(Int32 Id)
        {
            var entity = _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.GetByID(Id);

            try
            {
                if (entity != null)
                {
                    _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.Delete(Id);
                    _pgmCommonservice.PGMUnit.HouseMaintenanceChargeDetailsRepository.SaveChanges();
                }
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

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.GetByID(id);

            try
            {
                List<Type> allTypes = new List<Type> { typeof(PGM_HouseMaintenanceChargeDetail) };

                _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.Delete(entity.Id, allTypes);
                _pgmCommonservice.PGMUnit.HouseMaintenanceChargeMasterRepository.SaveChanges();

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


        private void populateDropdown(HouseMaintenanceChargeViewModel model)
        {
            #region Head
            var List = _pgmCommonservice.PGMUnit.SalaryHeadRepository.GetAll().OrderBy(x => x.HeadName).ToList();
            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(List);
            #endregion

        }
        private void populateChildDropdown(HouseMaintenanceChargeDetailViewModel model)
        {
            #region Grade List
            var List = _pgmCommonservice.GetLatestJobGrade();
            model.GradeList = Common.PopulateJobGradeDDL(List);
            #endregion

        }

    }
}