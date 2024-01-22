using Domain.PGM;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Areas.PGM.Models.ExcludeEmpFromReport;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class ExcludeEmpFromReportController : BaseController
    {
        // GET: PGM/ExcludeEmpFromReport
        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor
        public ExcludeEmpFromReportController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonService = pgmCommonservice;
        }
        #endregion

        #region Action
        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, ExcludeEmpFromReportModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<ExcludeEmpFromReportModel> list = (from SM in _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.GetAll()
                                                    select new ExcludeEmpFromReportModel()
                                                    {
                                                        Id = SM.Id,
                                                        SalaryHead = SM.PRM_SalaryHead.HeadName
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
                    d.SalaryHead,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ViewResult Index()
        {
            var model = new ExcludeEmpFromReportModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new ExcludeEmpFromReportModel();
            PopulateDropdown(model);
            return View(model);
        }
        [HttpPost]
        [NoCache]
        public ActionResult Create(ExcludeEmpFromReportModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            model.errClass = "failed";

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                foreach (var item in model.SelectedEmployees)
                {
                    if (string.IsNullOrEmpty(model.Employee))
                        model.Employee += item;
                    else
                        model.Employee += "," + item;
                }
                var entity = model.ToEntity();
                entity.IUser = User.Identity.Name;
                entity.IDate = Common.CurrentDateTime;
                entity.ZoneInfoId = LoggedUserZoneInfoId;
                try
                {

                    _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.Add(entity);
                    _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.SaveChanges();

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
            var model = _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.GetByID(Id).ToModel();
            PopulateDropdown(model);
            var empId = model.Employee.Split(',').ToList();
            model.SelectedEmployees = empId;
            model.strMode = "Edit";
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(ExcludeEmpFromReportModel model)
        {
            string errorList = string.Empty;
            string Message = string.Empty;
            model.IsError = 1;

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    foreach (var item in model.SelectedEmployees)
                    {
                        if(string.IsNullOrEmpty(model.Employee))
                            model.Employee += item;
                        else
                            model.Employee += ","+item;
                    }
                    var entity = model.ToEntity();
                    entity.ZoneInfoId = LoggedUserZoneInfoId;

                    _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.Update(entity);
                    _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.SaveChanges();

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
                _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.Delete(id);
                _pgmCommonService.PGMUnit.ExcludeEmpFromSalaryHeadRepository.SaveChanges();
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
        #endregion

        [NoCache]
        private void PopulateDropdown(ExcludeEmpFromReportModel model)
        {
            model.EmployeeList = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList(LoggedUserZoneInfoId)
                                   .ToList()
              .Select(y =>
              new SelectListItem()
              {
                  Text = y.FullName + " [" + y.EmpID + "]",
                  Value = y.Id.ToString()
              }).OrderBy(m => m.Text).ToList();
            model.SalaryHeadList = Common.PopulateSalaryHeadDDL(_pgmCommonService.PGMUnit.SalaryHeadRepository.GetAll());
        }

    }
}