using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.BankAdviceLetterTemplate;
using PGM.Web.Controllers;
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
    [NoCache]
    public class BankAdviceLetterTemplateController : BaseController
    {

        #region Fields
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor

        public BankAdviceLetterTemplateController(PGMCommonService pgmCommonservice)
        {
            this._pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion

        #region Actions
        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, BankAdviceLetterTemplateViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = (from BA in _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.GetAll()
                        join B in _pgmCommonService.PGMUnit.AccBankRepository.GetAll() on BA.BankId equals B.id
                        where (BA.ZoneInfoId == LoggedUserZoneInfoId)
                        select new BankAdviceLetterTemplateViewModel()
                        {
                            Id = BA.Id,
                            LetterType = BA.LetterType,
                            LetterSubject = BA.LetterSubject,
                            BankName = B.bankName
                        }).ToList();

            if (request.Searching)
            {
                if (!string.IsNullOrEmpty(model.LetterType))
                {
                    list = list.Where(x => x.LetterType.Trim().ToLower().Contains(model.LetterType.Trim().ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(model.BankName))
                {
                    list = list.Where(x => x.BankName.Trim().ToLower().Contains(model.BankName.Trim().ToLower())).ToList();
                }
            }

            totalRecords = list == null ? 0 : list.Count;

            #region Sorting

            if (request.SortingName == "LetterType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.LetterType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.LetterType).ToList();
                }
            }

            if (request.SortingName == "BankName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.BankName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.BankName).ToList();
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
                    d.LetterType,
                    d.LetterSubject,
                    d.BankName,
                    "Delete"
                }));
            }
            //Return data as json
            return new JqGridJsonResult() { Data = response };
        }
        [NoCache]
        public ActionResult Index()
        {
            var model = new BankAdviceLetterTemplateViewModel();
            return View(model);
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new BankAdviceLetterTemplateViewModel();
            List<PGM_BankAdviceLetterVariables> itemVariables = _pgmCommonService.PGMUnit.BankAdviceLetterVariablesRepository.GetAll().OrderBy(x => x.VariableNames).ToList();

            if (itemVariables.Count > 0)
            {
                foreach (var item in itemVariables)
                {
                    BankAdviceLetterBodyVariable details = new BankAdviceLetterBodyVariable();
                    details.Id = item.Id;
                    details.VariableNames = item.VariableNames;
                    model.BankLetterBodyVariableList.Add(details);
                }
            }
            PopulateDropdown(model);
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(BankAdviceLetterTemplateViewModel model)
        {
            Message = CheckBusinessRule(model);
            model.IsError = 1;
            if (ModelState.IsValid && string.IsNullOrEmpty(Message))
            {
                try
                {
                    if (model.BankId == 0 || model.BankId == null)
                    {
                        model.BankList = _pgmCommonService.PGMUnit.AccBankRepository.GetAll().OrderBy(x => x.bankName)
                                        .Select(y => new SelectListItem()
                                        {
                                            Text = y.bankName,
                                            Value = y.id.ToString()
                                        }).ToList();
                        foreach (var item in model.BankList)
                        {
                            var entity = model.ToEntity();
                            entity.ZoneInfoId = LoggedUserZoneInfoId;

                            entity.BankId = int.Parse(item.Value);

                            _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.Add(entity);
                            _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.SaveChanges();
                        }
                    }
                    else
                    {
                        var entity = model.ToEntity();
                        entity.ZoneInfoId = LoggedUserZoneInfoId;

                        _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.Add(entity);
                        _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.SaveChanges();
                    }
                    model.IsError = 0;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.InsertFailed : Message) : Common.GetModelStateError(ModelState);
            }
            PopulateDropdown(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.GetByID(id, "Id");
            var model = entity.ToModel();
            model.Mode = CrudeAction.Edit;
            model.Signatory1 = model.Signatory1Id + " " + _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList().Where(e => e.Id == model.Signatory1Id).Select(s => s.FullName).FirstOrDefault();
            model.Signatory2 = model.Signatory2Id + " " + _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList().Where(e => e.Id == model.Signatory2Id).Select(s => s.FullName).FirstOrDefault();
            List<PGM_BankAdviceLetterVariables> itemVariables = _pgmCommonService.PGMUnit.BankAdviceLetterVariablesRepository.GetAll().ToList();

            if (itemVariables.Count > 0)
            {
                foreach (var item in itemVariables)
                {
                    BankAdviceLetterBodyVariable details = new BankAdviceLetterBodyVariable();
                    details.Id = item.Id;
                    details.VariableNames = item.VariableNames;
                    model.BankLetterBodyVariableList.Add(details);
                }
            }

            PopulateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(BankAdviceLetterTemplateViewModel model)
        {
            model.IsError = 1;

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity();
                entity.ZoneInfoId = LoggedUserZoneInfoId;
                try
                {
                    _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.Update(entity);
                    _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.SaveChanges();
                    model.IsError = 0;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.ErrMsg = string.IsNullOrEmpty(Common.GetModelStateError(ModelState)) ? (string.IsNullOrEmpty(Message) ? ErrorMessages.UpdateFailed : Message) : Common.GetModelStateError(ModelState);
            }

            PopulateDropdown(model);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = false;
            string errMsg = string.Empty;

            var entity = _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.GetByID(id, "Id");

            if (entity != null)
            {
                try
                {
                    _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.Delete(entity.Id, "Id", null);
                    _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.SaveChanges();

                    result = true;
                    errMsg = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);

                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }
            else
            {
                errMsg = Common.GetCommomMessage(CommonMessage.DeleteFailed);
            }

            return Json(new
            {
                Success = result,
                Message = errMsg
            });
        }

        [NoCache]
        public ActionResult BackToList()
        {
            return RedirectToAction("Index");
        }

        #endregion

        #region Other Actions
        [NoCache]
        public ActionResult GetLetterTypeList()
        {
            var model = new BankAdviceLetterTemplateViewModel();
            model.LetterTypeList = Common.PopulateLetterTypeList().OrderBy(x => x.Text).ToList();
            return View("_Select", model.LetterTypeList);
        }

        [NoCache]
        public ActionResult GetBankList()
        {
            var model = new BankAdviceLetterTemplateViewModel();
            model.BankList = _pgmCommonService.PGMUnit.AccBankRepository.GetAll().OrderBy(x => x.bankName)
                            .Select(y => new SelectListItem()
                            {
                                Text = y.bankName,
                                Value = y.id.ToString()
                            }).ToList();
            return View("_Select", model.BankList);
        }

        [NoCache]
        private void PopulateDropdown(BankAdviceLetterTemplateViewModel model)
        {
            model.LetterTypeList = Common.PopulateLetterTypeList().OrderBy(x => x.Text).Select(y => new SelectListItem()
            {
                Text = y.Text,
                Value = y.Value
            }).ToList();

            //List of Bank
            model.BankList = _pgmCommonService.PGMUnit.AccBankRepository.GetAll().OrderBy(x => x.bankName)
                .Select(y => new SelectListItem()
                {
                    Text = y.bankName,
                    Value = y.id.ToString()
                }).ToList();
        }

        [NoCache]
        private string CheckBusinessRule(BankAdviceLetterTemplateViewModel model)
        {
            var obj = (from s in _pgmCommonService.PGMUnit.BankAdviceLetterTemplateRepository.GetAll()
                       where s.LetterType == model.LetterType && s.ZoneInfoId == LoggedUserZoneInfoId && s.BankId == model.BankId
                       select s.LetterType).ToList();

            if (obj.Count > 0)
            {
                Message = "Duplicate entry. Please try again.";
            }
            return Message;
        }

        public JsonResult GetEmployeeInfoTemp(int employeeId)
        {
            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(employeeId);
            return Json(new
            {
                id = emp.Id,
                EmpId = emp.EmpID + " - " + emp.FullName,
                Designation = emp.DesignationName,
                EmployeeName = emp.FullName,
                DateofJoining = emp.DateofJoining.ToString("yyyy-MM-dd")
            });
        }

        #endregion
    }
}
