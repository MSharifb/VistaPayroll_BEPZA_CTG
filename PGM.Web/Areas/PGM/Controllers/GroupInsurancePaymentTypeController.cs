using Domain.PGM;
using PGM.Web.Areas.PGM.Models.GroupInsurancePaymentType;
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
    public class GroupInsurancePaymentTypeController : Controller
    {
        private readonly PGMCommonService _pgmCommonservice;

        public GroupInsurancePaymentTypeController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonservice = pgmCommonservice;
        }

        // GET: PGM/GroupInsurancePaymentType
        public ActionResult Index()
        {
            var model = new GroupInsurancePaymentTypeViewModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, GroupInsurancePaymentTypeViewModel model)
        {

            string filterExpression = String.Empty;
            int totalRecords = 0;
            List<GroupInsurancePaymentTypeViewModel> list = (from HM in _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.GetAll()
                                                             select new GroupInsurancePaymentTypeViewModel
                                                             {
                                                                 Id = HM.Id,
                                                                 OrderNo = HM.OrderNo,
                                                                 PaymentType = HM.PaymentType,
                                                                 PaymentAmount = HM.PaymentAmount,
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

            if (request.SortingName == "PaymentType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PaymentType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PaymentType).ToList();
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
                    d.PaymentType,
                    d.OrderNo,
                    d.PaymentAmount,
                    d.EffectiveDate.ToString("dd/MM/yyyy"),
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new GroupInsurancePaymentTypeViewModel();
            model.EffectiveDate = DateTime.Now;
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(GroupInsurancePaymentTypeViewModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    var entity = model.ToEntity();

                    if (entity.Id > 0)
                        _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.Update(entity);
                    else
                        _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.Add(entity);

                    _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                Message = errorList;
            }

            model.ErrMsg = Message;
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        public ActionResult Edit(int? Id)
        {
            var model = _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.GetByID(Id).ToModel();
            model.Mode = CrudeAction.Edit;
            return View("Create", model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.GetByID(id);

            try
            {
                _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.Delete(entity.Id);
                _pgmCommonservice.PGMUnit.GroupInsurancePaymentTypeRepository.SaveChanges();

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

    }
}