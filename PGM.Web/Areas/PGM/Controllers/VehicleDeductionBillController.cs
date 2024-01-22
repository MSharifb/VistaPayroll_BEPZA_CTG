using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.VehicleDeductionBill;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

/*
Revision History (RH):
		SL	    : 01
		Author	: Md. Amanullah
		Date	: 2016-Jan-24
        SCR     : ERP_BEPZA_PGM_SCR.doc (SCR#57)
		Desc.	: If any employees inactive in any date the month then deduction bill could not entry for that month. Need to consider date of inactive month > entry month for deduction bill.
		---------
 
		SL	    : 02
		Author	: Md. Amanullah
		Date	: 2016-Mar-01
        SCR     : ERP_BEPZA_PGM_SCR.doc (SCR#59)
		Desc.	: System have to check individual salary is process or not during new entry of vehicle deduction bill.
		---------
 
*/

namespace PGM.Web.Areas.PGM.Controllers
{
    public class VehicleDeductionBillController : Controller
    {
        #region Fields
        private static string sYear;
        private static string sMonth;
        private static int sEmpID;
        private static string message;
        private readonly PGMCommonService _pgmCommonService;
        #endregion

        #region Ctor

        public VehicleDeductionBillController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonService = pgmCommonservice;
        }

        #endregion

        #region message Properties

        public string Message { get; set; }

        #endregion 

        [NoCache]
        public ActionResult Index(string message, bool? isSuccess)
        {
            VehicleDeductionBillViewModel model = new VehicleDeductionBillViewModel();
            PopulateDropdown(model);
            //if (!string.IsNullOrEmpty(message))
            //{
            //    model.IsSuccessful = (bool)isSuccess;               
            //    model.Message = message;          
            //}
            //model.Mode = CrudeAction.Create;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public JsonResult GetVehicleDeductionList(JqGridRequest request, VehicleDeductionBillViewModel model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            var list = _pgmCommonService.GetVehicleDeductionBillList().OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();
            list = string.IsNullOrEmpty(model.SalaryMonth) ? list : list.Where(q => q.SalaryMonth == model.SalaryMonth).ToList();
            list = string.IsNullOrEmpty(model.SalaryYear) ? list : list.Where(q => q.SalaryYear == model.SalaryYear).ToList();
            list = string.IsNullOrEmpty(model.EmployeeInitial) ? list : list.Where(q => q.EmployeeInitial == model.EmployeeInitial.ToUpper()).ToList();

            totalRecords = list.Count;
            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();
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

            if (request.SortingName == "SalaryYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryYear).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryYear).ToList();
                }
            }
            if (request.SortingName == "SalaryMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryMonth).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryMonth).ToList();
                }
            }

            if (request.SortingName == "EmpID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpID).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpID).ToList();
                }
            }

            if (request.SortingName == "EmployeeInitial")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmployeeInitial).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmployeeInitial).ToList();
                }
            }

            if (request.SortingName == "FullName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.FullName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.FullName).ToList();
                }
            }

            if (request.SortingName == "OfficalAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.OfficalAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.OfficalAmount).ToList();
                }
            }
            if (request.SortingName == "PersonalAmount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.PersonalAmount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.PersonalAmount).ToList();
                }
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.Id), new List<object>()
                {
                    d.Id,
                    d.SalaryYear,
                    d.SalaryMonth,
                    d.EmpID,
                    d.EmployeeInitial,
                    d.FullName,
                    d.OfficalAmount,
                    d.PersonalAmount,
                    "Delete"
                }));
            }

            return new JqGridJsonResult() { Data = response };

        }

        [NoCache]
        public ActionResult Create()
        {
            VehicleDeductionBillViewModel model = new VehicleDeductionBillViewModel();
            PopulateDropdown(model);
            model.Mode = CrudeAction.Create;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(VehicleDeductionBillViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = GetBusinessLogicValidationForCreate(model, model.SalaryYear, model.SalaryMonth);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    var entity = model.ToEntity();
                    entity.IDate = DateTime.Now;
                    entity.IUser = User.Identity.Name;

                    _pgmCommonService.PGMUnit.VehicleDeductionRepository.Add(entity);
                    _pgmCommonService.PGMUnit.VehicleDeductionRepository.SaveChanges();

                    model.IsError = 0;
                    model.Message = Common.GetCommomMessage(CommonMessage.InsertSuccessful);
                    // return RedirectToAction("Index", new { message = model.Message, isSuccess = true });

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
                model.Message = errorList;
            }
            PopulateDropdown(model);

            if (model.IsError == 0)
            {
                ViewBag.SuccessMessage = model.Message;
            }
            else
                ViewBag.ErrorMessage = model.Message;

            return View(model);
        }

        private string GetBusinessLogicValidation(VehicleDeductionBillViewModel model)
        {
            string errorMessage = string.Empty;

            string query = @"SELECT * FROM PGM_Salary";
            query += " WHERE EmployeeId=" + model.EmployeeId;
            query += " AND SalaryYear='" + model.SalaryYear + "'";
            query += " AND SalaryMonth='" + model.SalaryMonth + "'";

            if ((model.OfficalAmount + model.PersonalAmount) <= 0)
            {
                errorMessage = "Usage amount (summation of Personal and Official amount) must be greater than zero.";
            }
            var salaryChecking = _pgmCommonService.PGMUnit.SalaryMasterRepository.GetWithRawSql(query).FirstOrDefault();

            if (salaryChecking != null)
            {
                errorMessage = "Salary has been paid, so the record can't insert.";
            }
            return errorMessage;
        }

        [NoCache]
        public ActionResult Edit(int? id)
        {
            var data = _pgmCommonService.PGMUnit.VehicleDeductionRepository.GetByID(id);
            sYear = data.SalaryYear;
            sMonth = data.SalaryMonth;
            sEmpID = data.EmployeeId;
            var model = data.ToModel();

            if (model.EmployeeId != 0)
            {
                var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                model.EmpID = emp.EmpID;
                model.EmployeeInitial = emp.EmployeeInitial;
                model.FullName = emp.FullName;
                model.Designation = emp.DesignationName;

            }

            PopulateDropdown(model);
            // model.Mode = CrudeAction.Edit;
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(VehicleDeductionBillViewModel model)
        {
            string errorList = string.Empty;
            model.IsError = 1;
            errorList = GetBusinessLogicValidationForEdit(model, sYear, sMonth, sEmpID);

            if (ModelState.IsValid && (string.IsNullOrEmpty(errorList)))
            {
                try
                {
                    if (model.Id != 0)
                    {
                        var entity = model.ToEntity();
                        _pgmCommonService.PGMUnit.VehicleDeductionRepository.Update(entity);
                        _pgmCommonService.PGMUnit.VehicleDeductionRepository.SaveChanges();

                        model.Message = Common.GetCommomMessage(CommonMessage.UpdateSuccessful);
                        model.IsError = 0;

                        return RedirectToAction("Index", new { message = model.Message, isSuccess = true });
                    }
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                }
            }
            else
            {
                model.IsError = 1;
                model.Message = errorList;
            }
            model.Mode = CrudeAction.Edit;
            PopulateDropdown(model);

            if (model.IsError == 0)
            {
                return View("Index", model);
            }
            else
                ViewBag.ErrorMessage = model.Message;

            return View(model);
        }

        //[HttpPost, ActionName("Delete")]
        //[NoCache]
        //public JsonResult Delete(int id)
        //{
        //    bool result;
        //    string errMsg = string.Empty;

        //    var ObjVehicleDeduction = _pgmCommonservice.PGMUnit.VehicleDeductionBill.GetByID(id);

        //    if (ObjVehicleDeduction != null)
        //    {
        //        errMsg = GetBusinessLogicValidationForDelete(ObjVehicleDeduction.SalaryYear, ObjVehicleDeduction.SalaryMonth, ObjVehicleDeduction.EmployeeId);
        //    }

        //    if (string.IsNullOrEmpty(errMsg))
        //    {
        //        try
        //        {
        //            _pgmCommonservice.PGMUnit.VehicleDeductionBill.Delete(id);
        //            _pgmCommonservice.PGMUnit.VehicleDeductionBill.SaveChanges();
        //            result = true;
        //        }
        //        catch (UpdateException ex)
        //        {
        //            if (ex.InnerException != null && ex.InnerException is SqlException)
        //            {
        //                SqlException sqlException = ex.InnerException as SqlException;
        //            }

        //                errMsg = Common.GetSqlExceptionMessage(sqlException.Number);
        //                ModelState.AddModelError("Error", errMsg);
        //            result = false;
        //        }
        //        catch (Exception ex)
        //        {
        //            result = false;
        //        }
        //    }
        //    else
        //    {
        //        result = false;
        //    }
        //    return Json(new
        //    {
        //        Success = result,
        //        Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
        //    });
        //}

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(int id)
        {
            bool isSuccessful = false;
            string errMsg = string.Empty;
            var ObjVehicleDeduction = _pgmCommonService.PGMUnit.VehicleDeductionRepository.GetByID(id);

            if (ObjVehicleDeduction != null)
            {
                errMsg = GetBusinessLogicValidationForDelete(ObjVehicleDeduction.SalaryYear, ObjVehicleDeduction.SalaryMonth, ObjVehicleDeduction.EmployeeId);
            }

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    if (id != 0)
                    {
                        _pgmCommonService.PGMUnit.VehicleDeductionRepository.Delete(id);
                        _pgmCommonService.PGMUnit.VehicleDeductionRepository.SaveChanges();
                        message = Common.GetCommomMessage(CommonMessage.DeleteSuccessful);
                        isSuccessful = true;

                    }
                }
                catch (Exception ex)
                {
                    isSuccessful = false;
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                }
            }
            else
            {
                isSuccessful = false;
            }

            return Json(new
            {
                Success = isSuccessful,
                Message = isSuccessful == true ? message : errMsg
            }, JsonRequestBehavior.AllowGet);
        }

        private string GetBusinessLogicValidationForDelete(string spYear, string spMonth, int spEmpID)
        {
            string errorMessage = string.Empty;
            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(spMonth) || spMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(spYear) || spYear == tr.SalaryYear)
                               && (spEmpID == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;
            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for the month. Delete is not acceptable..";
            }
            return errorMessage;
        }

        private string GetBusinessLogicValidationForEdit(VehicleDeductionBillViewModel model, string spYear, string spMonth, int spEmpID)
        {
            string errorMessage = string.Empty;

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(spMonth) || spMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(spYear) || spYear == tr.SalaryYear)
                               && (spEmpID == tr.EmployeeId)
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;

            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for the month. Update is not acceptable.";
            }

            if ((model.OfficalAmount + model.PersonalAmount) <= 0)
            {
                errorMessage = "Usage amount (summation of Personal and Official amount) must be greater than zero.";
            }
            return errorMessage;

        }

        private string GetBusinessLogicValidationForCreate(VehicleDeductionBillViewModel model, string spYear, string spMonth)
        {
            string errorMessage = string.Empty;

            var salaryexist = (from tr in _pgmCommonService.PGMUnit.SalaryMasterRepository.GetAll()
                               where (string.IsNullOrEmpty(spMonth) || spMonth == tr.SalaryMonth)
                               && (string.IsNullOrEmpty(spYear) || spYear == tr.SalaryYear)
                     /*RH#02*/ && tr.EmployeeId == model.EmployeeId /*End RH#02*/
                               select tr.EmployeeId).ToList();

            int totalRecords = salaryexist == null ? 0 : salaryexist.Count;
            if (totalRecords > 0)
            {
                errorMessage = "Salary has been processed for the month. Create is not acceptable.";
            }

            if ((model.OfficalAmount + model.PersonalAmount) <= 0)
            {
                errorMessage = "Usage amount (summation of Personal and Official amount) must be greater than zero.";
            }
            return errorMessage;

        }

        [NoCache]
        public JsonResult GetEmployeeInfo(VehicleDeductionBillViewModel model)
        {
            string errorList = string.Empty;
            errorList = GetValidationChecking(model);

            if (string.IsNullOrEmpty(errorList))
            {
                try
                {
                    var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                    if (emp != null)
                    {
                        return Json(new
                        {
                            EmpId = emp.EmpID,
                            EmployeeInitial = emp.EmployeeInitial,
                            FullName = emp.FullName,
                            Designation = emp.DesignationName
                        });
                    }
                    else
                    {
                        return Json(new { Result = false });
                    }
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                    return Json(new { Result = false });
                }
            }
            else
            {
                return Json(new { Result = errorList });
            }
        }

        [NoCache]
        private string GetValidationChecking(VehicleDeductionBillViewModel model)
        {
            string errorMessage = string.Empty;

            if (String.IsNullOrEmpty(model.SalaryYear))
            {
                errorMessage = "Please select year.";
                return errorMessage;
            }

            if (String.IsNullOrEmpty(model.SalaryMonth))
            {
                errorMessage = "Please select month.";
                return errorMessage;
            }

            var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
            if (emp != null)
            {
                if (emp.DateofInactive != null)
                {

                    DateTime entryMonth = new DateTime(int.Parse(model.SalaryYear), UtilCommon.GetMonthNo(model.SalaryMonth), 1);
                    DateTime dateofInactive = Convert.ToDateTime(emp.DateofInactive);
                    DateTime dateOfInactiveMonth = new DateTime(dateofInactive.Year, dateofInactive.Month, 1);

                    // Comparing entry month is grater then date of inactive month
                    if (entryMonth > dateOfInactiveMonth)
                    {
                        errorMessage = "InActiveEmployee";
                        return errorMessage;
                    }
                }
            }
            return errorMessage;
        }

        [NoCache]
        public ActionResult GetYearList()
        {
            var model = new VehicleDeductionBillViewModel();
            model.YearList = Common.PopulateYearList().ToList();
            return View("_Select", model.YearList);
        }

        [NoCache]
        public ActionResult GetMonthList()
        {
            var monthList = Common.PopulateMonthList().ToList();
            return View("_Select", monthList);
        }

        public JsonResult AutoCompleteEmpInitaial(string term)
        {
            var result = (from r in _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                          where r.EmployeeInitial.ToLower().Contains(term.ToLower())
                          select new { r.EmployeeInitial, r.FullName }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Utilities

        private void PopulateDropdown(VehicleDeductionBillViewModel model)
        {
            model.YearList = Common.PopulateYearList().ToList();
            model.MonthList = Common.PopulateMonthList().ToList();
        }

        #endregion
    }
}
