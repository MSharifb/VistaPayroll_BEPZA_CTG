using Domain.PGM;
using PGM.Web.Areas.PGM.Models.CustomProperties;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class CustomPropertiesController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor
        public CustomPropertiesController(PGMCommonService pgmCommonService)
        {
            this._pgmCommonservice = pgmCommonService;
        }
        #endregion

        #region Action

        public ActionResult Index()
        {
            var model = new CustomPropertiesModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, CustomPropertiesModel model)
        {   
            int totalRecords = 0;

            var list = (from a in _pgmCommonservice.PGMUnit.CustomPropertyAttributeRepository.GetAll()
                        select new CustomPropertiesModel
                        {
                            Id = a.Id,
                            ModelName = a.ModelName,
                            PropertyName = a.PropertyName,
                            DisplayText = a.DisplayText,
                            IsRequired = Convert.ToBoolean(a.IsRequired)
                        }).OrderBy(o => o.ModelName).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.ModelName))
                {
                    list = list.Where(t => t.ModelName == model.ModelName).ToList();
                }

                if (!String.IsNullOrEmpty(model.PropertyName))
                {
                    list = list.Where(t => t.PropertyName == model.PropertyName).ToList();
                }

                if (!String.IsNullOrEmpty(model.DisplayText))
                {
                    list = list.Where(t => t.DisplayText == model.DisplayText).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "ModelName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ModelName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ModelName).ToList();
                }
            }

            if (request.SortingName == "PropertyName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PropertyName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PropertyName).ToList();
                }
            }

            if (request.SortingName == "DisplayText")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DisplayText).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DisplayText).ToList();
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
                    d.ModelName,
                    d.PropertyName,
                    d.DisplayText,
                    d.IsRequired
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Create()
        {
            var model = new CustomPropertiesModel();
            model.strMode = "Create";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(CustomPropertiesModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();
                    _pgmCommonservice.PGMUnit.CustomPropertyAttributeRepository.Add(entity);
                    _pgmCommonservice.PGMUnit.CustomPropertyAttributeRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                model.Message = Common.GetModelStateError(ModelState);
            }

            model.strMode = "Create";

            return View(model);
        }

        public ActionResult Edit(int Id)
        {
            var model = _pgmCommonservice.PGMUnit.CustomPropertyAttributeRepository.Get(c=> c.Id == Id).FirstOrDefault().ToModel();
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(CustomPropertiesModel model)
        {

            string Message = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = model.ToEntity();

                    _pgmCommonservice.PGMUnit.CustomPropertyAttributeRepository.Update(entity);
                    _pgmCommonservice.PGMUnit.CustomPropertyAttributeRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                model.Message = Common.GetModelStateError(ModelState);
            }

            model.strMode = "Edit";
            return View(model);
        }

        #endregion

    }
}