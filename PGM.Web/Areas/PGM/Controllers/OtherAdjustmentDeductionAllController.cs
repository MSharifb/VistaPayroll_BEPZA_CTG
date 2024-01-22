using DAL.PGM;
using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.OtherAdjustmentDeductionAll;
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
    public class OtherAdjustmentDeductionAllController : BaseController
    {
        #region Fields
        private static string type;
        private readonly PGMCommonService _pgmCommonservice;
        #endregion

        #region Constructor
        public OtherAdjustmentDeductionAllController(PGMCommonService pgmCommonservice)
        {
            _pgmCommonservice = pgmCommonservice;
        }

        #endregion

        #region Actions
        [NoCache]
        public ViewResult Index(string Id)
        {
            var model = new OtherAdjustmentDeductAll();
            type = Id;
            model.HeadType = Id;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, OtherAdjustmentDeductAll model)
        {
            string filterExpression = String.Empty;
            int totalRecords = 0;

            List<OtherAdjustmentDeductAll> list = (from OAdj in _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.GetAll()

                        // join with salaryhead
                    join Sal in _pgmCommonservice.PGMUnit.SalaryHeadRepository.GetAll() on OAdj.SalaryHeadId equals Sal.Id
                        // Left join with designation
                    join desigLj in _pgmCommonservice.PGMUnit.DesignationRepository.GetAll() on OAdj.DesignationId equals desigLj.Id into dGroup
                    from desig in dGroup.DefaultIfEmpty(new PRM_Designation { Name = "All", Id = 0 })
                        // Left Join with staff category
                    join secLj in _pgmCommonservice.PGMUnit.StaffCategoryRepository.GetAll() on OAdj.StaffCategoryId equals secLj.Id into sGroup
                    from sec in sGroup.DefaultIfEmpty(new PRM_StaffCategory { Name = "All", Id = 0 })
                        // Left Join with department
                    join deptLj in _pgmCommonservice.PGMUnit.DivisionRepository.GetAll() on OAdj.DepartmentId equals deptLj.Id into deptGroup
                    from dept in deptGroup.DefaultIfEmpty(new PRM_Division { Name = "All", Id = 0 })
                        // Left join with employmentype
                    join emptLj in _pgmCommonservice.PGMUnit.EmploymentTypeRepository.GetAll() on OAdj.EmploymentTypeId equals emptLj.Id into pGroup
                    from empt in pGroup.DefaultIfEmpty(new PRM_EmploymentType { Name = "All", Id = 0 })
                        // Left Join with From Job Grade
                    join fromJobGLj in _pgmCommonservice.PGMUnit.JobGradeRepository.GetAll() on OAdj.FromJobGradeId equals fromJobGLj.Id into fJgGroup
                    from fromJobGrade in fJgGroup.DefaultIfEmpty(new PRM_JobGrade { GradeName = "All", Id = 0})
                        // Left Join with to Job Grade
                    join toJobGLj in _pgmCommonservice.PGMUnit.JobGradeRepository.GetAll() on OAdj.ToJobGradeId equals toJobGLj.Id into tJgGroup
                    from toJobGrade in tJgGroup.DefaultIfEmpty(new PRM_JobGrade { GradeName = "All", Id = 0 })

                    select new OtherAdjustmentDeductAll()
                    {
                        Id = OAdj.Id,
                        SalaryMonth = OAdj.SalaryMonth,
                        SalaryYear = OAdj.SalaryYear,

                        HeadType = OAdj.HeadType,
                        SalaryHeadId = OAdj.SalaryHeadId,
                        SalaryHead = Sal.HeadName,

                        DepartmentId = OAdj.DepartmentId,
                        Department = dept.Name,
                        StaffCategoryId = OAdj.StaffCategoryId,
                        StaffCategory = sec.Name,
                        DesignationId = Convert.ToInt32(OAdj.DesignationId),
                        Designation = desig.Name,
                        EmploymentTypeId = OAdj.EmploymentTypeId,
                        EmploymentType = empt.Name,
                        JobGrade = fromJobGrade.GradeName + " - " + toJobGrade.GradeName,

                        IsOverrideStructureAmount = Convert.ToBoolean(OAdj.IsOverrideStructureAmount),
                        AmountType = OAdj.AmountType,
                        Amount = Convert.ToDecimal(OAdj.Amount),
                        ZoneInfoId = Convert.ToInt32(OAdj.ZoneInfoId)
                    }).OrderBy(x => Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")).ToList();


            #region Search

            list = list.Where(t => t.ZoneInfoId == LoggedUserZoneInfoId).ToList();

            if (!string.IsNullOrEmpty(model.SalaryMonth))
            {
                list = list.Where(t => t.SalaryMonth == model.SalaryMonth).ToList();
            }

            if (!string.IsNullOrEmpty(model.SalaryYear))
            {
                list = list.Where(t => t.SalaryYear == model.SalaryYear).ToList();
            }

            if (model.EmploymentTypeId != null)
            {
                list = list.Where(t => t.EmploymentTypeId == model.EmploymentTypeId).ToList();
            }

            if (!string.IsNullOrEmpty(model.HeadType) && !model.HeadType.Equals("All"))
            {
                list = list.Where(t => (t.HeadType ?? String.Empty).ToLower().Contains((model.HeadType ?? String.Empty).ToLower())).ToList();
            }

            if (model.SalaryHeadId != 0)
            {
                list = list.Where(t => t.SalaryHeadId == model.SalaryHeadId).ToList();
            }

            if (model.DesignationId > 0)
            {
                list = list.Where(t => t.DesignationId == model.DesignationId).ToList();
            }

            if (model.StaffCategoryId > 0)
            {
                list = list.Where(t => t.StaffCategoryId == model.StaffCategoryId).ToList();
            }
            #endregion

            totalRecords = list == null ? 0 : list.Count;

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

            if (request.SortingName == "Type")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.HeadType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.HeadType).ToList();
                }
            }

            if (request.SortingName == "EmploymentType")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmploymentType).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmploymentType).ToList();
                }
            }

            if (request.SortingName == "SalaryHead")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.SalaryHead).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.SalaryHead).ToList();
                }
            }

            if (request.SortingName == "Amount")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.Amount).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.Amount).ToList();
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


                    d.StaffCategoryId,
                    d.StaffCategory,
                    d.Department,
                    d.DesignationId,
                    d.Designation,
                    d.EmploymentTypeId,
                    d.EmploymentType,
                    d.JobGrade,

                    d.HeadType,
                    d.SalaryHead,
                    d.AmountType,
                    d.Amount,

                    d.IsOverrideStructureAmount,
                    "Delete"
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [NoCache]
        public ActionResult Create()
        {
            var model = new OtherAdjustmentDeductAll();
            model.Mode = "Add";
            model.IsOverrideStructureAmount = true;
            PrepareModel(model);

            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult Create(OtherAdjustmentDeductAll model)
        {
            model.Mode = "Add";
            type = model.SelectedType;
            model.HeadType = model.SelectedType;

            string errorList = GetBusinessLogicValidation(model);

            //var x = this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    foreach (var item in CreateEntityList(model))
                    {
                        _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.Add(item);
                    }

                    _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.SaveChanges();
                    return RedirectToAction("Index/" + type.ToString());
                }

                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                    PrepareErrorModel(model);
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrClss = "failed";
                model.ErrMsg = errorList;
                PrepareErrorModel(model);
            }

            PrepareModel(model);
            return View(model);
        }

        [NoCache]
        public ActionResult Edit(int id)
        {
            var entity = _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.GetByID(id);
            if (entity != null)
            {
                var model = entity.ToModel();
                model.Mode = CrudeAction.Edit;

                PrepareModel(model);
                return View(model);
            }
            else
            {
                return View(new OtherAdjustmentDeductAll());
            }
        }

        [HttpPost]
        [NoCache]
        public ActionResult Edit(OtherAdjustmentDeductAll model)
        {
            model.Mode = CrudeAction.Edit;
            type = model.SelectedType;
            model.HeadType = model.SelectedType;

            string errorList = string.Empty;
            errorList = GetBusinessLogicValidationForEdit(model);

            // Remove unnessesary required field for edit
            ModelState.Remove("FromYear");
            ModelState.Remove("ToYear");
            ModelState.Remove("FromMonth");
            ModelState.Remove("ToMonth");

            if (ModelState.IsValid && string.IsNullOrEmpty(errorList))
            {
                try
                {
                    model.ZoneInfoId = LoggedUserZoneInfoId;

                    model.IsDayWise = false;
                    if (model.AmountType == PGMEnum.AmountType_SalaryAdjustmentAll.Day_Wise.ToString())
                        model.IsDayWise = true;

                    var entity = model.ToEntity();
                    entity.EUser = User.Identity.Name;
                    entity.EDate = Common.CurrentDateTime;

                    _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.Update(entity);
                    _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.SaveChanges();

                    return RedirectToAction("Index/" + type);
                }
                catch (Exception ex)
                {
                    model.IsError = 1;
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    PrepareErrorModel(model);
                }
            }
            else
            {
                model.IsError = 1;
                model.ErrClss = "failed";
                model.ErrMsg = errorList;
                PrepareErrorModel(model);
            }
            PrepareModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [NoCache]
        public JsonResult Delete(int id)
        {
            bool result;
            string errMsg = string.Empty;
            var entity = _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.GetByID(id);

            var yearMonth = Convert.ToDateTime(entity.SalaryYear + "-" + entity.SalaryMonth + "-01");

            var employeeSalary = (from s in _pgmCommonservice.PGMUnit.SalaryMasterRepository.GetAll()
                                      .Where(x => (Convert.ToDateTime(x.SalaryYear + "-" + x.SalaryMonth + "-01")) == yearMonth
                                      && x.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId) select s)
                                      .ToList();

            if (employeeSalary.Count > 0)
            {
                errMsg = "Monthly salary has already been processed for " + entity.SalaryMonth + "-" + entity.SalaryYear + ". You cannot delete entry.";
            }

            if (string.IsNullOrEmpty(errMsg))
            {
                try
                {
                    _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.Delete(id);
                    _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    errMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Delete);
                    ModelState.AddModelError("Error", Common.GetCommomMessage(CommonMessage.DeleteFailed));
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return Json(new
            {
                Success = result,
                Message = result ? Common.GetCommomMessage(CommonMessage.DeleteSuccessful) : errMsg
            });
        }

        #endregion

        #region Others

        [NoCache]
        public string GetBusinessLogicValidation(OtherAdjustmentDeductAll model)
        {
            StringBuilder errorMessage = new StringBuilder();


            if (Convert.ToDateTime(model.FromDate) > Convert.ToDateTime(model.ToDate))
            {
                errorMessage.AppendLine().Append("To month must be greater then From month.");
            }


            bool salaryFound = false;
            var listYearMonth = GetListOfYearMonth(model);
            foreach (var item in listYearMonth)
            {
                var salaryexist1 = (from m in _pgmCommonservice.PGMUnit.SalaryMasterRepository.GetAll()
                                    join emp in _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeList() on m.EmployeeId equals emp.Id
                                    where m.SalaryYear == item.Year
                                        && m.SalaryMonth == item.Month
                                        && emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                                    select m.EmployeeId)
                                   .ToList();

                if (salaryexist1 != null && salaryexist1.Any())
                {
                    salaryFound = true;
                }


                // TODO: Need to recheck duplicate validation; job grade
                var duplicate = _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.GetAll()
                                .Where(o => o.SalaryYear == item.Year
                                            && o.SalaryMonth == item.Month
                                            && o.HeadType == model.HeadType
                                            && o.SalaryHeadId == model.SalaryHeadId

                                            && o.DesignationId == model.DesignationId
                                            && o.StaffCategoryId == model.StaffCategoryId
                                            && o.EmploymentTypeId == model.EmploymentTypeId)
                                .ToList();

                //var duplicate = (from o in _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.GetAll()
                //    // Left join with employmentype
                //    join empt in _prmCommonservice.PRMUnit.EmploymentTypeRepository.GetAll() on o.EmploymentTypeId
                //    equals empt.Id into pGroup
                //    from empt2 in pGroup.DefaultIfEmpty(new PRM_EmploymentType {Name = "All", Id = 0})
                //    // join with salaryhead
                //    join Sal in _prmCommonservice.PRMUnit.SalaryHeadRepository.GetAll() on o.SalaryHeadId equals Sal.Id
                //    // Left join with designation
                //    join desigLj in _prmCommonservice.PRMUnit.DesignationRepository.GetAll() on o.DesignationId
                //    equals desigLj.Id into dGroup
                //    from desig in dGroup.DefaultIfEmpty(new PRM_Designation {Name = "All", Id = 0})
                //    // Left Join with section
                //    join secLj in _prmCommonservice.PRMUnit.SectionRepository.GetAll() on o.SectionId equals secLj.Id
                //    into sGroup
                //    from sec in sGroup.DefaultIfEmpty(new PRM_Section {Name = "All", Id = 0})
                //    // Left Join with department
                //    join deptLj in _prmCommonservice.PRMUnit.DivisionRepository.GetAll() on o.DepartmentId equals
                //    deptLj.Id into deptGroup
                //    from dept in deptGroup.DefaultIfEmpty(new PRM_Division {Name = "All", Id = 0})

                //    where o.SalaryYear == item.Year
                //          && o.SalaryMonth == item.Month
                //          && empt2.Id == model.EmploymentTypeId
                //          && Sal.Id == model.SalaryHeadId
                //          && desig.Id == model.DesignationId
                //          && sec.Id == model.SectionId
                //          && dept.Id == model.DepartmentId
                //          && o.Type == model.Type
                //          && o.ZoneInfoId == LoggedUserZoneInfoId
                //    select o).ToList();


                if (duplicate.Any())
                {
                    errorMessage.AppendLine().Append("Duplicate Entry for " + item.Year + "/" + item.Month + ".");
                }
            }



            if (salaryFound)
            {
                errorMessage.AppendLine().Append("Salary has been processed for the month. Create is not acceptable.");
            }

            if (
                    (model.FromJobGradeId != null && model.ToJobGradeId == null)
                    || (model.FromJobGradeId == null && model.ToJobGradeId != null)
                )
            {
                errorMessage.AppendLine().Append("Please select both job grade (From & To)");
            }

            return errorMessage.ToString();
        }

        [NoCache]
        public string GetBusinessLogicValidationForEdit(OtherAdjustmentDeductAll model)
        {
            //string errorMessage = string.Empty;
            StringBuilder errorMessage = new StringBuilder();
            var firstDate = Convert.ToDateTime(model.SalaryYear + "-" + model.SalaryMonth + "-01");

            var employeeSalary = (from s in _pgmCommonservice.PGMUnit.SalaryMasterRepository.GetAll()
                                  join emp in _pgmCommonservice.PGMUnit.FunctionRepository.GetEmployeeList() on s.EmployeeId equals emp.Id
                                  where s.SalaryYear == model.SalaryYear
                                      && s.SalaryMonth == model.SalaryMonth
                                      && emp.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId
                                  select s).ToList();

            if (employeeSalary.Count > 0)
            {
                errorMessage.AppendLine().Append("After processing salary, update is not acceptable for " + model.SalaryMonth + "-" + model.SalaryYear + ".");
            }

            if (Convert.ToDateTime(model.FromDate) > Convert.ToDateTime(model.ToDate))
            {
                errorMessage.AppendLine().Append("From month must be less then to month.");
            }

            if (
                    (model.FromJobGradeId != null && model.ToJobGradeId == null)
                    || (model.FromJobGradeId == null && model.ToJobGradeId != null)
                )
            {
                errorMessage.AppendLine().Append("Please select both job grade (From & To)");
            }

            var duplicate = _pgmCommonservice.PGMUnit.OtherAdjustmentDeductionAllRepository.GetAll()
                .Where(o => o.SalaryYear == model.SalaryYear
                                && o.SalaryMonth == model.SalaryMonth
                                && o.HeadType == model.HeadType
                                && o.SalaryHeadId == model.SalaryHeadId
                                && o.DesignationId == model.DesignationId
                                && o.StaffCategoryId == model.StaffCategoryId
                                && o.Id != model.Id)
                                .ToList();

            if (duplicate.Any())
            {
                errorMessage.AppendLine().Append("Duplicate Entry.");
            }


            return errorMessage.ToString();
        }

        [NoCache]
        private PGM_OtherAdjustDeductAll SetInsertUserAuditInfo(PGM_OtherAdjustDeductAll entity)
        {
            entity.IUser = User.Identity.Name;
            entity.IDate = DateTime.Now;

            return entity;
        }

        [NoCache]
        private PGM_OtherAdjustDeductAll SetEditUserAuditInfo(PGM_OtherAdjustDeductAll entity)
        {
            entity.EUser = User.Identity.Name;
            entity.EDate = DateTime.Now;
            return entity;
        }

        [NoCache]
        private void PrepareModel(OtherAdjustmentDeductAll model)
        {
            model.HeadType = type;
            model.SalaryYearList = SalaryYearList();

            if (model.Mode == "Add")
            {
                model.SalaryMonthList = Common.PopulateMonthList2();
            }
            else
            {
                model.SalaryMonthList = Common.PopulateMonthList();
            }

            model.SalaryHeadList = SalaryHeadAdjustmentList(model);
            model.EmploymentTypeList = EmploymentTypeList();
            model.AmountTypeList = GetAmountType();

            model.DepartmentList = GetDepartment();
            model.StaffCategoryList = GetStaffCategory();
            model.JobGradeList = GetJobGrade();
            model.DesignationList = GetDesignation();
        }

        private IList<SelectListItem> GetDepartment()
        {
            // Department strord in Division table !!
            var list = _pgmCommonservice.PGMUnit.DivisionRepository.GetAll().Where(d=> d.ZoneInfoId == LoggedUserZoneInfoId).ToList();
            return Common.PopulateDDLList(list);
        }

        private IList<SelectListItem> GetDesignation()
        {
            var list = _pgmCommonservice.PGMUnit.DesignationRepository.GetAll();
            return Common.PopulateDDLList(list);
        }

        private IList<SelectListItem> GetStaffCategory()
        {
            var list = _pgmCommonservice.PGMUnit.StaffCategoryRepository.GetAll();
            return Common.PopulateDDLList(list);
        }

        private IList<SelectListItem> GetJobGrade()
        {
            var list = _pgmCommonservice.GetLatestJobGrade();
            return Common.PopulateJobGradeDDL(list);
        }

        [NoCache]
        private IList<SelectListItem> GetAmountType()
        {
            return Common.PopulateAmountTypeForEarningDeduction().OrderBy(x => x.Text).ToList();
        }

        [NoCache]
        private IList<SelectListItem> EmploymentTypeList()
        {
            var list = _pgmCommonservice.PGMUnit.EmploymentTypeRepository.GetAll().ToList();
            return Common.PopulateDDLList(list);
        }

        [NoCache]
        public ActionResult GetEmploymentTypeList()
        {
            var list = _pgmCommonservice.PGMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.Name).ToList();

            var listItem = Common.PopulateDDLList(list);
            return PartialView("_SelectDesignation", listItem);
        }

        [NoCache]
        public ActionResult GetDesignationList()
        {
            var list = _pgmCommonservice.PGMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();

            var listItem = Common.PopulateDDLList(list);
            return PartialView("_SelectDesignation", listItem);
        }

        [NoCache]
        public ActionResult GetStaffCategoryList()
        {
            var list = _pgmCommonservice.PGMUnit.StaffCategoryRepository.GetAll().OrderBy(x => x.Name).ToList();

            var listItem = Common.PopulateDDLList(list);
            return PartialView("_SelectDesignation", listItem);
        }
        [NoCache]
        public ActionResult GetSalaryMonthList()
        {
            var SalaryMonth = new Dictionary<string, string>();

            foreach (var item in Common.PopulateMonthList())
            {
                SalaryMonth.Add(item.Text, item.Value);
            }

            ViewBag.SalaryMonthList = SalaryMonth;
            return PartialView("Select", SalaryMonth);
        }

        [NoCache]
        private IList<SelectListItem> SalaryYearList()
        {
            var SalaryYear = Common.PopulateYearList().DistinctBy(x => x.Value).ToList();
            return SalaryYear;
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
        public ActionResult GetSalaryHeadList()
        {
            string HeadType = string.Empty;

            HeadType = type == "Adjust" ? "Addition" : "Deduction";

            var list = _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(x => x.HeadType == HeadType).OrderBy(x => x.HeadName).ToList();

            var itemList = Common.PopulateSalaryHeadDDL(list);

            return PartialView("_SelectDesignation", itemList);
        }

        [NoCache]
        private void PrepareErrorModel(OtherAdjustmentDeductAll model)
        {
            var SalaryHeads = new List<SelectListItem>();

            SalaryHeads = _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(x => x.HeadType == model.HeadType && x.Id == model.SalaryHeadId).OrderBy(x => x.HeadName).ToList()
            .Select(y => new SelectListItem()
            {
                Text = y.HeadName,
                Value = y.Id.ToString()
            }).ToList();

            type = model.SelectedType;
        }

        [NoCache]
        public ActionResult SalaryHeadList(string AdjustDeductType)
        {
            var list = _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(x => x.HeadType == AdjustDeductType).OrderBy(x => x.HeadName).ToList();

            var itemList = Common.PopulateSalaryHeadDDL(list);

            return Json(itemList);
        }

        [NoCache]
        private IList<SelectListItem> SalaryHeadAdjustmentList(OtherAdjustmentDeductAll model)
        {
            string HeadType = model.HeadType;
            var list = _pgmCommonservice.PGMUnit.SalaryHeadRepository.Get(x => x.HeadType == HeadType).OrderBy(x => x.HeadName).ToList();

            return Common.PopulateSalaryHeadDDL(list);
        }

        private IList<PGM_OtherAdjustDeductAll> CreateEntityList(OtherAdjustmentDeductAll model)
        {
            var list = new List<PGM_OtherAdjustDeductAll>();

            var listYearMonth = GetListOfYearMonth(model);

            foreach (var item in listYearMonth)
            {
                model.SalaryYear = item.Year;
                model.SalaryMonth = item.Month;
                model.ZoneInfoId = LoggedUserZoneInfoId;

                model.IsDayWise = false;
                if (model.AmountType == PGMEnum.AmountType_SalaryAdjustmentAll.Day_Wise.ToString())
                    model.IsDayWise = true;

                var entity = model.ToEntity();
                entity = SetInsertUserAuditInfo(entity);

                list.Add(entity);
            }

            return list;
        }

        private IList<YearMonth> GetListOfYearMonth(OtherAdjustmentDeductAll model)
        {
            IList<YearMonth> listYearMonth = new List<YearMonth>();

            int intFromYear = Convert.ToInt32(model.FromYear);
            int intFromMonth = Convert.ToInt32(model.FromMonth);

            while (Convert.ToDateTime(intFromYear + "/" + intFromMonth + "/01") <= model.ToDate)
            {
                listYearMonth.Add(new YearMonth { Year = intFromYear.ToString(), Month = UtilCommon.GetMonthName(intFromMonth) });

                if (intFromMonth == 12)
                {
                    intFromMonth = 1;
                    intFromYear += 1;
                }
                else
                {
                    intFromMonth += 1;
                }
            }

            return listYearMonth;
        }

        #endregion
    }
}