using DAL.PGM;
using Domain.PGM;
using Lib.Web.Mvc;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Areas.PGM.Models.SalaryAutoIncrement;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class SalaryAutoIncrementController : BaseController
    {
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        private readonly PGMMonthlySalaryService _monthlySalaryService;
        private readonly PGMEntities _pgmContext;
        #endregion

        #region Ctor

        public SalaryAutoIncrementController(PGMCommonService pgmCommonservice, PGMMonthlySalaryService monthlySalaryService, PGMEntities context)
        {
            this._pgmCommonService = pgmCommonservice;
            this._monthlySalaryService = monthlySalaryService;
            this._pgmContext = context;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        // GET: PGM/SalaryAutoIncrement
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetList(JqGridRequest request, SalaryAutoIncrementViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<SalaryAutoIncrementViewModel> list = (from tr in _pgmCommonService.PGMUnit.SalaryBeforeIncrementRepository.GetAll()
                                                       where (tr.ZoneInfoId == LoggedUserZoneInfoId)
                                                       select new SalaryAutoIncrementViewModel()
                                                       {
                                                           ProcessDate = tr.ProcessDate
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
                    Convert.ToDateTime(d.ProcessDate).ToString("yyyy-MM-dd")
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        public ActionResult Index()
        {
            var model = new SalaryAutoIncrementViewModel();
            return View(model);
        }
        public ActionResult CreateOrEdit()
        {
            SalaryAutoIncrementViewModel model = new SalaryAutoIncrementViewModel();
            PopulateDropdown(model);
            model.IsError = 0;

            if (DateTime.Now.Month != 7)
            {
                model.IsError = 1;
                model.ErrMsg = "Auto increment process is only valid for month of july.";
            }
            else
            {
                var juneSalary = _monthlySalaryService.GetSalaryInfoList(""
                                            , string.Empty
                                            , string.Empty
                                            , 0
                                            , 0
                                            , 1
                                            , true
                                            , DateTime.Now.Year.ToString()
                                            , "June"
                                            , LoggedUserZoneInfoId)
                                    .ToList();

                juneSalary = juneSalary.Where(x => x.IsConfirmed).ToList();
                if (juneSalary.Count == 0)
                {
                    model.IsError = 1;
                    model.ErrMsg = "Please confirm june salary first!";
                }
                else
                {
                    model.IsError = 0;
                }
            }
            return PartialView("_CreateOrEdit", model);
        }

        public JsonResult SalaryAutoIncrementProcess(string financialYear)
        {
            int result = 0;
            List<string> Message = new List<string>();
            bool Success = true;
            List<string> errorList = new List<string>();
            var messageToReturn = String.Empty;

            try
            {
                // Data validation
                //errorList = GetBusinessLogicValidation(year, month, employeeId_pk);

                // Process
                if ((ModelState.IsValid) && (errorList.Count == 0))
                {
                    int totalProcessed = 0;
                    result = _pgmCommonService.PGMUnit.FunctionRepository.SalaryAutoIncrementProcess(Convert.ToDateTime(financialYear), LoggedUserZoneInfoId, User.Identity.Name, out totalProcessed);

                    if (result == 0)
                    {
                        messageToReturn = "Salary Auto Increment Process has been completed successfully." + Environment.NewLine + "Total Processed: " + totalProcessed;
                    }
                    else
                    {
                        Success = false;
                        Message.Add("Salary Auto Increment Process is failed!");
                    }
                }
                else
                {
                    Success = false;
                    foreach (var msg in errorList)
                    {
                        Message.Add(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Message.Add(CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save));
                Success = false;
            }

            if (!Success) messageToReturn = Common.ErrorListToString(Message);

            return Json(new
            {
                Success = Success,
                Message = messageToReturn
            });
        }

        private void PopulateDropdown(SalaryAutoIncrementViewModel model)
        {
            //------------
        }
    }
}