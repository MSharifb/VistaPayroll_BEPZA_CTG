using Domain.PGM;
using PGM.Web.Areas.PGM.Models.ELMAHError;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class ELMAHErrorController : BaseController
    {
        private readonly PGMCommonService _pgmCommonservice;

        public ELMAHErrorController(PGMCommonService pgmCommonService)
        {
            this._pgmCommonservice = pgmCommonService;
        }

        public ActionResult Index()
        {
            var model = new ELMAHErrorViewModel();
            model.TimeUtc = DateTime.Now.ToString("yy-mm-dd");
            return View(model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, ELMAHErrorViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;
            DateTime filterDate = DateTime.Now;
            if (!String.IsNullOrEmpty(model.TimeUtc) && Convert.ToDateTime(model.TimeUtc) != DateTime.MinValue)
            {
                filterDate = Convert.ToDateTime(model.TimeUtc);
            }

            var list = _pgmCommonservice.PGMUnit.FunctionRepository.GetElmahErrorList(filterDate)
                .Select(e => new ELMAHErrorViewModel()
                {
                    ErrorId = e.ErrorId,
                    Application = e.Application,
                    Host = e.Host,
                    Type = e.Type,
                    Source = e.Source,
                    Message = e.Message,
                    TimeUtc = e.TimeUtc,
                    Sequence = e.Sequence,
                }).ToList();

            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.Application) && model.Application != "0")
                {
                    list = list.Where(e => e.Application == model.Application).ToList();
                }
                if (!String.IsNullOrEmpty(model.Host) && model.Host != "0")
                {
                    list = list.Where(e => e.Host == model.Host).ToList();
                }
                if (!String.IsNullOrEmpty(model.Type) && model.Type != "0")
                {
                    list = list.Where(e => e.Type == model.Type).ToList();
                }
                if (!String.IsNullOrEmpty(model.Source) && model.Source != "0")
                {
                    list = list.Where(e => e.Source == model.Source).ToList();
                }
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

            //String sortingName = request.SortingName;

            //if (request.SortingName == "ErrorId")
            //{
            //if (request.SortingOrder.ToString().ToLower() == "asc")
            //{   
            //    list = (from l in list orderby (sortingName) select l).ToList();
            //}
            //else
            //{
            //    list = (from l in list orderby (sortingName) descending select l).ToList();
            //}
            //}

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.ErrorId), new List<object>()
                {
                    d.ErrorId,

                    d.Application,
                    d.Host,

                    d.Type,
                    d.Source,
                    d.Message,
                    d.TimeUtc,
                    d.Sequence,
                    
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }


        public ActionResult GetApplicationSearch()
        {
            var list = _pgmCommonservice.PGMUnit.FunctionRepository.GetElmahSearchData("Application").ToList()
                .Select(e => new
                {
                    Text = e.Item,
                    Value = e.Item
                });

            return PartialView("_Select", Common.PopulateDDL(list));
        }

        public ActionResult GetHostSearch()
        {
            var list = _pgmCommonservice.PGMUnit.FunctionRepository.GetElmahSearchData("Host").ToList()
                .Select(e => new
                {
                    Text = e.Item,
                    Value = e.Item
                });

            return PartialView("_Select", Common.PopulateDDL(list));
        }

        public ActionResult GetTypeSearch()
        {
            var list = _pgmCommonservice.PGMUnit.FunctionRepository.GetElmahSearchData("Type").ToList()
                .Select(e => new
                {
                    Text = e.Item,
                    Value = e.Item
                });

            return PartialView("_Select", Common.PopulateDDL(list));
        }

        public ActionResult GetSourceSearch()
        {
            var list = _pgmCommonservice.PGMUnit.FunctionRepository.GetElmahSearchData("Source").ToList()
                .Select(e => new
                {
                    Text = e.Item,
                    Value = e.Item
                });

            return PartialView("_Select", Common.PopulateDDL(list));
        }

        public ActionResult Delete(string ErrorId)
        {
            throw new NotImplementedException();
        }

        public ActionResult Edit(string id)
        {
            throw new NotImplementedException();
        }
    }
}