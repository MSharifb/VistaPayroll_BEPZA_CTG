using Domain.PGM;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Areas.PGM.Models.SalaryHeadCOASubledgerMapping;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class SalaryHeadCOASubledgerMappingController : BaseController
    {
        // GET: PGM/SalaryHeadCOASubledgerMapping
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor

        public SalaryHeadCOASubledgerMappingController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonService = pgmCommonservice;
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, SalaryHeadCOASubledgerMappingViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<SalaryHeadCOASubledgerMappingViewModel> list = (from SM in _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.GetAll()
                                           join COA in _pgmCommonService.PGMUnit.AccChartOfAccountRepository.GetAll() on SM.COAId equals COA.id
                                           into gj
                                           from subpet in gj.DefaultIfEmpty()
                                           join SL in _pgmCommonService.PGMUnit.AccSubLedger.GetAll() on SM.SubledgerId equals SL.id
                                           into SLJ
                                           from SLpet in SLJ.DefaultIfEmpty()
                                           join SH in _pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll() on SM.SalaryHeadId equals SH.Id
                                           where (SM.ZoneInfoId == LoggedUserZoneInfoId) && (model.SalaryHeadId == 0 || model.SalaryHeadId == SH.Id)
                                           select new SalaryHeadCOASubledgerMappingViewModel()
                                           {
                                               Id = SM.Id,
                                               SalaryHeadId = SH.Id,
                                               SalaryHead = SH == null ?string.Empty : SH.HeadName,
                                               COA = subpet == null ?string.Empty : subpet.accountName,
                                               SubLedger = SLpet == null ? string.Empty : SLpet.name
                                           }).ToList();

            totalRecords = list == null ? 0 : list.Count;

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
                    d.SalaryHead,
                    d.COA,
                    d.SubLedger,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ViewResult Index()
        {
            var model = new SalaryHeadCOASubledgerMappingViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new SalaryHeadCOASubledgerMappingViewModel();
            PopulateDropdown(model);
            return View(model);
        }
        [HttpPost]
        [NoCache]
        public ActionResult Create(SalaryHeadCOASubledgerMappingViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.errClass = "failed";

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                var entity = model.ToEntity();
                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;
                entity.ZoneInfoId = LoggedUserZoneInfoId;
                try
                {
                    _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.Add(entity);
                    _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    return View("Index", model);
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }
            return View("Index", model);
        }
        public ActionResult Edit(int Id)
        {
            var model = _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.GetByID(Id).ToModel();
            PopulateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(SalaryHeadCOASubledgerMappingViewModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.ZoneInfoId = LoggedUserZoneInfoId;

                    _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.Update(entity);
                    _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);

                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.ErrMsg = errorList;
            }
            PopulateDropdown(model);
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result;
            string errMsg = "Error while deleting data!";

            try
            {
                _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.Delete(id);
                _pgmCommonService.PGMUnit.SalaryHeadCOASubledgerMappingRepository.SaveChanges();
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
                Message = result ? "Information has been deleted successfully." : errMsg
            });
        }

        [NoCache]
        private void PopulateDropdown(SalaryHeadCOASubledgerMappingViewModel model)
        {
            model.COAList = _pgmCommonService.PGMUnit.AccChartOfAccountRepository.GetAll()
                                   .Join(_pgmCommonService.PGMUnit.AccFundControlInfoRepository.GetAll(), c => c.fundControlId, f => f.id, (c, f) => new { c, f })
                                   .Where(m => m.c.isControlhead == 0).ToList()
              .Select(y =>
              new SelectListItem()
              {
                  Text = y.c.accountName + " (" + y.f.fundControlName + ")",
                  Value = y.c.id.ToString()
              }).OrderBy(m => m.Text).ToList();

            model.SubledgerList = _pgmCommonService.PGMUnit.AccSubLedger.GetAll()
                                  .Select(y =>
                                  new SelectListItem()
                                  {
                                      Text = y.name,
                                      Value = y.id.ToString()
                                  }).OrderBy(m => m.Text).ToList();

            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(_pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll());
        }

        [NoCache]
        public ActionResult GetSalaryHeadList()
        {
            var model = new SalaryHeadCOASubledgerMappingViewModel();
            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(_pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll());
            return PartialView("_Select", model.SalaryHeadList);
        }

    }
}