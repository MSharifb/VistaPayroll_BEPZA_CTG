using DAL.PGM;
using Domain.PGM;
using PGM.Web.Areas.PGM.Models.OtherAdjustmentStyleOne;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using Lib.Web.Mvc.JQuery.JqGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace PGM.Web.Areas.PGM.Controllers
{
    [NoCache]
    public class OtherAdjustmentStyleOneController : BaseController
    {
        #region Fields
        private readonly PgmOtherAdjustmentService _pgmOtherAdjustmentService;
        private readonly PgmEmployeeSalaryStructureService _empSalaryStructureService;
        #endregion

        #region Constructor

        public OtherAdjustmentStyleOneController(PgmOtherAdjustmentService pgmOtherAdjustmentService, PgmEmployeeSalaryStructureService empSSS)
        {
            this._pgmOtherAdjustmentService = pgmOtherAdjustmentService;
            this._empSalaryStructureService = empSSS;
        }

        #endregion

        #region Actions

        [NoCache]
        public ViewResult Index()
        {
            var model = new OtherAdjustmentStyleOneModel();
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, OtherAdjustmentStyleOneModel model)
        {
            int totalRecords = 0;

            var list = _pgmOtherAdjustmentService.OtherAdjustmentListMasterStyleOne(
                model.SalaryMonth,
                model.SalaryYear,
                LoggedUserZoneInfoId);


            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            #region Sorting

            if (request.SortingName == "SalaryYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryYear).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryYear).ToList();
            }

            if (request.SortingName == "SalaryMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryMonth).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryMonth).ToList();
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(d.SalaryYear + "-" + d.SalaryMonth, new List<object>()
                {
                    d.SalaryYear+"-"+ d.SalaryMonth,
                    d.SalaryYear,
                    d.SalaryMonth,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetDetailList(JqGridRequest request, string year, string month, OtherAdjustmentStyleOneModel model)
        {
            int totalRecords = 0;

            var list = _pgmOtherAdjustmentService.OtherAdjustmentSearchListStyleOne(
                month,
                year,
                model.EmpID,
                model.EmployeeName,
                model.EmployeeDesignation,
                LoggedUserZoneInfoId);

            totalRecords = list == null ? 0 : list.Count;

            JqGridResponse response = new JqGridResponse()
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * (request.PagesCount.HasValue ? request.PagesCount.Value : 1)).ToList();

            #region Sorting

            if (request.SortingName == "SalaryYear")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryYear).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryYear).ToList();
            }

            if (request.SortingName == "SalaryMonth")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.SalaryMonth).ToList();
                else
                    list = list.OrderByDescending(x => x.SalaryMonth).ToList();
            }

            if (request.SortingName == "EmpID")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmpID).ToList();
                else
                    list = list.OrderByDescending(x => x.EmpID).ToList();
            }

            if (request.SortingName == "EmployeeName")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmployeeName).ToList();
                else
                    list = list.OrderByDescending(x => x.EmployeeName).ToList();
            }

            if (request.SortingName == "EmployeeDesignation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                    list = list.OrderBy(x => x.EmployeeDesignation).ToList();
                else
                    list = list.OrderByDescending(x => x.EmployeeDesignation).ToList();
            }

            #endregion

            foreach (var d in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(d.EmployeeId), new List<object>()
                {
                    d.EmployeeId,
                    d.EmpID,
                    d.EmployeeName,
                    d.EmployeeDesignation,
                    d.SalaryYear,
                    d.SalaryMonth,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new OtherAdjustmentStyleOneModel();
            model.strMode = CrudeAction.Create;
            PopulateDropdown(model);

            model.SalaryYear = DateTime.Now.Year.ToString();
            model.SalaryMonth = DateTime.Now.ToString("MMMM");
            model.LockEmpDDL = false;
            model.LockYearMonth = false;

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(OtherAdjustmentStyleOneModel model)
        {
            model.strMode = CrudeAction.Create;
            string errorList = string.Empty;
            errorList = GetBusinessLogicValidation(model);

            ModelState.Remove("Id");

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    int salaryStructureId = 0;
                    var empSalaryDetails = _empSalaryStructureService.GetEmpSalaryStructureDetails(model.EmployeeId, out salaryStructureId);

                    var monthlyCharges = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetMonthlyCharges(model.EmployeeId, model.SalaryYear, model.SalaryMonth);
                    var monthlyChargeAmount = 0.0M;

                    foreach (var item in model.SalaryStructureDetail)
                    {
                        //var salaryHead = empSalaryDetails.FirstOrDefault(s =>
                        //    s.HeadId == item.HeadId && s.HeadType == item.HeadType && s.Amount == item.Amount);

                        monthlyChargeAmount = 0.0M;
                        if (monthlyCharges.Any(c => c.HeadId == item.HeadId))
                        {
                            monthlyChargeAmount = Convert.ToDecimal(monthlyCharges.FirstOrDefault(c => c.HeadId == item.HeadId).HeadAmount);
                        }

                        //if (salaryHead == null)
                        if (monthlyChargeAmount != item.Amount)
                        {
                            var isDataExists = _pgmOtherAdjustmentService
                                .PGMUnit
                                .OtherAdjustDeductionRepository
                                .Get(a => a.EmployeeId == model.EmployeeId
                                            && a.SalaryMonth == model.SalaryMonth
                                            && a.SalaryYear == model.SalaryYear
                                            && a.SalaryHeadId == item.HeadId);

                            if (isDataExists != null && isDataExists.Count() > 0)
                            {
                                _pgmOtherAdjustmentService
                                    .PGMUnit
                                    .OtherAdjustDeductionRepository
                                    .Delete(d => d.EmployeeId == model.EmployeeId
                                                    && d.SalaryMonth == model.SalaryMonth
                                                    && d.SalaryYear == model.SalaryYear
                                                    && d.SalaryHeadId == item.HeadId);
                            }


                            var otherAdjustDeduct = new PGM_OtherAdjustDeduct()
                            {
                                EmployeeId = model.EmployeeId,
                                SalaryMonth = model.SalaryMonth,
                                SalaryYear = model.SalaryYear,

                                HeadType = item.HeadType,
                                SalaryHeadId = item.HeadId,
                                AmountType = item.AmountType,
                                Amount = item.Amount,
                                Remarks = model.Remarks,
                                IsOverrideStructureAmount = true,

                                IUser = User.Identity.Name,
                                IDate = Common.CurrentDateTime
                            };

                            _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.Add(otherAdjustDeduct);
                        }
                    }

                    _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.SaveChanges();

                    return RedirectToAction("GoToDetails", new { idYearMonth = model.SalaryYear + "-" + model.SalaryMonth });
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrMsg = errorList;
            }

            PopulateDropdown(model);
            GetSalaryHeadAmountTypeSetting(model);

            return View(model);
        }

        [NoCache]
        public ActionResult Edit(String id) // id is containing employee id, salary year and salary month
        {
            int employeeId = Convert.ToInt32(id.Split('-')[0]);
            string salaryYear = id.Split('-')[1];
            string salaryMonth = id.Split('-')[2];

            var entity = _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.Get(a =>
                a.EmployeeId == employeeId && a.SalaryMonth == salaryMonth && a.SalaryYear == salaryYear).FirstOrDefault();

            // if no other adjustment found during NEXT-PREV navigation
            // then create new model.

            var model = new OtherAdjustmentStyleOneModel();
            if (entity != null)
            {
                model = entity.ToStyleOneModel();
            }
            else
            {
                model.EmployeeId = employeeId;
                model.SalaryYear = salaryYear;
                model.SalaryMonth = salaryMonth;
            }

            model.strMode = CrudeAction.Create;
            model.LockEmpDDL = true;

            var salaryexist1 = (from tr in _pgmOtherAdjustmentService.PGMUnit.SalaryMasterRepository.GetAll()
                                where tr.SalaryYear == model.SalaryYear
                                      && tr.SalaryMonth == model.SalaryMonth
                                      && tr.EmployeeId == model.EmployeeId
                                select tr.EmployeeId)
                .ToList();

            if (salaryexist1 != null && salaryexist1.Any())
            {
                model.LockYearMonth = true;
            }

            PopulateDropdown(model);

            if (model.EmployeeId != 0)
            {
                var emp = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

                model.EmpID = emp.EmpID;
                model.EmployeeName = emp.FullName;
                model.EmployeeDesignation = emp.DesignationName;

                PrepareSalaryStructure(model);
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(String id)
        {
            bool result = false;
            string errMsg = string.Empty;

            int employeeId = Convert.ToInt32(id.Split('-')[0]);
            string salaryYear = id.Split('-')[1];
            string salaryMonth = id.Split('-')[2];

            var entity = _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.Get(a =>
                a.EmployeeId == employeeId && a.SalaryMonth == salaryMonth && a.SalaryYear == salaryYear);

            errMsg = GetBusinessLogicValidationForDelete(entity.FirstOrDefault().ToStyleOneModel());

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    foreach (var item in entity)
                    {
                        _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.Delete(item);
                    }
                    _pgmOtherAdjustmentService.PGMUnit.OtherAdjustDeductionRepository.SaveChanges();

                    result = true;
                }
                catch (UpdateException ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                    ModelState.AddModelError("Error", errMsg);
                }
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        [NoCache]
        public ActionResult GoToDetails(string idYearMonth)
        {
            var model = new OtherAdjustmentStyleOneModel();

            string[] yearMonth = idYearMonth.Split('-');
            model.SalaryYear = yearMonth[0];
            model.SalaryMonth = yearMonth[1];

            return View("DetailsList", model);
        }

        [NoCache]
        public ActionResult GetEmployeeSalaryStructure(int employeeId, string salaryYear, string salaryMonth)
        {
            var model = new OtherAdjustmentStyleOneModel();
            model.EmployeeId = employeeId;
            model.SalaryMonth = salaryMonth;
            model.SalaryYear = salaryYear;

            PrepareSalaryStructure(model);

            return PartialView("_SalaryStructureDetail", model);
        }



        [NoCache]
        public JsonResult GetPrevNextEmployeeId(int currentEmployeeId, bool isNext = true)
        {
            var empList = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeList(LoggedUserZoneInfoId);
            var currentEmp = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeById(currentEmployeeId);
            var index = empList.IndexOf(currentEmp);

            int resultEmployeeId = 0;
            vwPGMEmploymentInfo emp = null;

            if (isNext)
            {
                if (index + 1 < empList.Count())
                {
                    emp = empList[index + 1];
                }
            }
            else
            {
                if (index - 1 > -1)
                {
                    emp = empList[index - 1];
                }
            }

            if (emp != null)
            {
                resultEmployeeId = emp.Id;
            }

            return Json(new
            {
                employeeId = resultEmployeeId,
            }, JsonRequestBehavior.AllowGet);
        }


        #endregion Action

        #region Others

        [NoCache]
        private void PopulateDropdown(OtherAdjustmentStyleOneModel model)
        {
            model.SalaryYearList = SalaryYearList();
            model.SalaryMonthList = Common.PopulateMonthList();

            //----------
            var emps = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetEmployeeList().Select(q => new
            {
                ZoneInfoId = q.SalaryWithdrawFromZoneId,
                EmpID = q.EmpID,
                Id = q.Id,
                DateOfInactive = q.DateofInactive,
                DisplayText = q.FullName + " [" + q.EmpID + " ]"
            })
                .Where(x => x.ZoneInfoId == LoggedUserZoneInfoId && x.DateOfInactive == null).ToList();

            model.EmployeeList = emps.Select(e => new SelectListItem()
            {
                Text = e.DisplayText,
                Value = e.Id.ToString()
            }).ToList();
            //------------
        }

        [NoCache]
        public string GetBusinessLogicValidation(OtherAdjustmentStyleOneModel model)
        {
            List<String> errorMessage = new List<string>();

            bool salaryFound = false;

            var salaryexist1 = (from tr in _pgmOtherAdjustmentService.PGMUnit.SalaryMasterRepository.GetAll()
                                where tr.SalaryYear == model.SalaryYear
                                && tr.SalaryMonth == model.SalaryMonth
                                && tr.EmployeeId == model.EmployeeId
                                select tr.EmployeeId)
                               .ToList();

            if (salaryexist1 != null && salaryexist1.Any())
            {
                salaryFound = true;
            }


            if (salaryFound)
            {
                errorMessage.Add("Salary has been processed for thia month " + model.SalaryMonth + "/" + model.SalaryYear + ". Adjustment is not acceptable.");
            }

            return Common.ErrorListToString(errorMessage);
        }

        [NoCache]
        private string GetBusinessLogicValidationForDelete(OtherAdjustmentStyleOneModel model)
        {
            StringBuilder errorMessage = new StringBuilder();
            bool salaryFound = false;

            var salaryexist1 = (from tr in _pgmOtherAdjustmentService.PGMUnit.SalaryMasterRepository.GetAll()
                                where tr.SalaryYear == model.SalaryYear
                                && tr.SalaryMonth == model.SalaryMonth
                                && tr.EmployeeId == model.EmployeeId
                                select tr.EmployeeId)
                                   .ToList();

            if (salaryexist1 != null && salaryexist1.Any())
            {
                salaryFound = true;
            }

            if (salaryFound)
            {
                errorMessage.AppendLine().Append("Salary has been processed for thia month " + model.SalaryMonth + "/" + model.SalaryYear + ". Adjustment is not acceptable.");
            }

            return errorMessage.ToString();
        }

        #region Index Page Search

        [NoCache]
        public ActionResult GetSalaryMonthList()
        {
            var salaryMonth = new Dictionary<string, string>();

            foreach (var item in Common.PopulateMonthList())
            {
                salaryMonth.Add(item.Text, item.Value);
            }

            ViewBag.SalaryMonthList = salaryMonth;
            return PartialView("Select", salaryMonth);
        }

        [NoCache]
        public ActionResult GetSalaryYearList()
        {
            var SalaryYear = new Dictionary<string, string>();

            foreach (var item in Common.PopulateYearList())
            {
                SalaryYear.Add(item.Text, item.Value);
            }

            ViewBag.IncomeYearList = SalaryYear;
            return PartialView("Select", SalaryYear);
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {
            var Designation = new List<SelectListItem>();

            Designation = _pgmOtherAdjustmentService.PGMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name)
            .ToList()
            .Select(y => new SelectListItem()
            {
                Text = y.Name,
                Value = y.Id.ToString()
            }).ToList();

            return PartialView("_Select", Designation);
        }

        #endregion

        [NoCache]
        private IList<SelectListItem> SalaryYearList()
        {
            var SalaryYear = new List<SelectListItem>();
            return Common.PopulateYearList().DistinctBy(x => x.Value).ToList();
        }

        private void PrepareSalaryStructure(OtherAdjustmentStyleOneModel model)
        {
            // STEP 1: Get Employee Salary Structure
            // STEP 2: Get Other Adjustment - Single, all
            // STEP 3: Get Monthly Charges like Charge Allowance, Residence and Water Bill, Electric Bill, Loan



            var salaryStructureId = 0;

            var empSalaryDetails = _empSalaryStructureService.GetEmpSalaryStructureDetails(model.EmployeeId, out salaryStructureId);
            var salaryHeads = _pgmOtherAdjustmentService.PGMUnit.SalaryHeadRepository.Fetch().OrderBy(s => s.SortOrder).ToList();

            var adjustments = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetOtherAdjustments(model.EmployeeId, model.SalaryYear, model.SalaryMonth).ToList();

            if (empSalaryDetails.Count > 0)
            {
                var monthlyCharges = _pgmOtherAdjustmentService.PGMUnit.FunctionRepository.GetMonthlyCharges(model.EmployeeId,
                    model.SalaryYear, model.SalaryMonth);

                foreach (var item in empSalaryDetails)
                {
                    var ssdModel = item.ToModel();
                    ssdModel.cssSalaryHeadClass = "";

                    ssdModel.AmountType = item.AmountType;

                    if (adjustments.Any(a => a.HeadId == item.HeadId))
                    {
                        ssdModel.AmountType = adjustments.FirstOrDefault(a => a.HeadId == item.HeadId).AmountType;
                        ssdModel.Amount = Convert.ToDecimal(adjustments.FirstOrDefault(a => a.HeadId == item.HeadId).HeadAmount);
                        ssdModel.cssSalaryHeadClass = "cssSalaryHeadClass";
                    }

                    if (monthlyCharges.Any(c => c.HeadId == item.HeadId))
                    {
                        ssdModel.Amount = Convert.ToDecimal(monthlyCharges.FirstOrDefault(c => c.HeadId == item.HeadId).HeadAmount);
                    }

                    ssdModel.HeadAmountTypeList = Common.PopulateAmountType().ToList();
                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;
                    ssdModel.IsBasicHead = salaryHeads.Find(x => x.Id == item.HeadId).IsBasicHead;
                    model.SalaryStructureDetail.Add(ssdModel);
                }
            }

            model.TotalAddition = model.SalaryStructureDetail.Where(s => s.HeadType == "Addition").Sum(x => x.Amount);
            model.TotalDeduction = model.SalaryStructureDetail.Where(s => s.HeadType == "Deduction").Sum(x => x.Amount);
            model.NetPay = model.TotalAddition - model.TotalDeduction;
            model.SalaryStructureId = salaryStructureId;
        }

        private void GetSalaryHeadAmountTypeSetting(OtherAdjustmentStyleOneModel model)
        {
            foreach (var item in model.SalaryStructureDetail)
            {
                var ssdModel = item;
                ssdModel.AmountType = item.AmountType;
                ssdModel.HeadAmountTypeList = Common.PopulateAmountType().ToList();

                // Select Amount Type
                if (ssdModel.HeadAmountTypeList.Count > 0)
                {
                    foreach (var amttype in ssdModel.HeadAmountTypeList.Where(h => h.Value == ssdModel.AmountType))
                    {
                        amttype.Selected = true;
                    }
                }
            }
        }

        #endregion Others
    }
}
