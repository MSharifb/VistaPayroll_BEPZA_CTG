using Domain.PGM;
using PGM.Web.Areas.PGM.Models.BonusType;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class BonusTypeController : Controller
    {
        private readonly PGMCommonService _pgmCommonservice;
        private static string message;

        public BonusTypeController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonservice = pgmCommonservice;
        }

        [NoCache]
        public ActionResult Index(string message, bool? isSuccess)
        {
            BonusTypeViewModel viewModel = new BonusTypeViewModel();
            PopulateDropdown(viewModel);
            if (!string.IsNullOrEmpty(message))
            {
                viewModel.IsSuccessful = (bool)isSuccess;
                viewModel.Message = message;
            }

            return View("BonusTypeIndex", viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public JsonResult GetBonusTypeData(JqGridRequest request)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmCommonservice.GetBonusTypeList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, false).ToList();

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

            if (request.SortingName == "BonusType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BonusType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BonusType).ToList();
                }
            }

            if (request.SortingName == "Religion")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Religion).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Religion).ToList();
                }
            }

            if (request.SortingName == "IsTaxable")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsTaxable).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsTaxable).ToList();
                }
            }

            #endregion

            totalRecords = _pgmCommonservice.GetBonusTypeList("", request.SortingName, request.SortingOrder.ToString(), request.PageIndex, request.RecordsCount, request.PagesCount.HasValue ? request.PagesCount.Value : 1, true).Count();

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.BonusType,
                    d.Religion,
                    d.IsTaxable==true?"Yes":"No",
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };

        }

        [NoCache]
        public ActionResult Create()
        {
            BonusTypeViewModel viewModel = new BonusTypeViewModel();
            viewModel.Mode = CrudeAction.Create;
            return View("BonusTypeIndex", viewModel);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(BonusTypeViewModel viewModel)
        {
            int success = 1;
            try
            {
                if (ModelState.IsValid)
                {
                    var entity = viewModel.ToEntity();
                    entity.IDate = DateTime.Now;
                    entity.IUser = User.Identity.Name;
                    _pgmCommonservice.PGMUnit.BonusTypeRepository.Add(entity);
                    _pgmCommonservice.PGMUnit.BonusTypeRepository.SaveChanges();
                    viewModel.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
            }
            catch (Exception ex)
            {
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                success = 0;
            }

            return Json(new
            {
                Success = success,
                Message = viewModel.Message
            });
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var data = _pgmCommonservice.PGMUnit.BonusTypeRepository.GetByID(id);
            var model = data.ToModel();
            PopulateDropdown(model);
            model.Mode = CrudeAction.Edit;
            return View("BonusTypeIndex", model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(BonusTypeViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.Id != 0)
                    {
                        var entity = viewModel.ToEntity();
                        _pgmCommonservice.PGMUnit.BonusTypeRepository.Update(entity);
                        _pgmCommonservice.PGMUnit.BonusTypeRepository.SaveChanges();
                        viewModel.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        viewModel.IsSuccessful = true;

                        return Json(new
                        {
                            Success = viewModel.IsSuccessful,
                            Message = viewModel.Message
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                viewModel.IsSuccessful = false;
                viewModel.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);

                return Json(new
                {
                    Success = viewModel.IsSuccessful,
                    Message = viewModel.Message
                });
            }
            return Json(new
            {
                Success = viewModel.IsSuccessful,
                Message = viewModel.Message
            });
        }

        [NoCache]
        public JsonResult Delete(int id)
        {
            bool isSuccessful = false;
            try
            {
                if (id != 0)
                {
                    _pgmCommonservice.PGMUnit.BonusTypeRepository.Delete(id);
                    _pgmCommonservice.PGMUnit.BonusTypeRepository.SaveChanges();
                    message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                    isSuccessful = true;
                    return Json(new
                    {
                        Success = isSuccessful,
                        Message = message
                    });

                }
            }
            catch (Exception ex)
            {
                isSuccessful = false;
                message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);

                return Json(new
                {
                    Success = isSuccessful,
                    Message = message
                });
            }
            return Json(new
            {
                Success = isSuccessful,
                Message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful)


            }, JsonRequestBehavior.AllowGet);
        }

        [NoCache]
        private void PopulateDropdown(BonusTypeViewModel viewModel)
        {
            dynamic ddlList;
            ddlList = _pgmCommonservice.PGMUnit.Religion.GetAll().OrderBy(x => x.Name);
            viewModel.ReligionList = Common.PopulateDDLList(ddlList);
        }

    }
}
