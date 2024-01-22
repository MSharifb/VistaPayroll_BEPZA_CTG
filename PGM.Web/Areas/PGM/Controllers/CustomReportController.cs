using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.PGM;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Areas.PGM.Models.CustomReport;
using PGM.Web.Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class CustomReportController : Controller
    {

        #region Fields
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor
        public CustomReportController(PGMCommonService pgmCommonService)
        {
            this._pgmCommonservice = pgmCommonService;
        }
        #endregion

        public ActionResult Index()
        {
            var model = new CustomReportModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, CustomReportModel model)
        {
            int totalRecords = 0;

            var list = (from a in _pgmCommonservice.PGMUnit.CustomReportBackOfficeRepository.GetAll()
                        where a.AppModule == "PGM"
                        select new CustomReportModel
                        {
                            Id = a.Id,
                            ReportName = a.ReportName,
                            ReportParameterNameHints = a.ReportParameterNameHints,
                            ReportParameterDefaultValue = a.ReportParameterDefaultValue
                        }).OrderBy(o => o.ReportName).ToList();

            #region Searching
            if (request.Searching)
            {
                if (!String.IsNullOrEmpty(model.ReportName))
                {
                    list = list.Where(t => t.ReportName == model.ReportName).ToList();
                }

                if (!String.IsNullOrEmpty(model.ReportParameterNameHints))
                {
                    list = list.Where(t => t.ReportParameterNameHints == model.ReportParameterNameHints).ToList();
                }

                if (!String.IsNullOrEmpty(model.ReportParameterDefaultValue))
                {
                    list = list.Where(t => t.ReportParameterDefaultValue == model.ReportParameterDefaultValue).ToList();
                }
            }
            #endregion

            totalRecords = list == null ? 0 : list.Count;

            #region sorting

            if (request.SortingName == "ReportName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.ReportName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.ReportName).ToList();
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
                string defaultValue = String.Empty;

                // Getting cookie key
                var cookieKey = String.Concat(d.Id.ToString(), d.ReportName.Replace(" ", ""));

                // Get if exists
                if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(cookieKey))
                {
                    defaultValue = this.ControllerContext.HttpContext.Request.Cookies[cookieKey].Value;
                }
                else
                {
                    defaultValue = d.ReportParameterDefaultValue;
                }


                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.ReportName,
                    d.ReportParameterNameHints,
                    defaultValue
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult ViewReport(int Id)
        {
            var model = _pgmCommonservice.PGMUnit.CustomReportBackOfficeRepository.Get(c => c.Id == Id).FirstOrDefault().ToModel();

            // Getting cookie key
            var cookieKey = String.Concat(model.Id.ToString(), model.ReportName.Replace(" ", ""));

            // Get if exists
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(cookieKey))
            {
                model.ReportParameterDefaultValue = this.ControllerContext.HttpContext.Request.Cookies[cookieKey].Value;
            }

            model.ReportDataTable = _pgmCommonservice.PGMUnit.FunctionRepository.GetCustomReportData(Id, model.ReportParameterDefaultValue);

            return View(model);
        }

        [NoCache]
        [HttpPost]
        public void EditParameterValue(CustomReportModel model)
        {
            try
            {
                var customReport = _pgmCommonservice.PGMUnit.CustomReportBackOfficeRepository.Get(d => d.Id == model.Id).FirstOrDefault();

                // Getting cookie key
                var cookieKey = String.Concat(customReport.Id.ToString(), customReport.ReportName.Replace(" ", ""));

                // Remove if exists
                if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains(cookieKey))
                {
                    HttpCookie cookieToRemove = this.ControllerContext.HttpContext.Request.Cookies[cookieKey];
                    cookieToRemove.Expires = DateTime.Now.AddDays(-1);
                    this.ControllerContext.HttpContext.Response.Cookies.Add(cookieToRemove);
                }

                // Add
                HttpCookie myCookie = new HttpCookie(cookieKey);
                myCookie.Value = model.ReportParameterDefaultValue;
                myCookie.Expires = DateTime.Now.AddMonths(24);
                this.ControllerContext.HttpContext.Response.Cookies.Add(myCookie);
            }
            catch (Exception ex)
            {
                CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }
        }
    }
}