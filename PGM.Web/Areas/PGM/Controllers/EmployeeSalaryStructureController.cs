
using DAL.PGM;
using Domain.PGM;
using Lib.Web.Mvc.JQuery.JqGrid;
using PGM.Web.Areas.PGM.Models.EmployeeSalaryStructure;
using PGM.Web.Areas.PGM.Models.SalaryStructure;
using PGM.Web.Controllers;
using PGM.Web.Resources;
using PGM.Web.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Utility;

namespace PGM.Web.Areas.PGM.Controllers
{
    public class EmployeeSalaryStructureController : BaseController
    {

        #region Fields
        private readonly PgmSalaryStructureService _salaryStructureService;
        private readonly PgmEmployeeSalaryStructureService _pgmEmpSalaryStructureService;
        #endregion

        #region Constructor
        public EmployeeSalaryStructureController(PgmSalaryStructureService salaryStructureService, PgmEmployeeSalaryStructureService empSSS)
        {
            this._salaryStructureService = salaryStructureService;
            this._pgmEmpSalaryStructureService = empSSS;
        }
        #endregion

        public ActionResult Index()
        {
            var model = new EmployeeInfoModel();
            model.ZoneInfoId = LoggedUserZoneInfoId;
            model.ActionName = "EditEmploymentInfo";

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetList(JqGridRequest request, EmployeeInfoModel model)
        {
            var list = (from emp in _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeList(LoggedUserZoneInfoId)

                        where
                        (string.IsNullOrEmpty(model.FullName) || (emp.FullName ?? String.Empty).ToLower().Contains((model.FullName ?? String.Empty).ToLower()))
                        && (string.IsNullOrEmpty(model.EmpID) || model.EmpID == emp.EmpID)
                        && (string.IsNullOrEmpty(model.WorkingZoneName) || (emp.ZoneName ?? String.Empty).ToLower().Contains((model.WorkingZoneName ?? String.Empty).ToLower()))
                        && (model.DesignationId == 0 || emp.DesignationId == model.DesignationId)
                        && (model.JobGradeId == 0 || emp.JobGradeId == model.JobGradeId)
                        && (model.EmploymentTypeId == 0 || emp.EmploymentTypeId == model.EmploymentTypeId)
                        && (model.StaffCategoryId == 0 || emp.StaffCategoryId == model.StaffCategoryId)
                        && (model.EmployeeStatusId == 0 || emp.EmploymentStatusId == model.EmployeeStatusId)
                        && (model.IsBonusEligible == null || emp.IsBonusEligible == model.IsBonusEligible)
                        && (model.IsOvertimeEligible == null || emp.IsOvertimeEligible == model.IsOvertimeEligible)
                        && (model.IsRefreshmentEligible == null || emp.IsRefreshmentEligible == model.IsRefreshmentEligible)
                        && (model.IsGPFEligible == null || emp.IsGPFEligible == model.IsGPFEligible)

                        select new EmployeeInfoModel
                        {
                            Id = emp.Id,
                            FullName = emp.FullName,
                            EmpID = emp.EmpID,
                            DesignationName = emp.DesignationShortName,
                            BankAccountNo = emp.BankAccountNo,
                            JobGradeId = emp.JobGradeId,
                            JobGradeName = emp.JobGradeName,
                            DateofJoining = emp.DateofJoining,
                            DateofConfirmation = emp.DateofConfirmation,
                            EmploymentTypeId = emp.EmploymentTypeId,
                            EmpTypeName = emp.EmpTypeName,
                            StaffCategoryId = emp.StaffCategoryId,
                            StaffCategoryName = emp.StaffCategoryName,
                            EmployeeStatusId = emp.EmploymentStatusId,
                            EmpStatusName = emp.EmpStatusName,
                            WorkingZoneId = emp.ZoneInfoId,
                            WorkingZoneName = emp.ZoneName,
                            IsBonusEligible = emp.IsBonusEligible,
                            IsOvertimeEligible = emp.IsOvertimeEligible,
                            IsRefreshmentEligible = emp.IsRefreshmentEligible,
                            IsGPFEligible = emp.IsGPFEligible,
                            DesigSortOrder = emp.SortingOrder,
                            PFMembershipDate = emp.PFMembershipDate
                        }).OrderBy(o => o.DesigSortOrder).ToList();

            #region Sorting

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

            if (request.SortingName == "DesignationId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DesignationName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DesignationName).ToList();
                }
            }

            if (request.SortingName == "JobGradeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.JobGradeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.JobGradeName).ToList();
                }
            }

            if (request.SortingName == "DateofJoining")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateofJoining).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateofJoining).ToList();
                }
            }

            if (request.SortingName == "DateofConfirmation")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.DateofConfirmation).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.DateofConfirmation).ToList();
                }
            }

            if (request.SortingName == "EmploymentTypeId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpTypeName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpTypeName).ToList();
                }
            }

            if (request.SortingName == "EmployeeStatusId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.EmpStatusName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.EmpStatusName).ToList();
                }
            }

            if (request.SortingName == "WorkingZoneId")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.WorkingZoneName).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.WorkingZoneName).ToList();
                }
            }

            if (request.SortingName == "IsBonusEligible")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsBonusEligible).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsBonusEligible).ToList();
                }
            }

            if (request.SortingName == "IsOvertimeEligible")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsOvertimeEligible).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsOvertimeEligible).ToList();
                }
            }

            if (request.SortingName == "IsRefreshmentEligible")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsRefreshmentEligible).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsRefreshmentEligible).ToList();
                }
            }

            if (request.SortingName == "IsGPFEligible")
            {
                if (request.SortingOrder.ToString().ToLower() == "asc")
                {
                    list = list.OrderBy(x => x.IsGPFEligible).ToList();
                }
                else
                {
                    list = list.OrderByDescending(x => x.IsGPFEligible).ToList();
                }
            }

            #endregion

            int totalRecords = list == null ? 0 : list.ToList().Count;
            int pageSize = request.PagesCount.HasValue ? request.PagesCount.Value : 1;

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * pageSize).ToList();

            JqGridResponse response = new JqGridResponse
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.FullName,
                    item.EmpID,
                    item.DesignationName,
                    item.BankAccountNo,
                    item.JobGradeName,
                    item.DateofJoining,
                    item.DateofConfirmation,
                    item.EmpTypeName,
                    item.StaffCategoryName,
                    item.EmpStatusName,
                    item.WorkingZoneName,
                    item.IsBonusEligible,
                    item.IsOvertimeEligible,
                    item.IsRefreshmentEligible,
                    item.IsGPFEligible
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [NoCache]
        public ActionResult GetListSalaryZoneChangeHistory(JqGridRequest request, int employeeId, EmployeeInfoModel model)
        {
            var list = (from emp in _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeList()
                        join ZH in _pgmEmpSalaryStructureService.PGMUnit.SalaryZoneChangeHistoryRepository.GetAll() on emp.Id equals ZH.EmployeeId
                        join Z in _pgmEmpSalaryStructureService.PGMUnit.ZoneInfoRepository.GetAll() on ZH.SalaryWithdrawFromZoneId equals Z.Id

                        where emp.Id == employeeId

                        select new SalaryZoneChangeHistoryModel
                        {
                            Id = ZH.Id,
                            SalaryZoneName = Z.ZoneName,
                            ChangeDate = ZH.ChangeDate,
                            IsInactive = emp.DateofInactive != null
                        }).OrderBy(p => p.ChangeDate).ToList();

            int totalRecords = list == null ? 0 : list.ToList().Count;
            int pageSize = request.PagesCount.HasValue ? request.PagesCount.Value : 1;

            list = list.Skip(request.PageIndex * request.RecordsCount).Take(request.RecordsCount * pageSize).ToList();

            JqGridResponse response = new JqGridResponse
            {
                TotalPagesCount = (int)Math.Ceiling((float)totalRecords / (float)request.RecordsCount),
                PageIndex = request.PageIndex,
                TotalRecordsCount = totalRecords
            };

            foreach (var item in list)
            {
                response.Records.Add(new JqGridRecord(Convert.ToString(item.Id), new List<object>()
                {
                    item.Id,
                    item.SalaryZoneName,
                    item.ChangeDate.ToString(DateAndTime.GlobalDateFormat),
                    item.IsInactive
                }));
            }
            return new JqGridJsonResult() { Data = response };
        }

        #region Employee Information

        #region Update--------------------------------------

        [NoCache]
        public ActionResult EditEmploymentInfo(int id, string type)
        {
            var parentModel = new EmployeeModel();
            parentModel.ViewType = "EmploymentInfo";

            var emp = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(id);
            var model = emp.ToModel();

            #region Job Grade ddl Or Textbox
            var salaryProcess = _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetAll().Where(x => x.EmployeeId == id).ToList();
            if (salaryProcess.Count > 0)
            {
                model.IsSalaryAlreadyProcessed = true;
            }
            else
            {
                model.IsSalaryAlreadyProcessed = false;
            }
            #endregion

            PopulateDropdown(model);

            model.WorkingZoneId = emp.ZoneInfoId;
            model.ActionType = "EditEmploymentInfo";
            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "active";

            parentModel.EmploymentInfo = model;
            parentModel.Id = model.Id;
            parentModel.EmpId = model.EmpID;

            if (type == "success")
            {
                parentModel.EmploymentInfo.Message = ErrorMessages.InsertSuccessful;
                parentModel.EmploymentInfo.IsError = 0;
            }

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        [NoCache]
        public ActionResult EditEmploymentInfo(EmployeeInfoModel model)
        {
            var parentModel = new EmployeeModel();
            parentModel.ViewType = "EmploymentInfo";

            model.ButtonText = "Update";
            model.DeleteEnable = true;
            model.SelectedClass = "active";
            model.IsError = 0;

            #region Job Grade ddl Or Textbox
            model.IsSalaryAlreadyProcessed = _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetAll().Where(x => x.EmployeeId == model.Id).ToList().Count > 0;
            #endregion

            var error = CheckEmpInfoBusinessRule(model);

            if (ModelState.IsValid && error == string.Empty)
            {
                #region Update Success
                try
                {
                    bool result = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.UpdateEmploymentInfo(
                        model.Id,
                        model.JobGradeId,
                        model.BankId,
                        model.BankBranchId,
                        model.BankAccountNo,
                        model.SalaryWithdrawFromZoneId,
                        model.WorkingZoneId,
                        model.TaxRegionId,
                        model.TaxAssesseeType,
                        model.HavingChildWithDisability,
                        Convert.ToBoolean(model.IsBonusEligible),
                        Convert.ToBoolean(model.IsOvertimeEligible),
                        Convert.ToBoolean(model.IsGPFEligible),
                        Convert.ToBoolean(model.IsPensionEligible),
                        model.IsLeverageEligible,
                        Convert.ToBoolean(model.IsRefreshmentEligible),
                        model.MembershipStatus,
                        User.Identity.Name,
                        model.EmialAddress,
                        model.MobileNo);

                    if (result)
                    {
                        model.Message = ErrorMessages.UpdateSuccessful;
                    }
                    else
                    {
                        model.Message = ErrorMessages.UpdateFailed;
                        model.IsError = 1;
                    }
                }
                catch (Exception ex)
                {
                    PopulateDropdown(model);
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    model.IsError = 1;

                    parentModel.EmploymentInfo = model;
                    return View("CreateOrEdit", parentModel);
                }

                PopulateDropdown(model);

                parentModel.EmploymentInfo = model;
                parentModel.Id = model.Id;
                parentModel.EmpId = model.EmpID;

                #endregion

                return View("CreateOrEdit", parentModel);
            }

            PopulateDropdown(model);

            model.Message = ErrorMessages.UpdateFailed + " " + error;
            model.IsError = 1;

            parentModel.EmploymentInfo = model;

            return View("CreateOrEdit", parentModel);
        }

        #endregion

        private string CheckEmpInfoBusinessRule(EmployeeInfoModel model)
        {
            // for update
            if (model.Id > 0)
            {
                // Is salary exist
                var salaryExisting = _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.Get(x => x.EmployeeId == model.Id).FirstOrDefault();
                if (salaryExisting != null)
                {
                    model.IsSalaryAlreadyProcessed = true;

                    if (salaryExisting.isConsolidated && model.EmploymentTypeId != 2) // 2=contructual employee
                    {
                        return "Contructual salary structure is assigned already! please, select appropriate employment type.";
                    }
                    else if (salaryExisting.isConsolidated == false && model.EmploymentTypeId == 2)
                    {
                        return "Regular salary structure is assigned already! please, select appropriate employment type.";
                    }
                }

                // Restricting change of salary zone if salary process already done.
                var currentSalaryZone = _pgmEmpSalaryStructureService
                    .PGMUnit
                    .FunctionRepository
                    .GetEmployeeById(model.Id)
                    .SalaryWithdrawFromZoneId;
                var proposedSalaryZone = model.SalaryWithdrawFromZoneId;
                if (currentSalaryZone != proposedSalaryZone)
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("SELECT * FROM PGM_Salary sal");
                    query.Append(" WHERE sal.EmployeeId = " + model.Id);
                    query.Append(" AND '" + DateTime.Now.ToString("yyyy") + "' = sal.SalaryYear");
                    query.Append(" AND '" + DateTime.Now.ToString("MMMM") + "' = sal.SalaryMonth");

                    var salaryChecking = _pgmEmpSalaryStructureService.PGMUnit.SalaryMasterRepository.GetWithRawSql(query.ToString())
                        .FirstOrDefault();

                    if (salaryChecking != null)
                    {
                        return "Salary has been processed for this month (" + salaryChecking.SalaryMonth + "/" + salaryChecking.SalaryYear + ")." + Environment.NewLine
                            + " You cannot not change salary zone this month. Please try next month or rollback salary first. ";
                    }
                }
            }

            if (Common.GetInteger(model.TaxRegionId) == 0)
            {
                return "Please fill up tax retion.";
            }

            if (Common.GetInteger(model.TaxAssesseeType) == 0)
            {
                return "Please fill up tax asessee type.";
            }

            return string.Empty;
        }

        private void PopulateDropdown(EmployeeInfoModel model)
        {
            #region Job Grade
            model.JobGradeList = Common.PopulateJobGradeDDL(_pgmEmpSalaryStructureService.GetLatestJobGrade());
            #endregion

            #region Bank Name
            var bankNameList = _pgmEmpSalaryStructureService.PGMUnit.AccBankRepository.Fetch().OrderBy(x => x.bankName).ToList();
            model.BankList = Common.PopulateDDLBankList(bankNameList);
            #endregion

            #region Branch Name
            var branchNameList = _pgmEmpSalaryStructureService.PGMUnit.AccBankBranchRepository.Fetch().OrderBy(x => x.branchName).ToList();
            model.BankBranchList = Common.PopulateDDLBankBranchList(branchNameList);
            #endregion

            #region Salary withdraw From
            model.SalaryWithdrawFromList = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetZoneInfoList()
                .Select(y => new SelectListItem()
                {
                    Text = y.ZoneName,
                    Value = y.Id.ToString()
                })
                .ToList();
            #endregion

            #region Working Zone
            model.WorkingZoneList = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetZoneInfoList()
                .Select(y => new SelectListItem()
                {
                    Text = y.ZoneName,
                    Value = y.Id.ToString()
                })
                .ToList();
            #endregion

            #region Tax Region
            model.TaxRegionList = _pgmEmpSalaryStructureService.PGMUnit.TaxRegionRuleRepository.GetAll().Where(x => x.IsActive == true).ToList()
                .Select(y =>
                new SelectListItem()
                {
                    Text = y.RegionName,
                    Value = y.Id.ToString()
                }).ToList();
            #endregion

            #region Assessee Type
            Dictionary<int, string> assesseeList = Common.GetEnumAsDictionary<PGMEnum.TaxAssesseeType>();
            foreach (KeyValuePair<int, string> item in assesseeList)
            {
                model.AssesseTypeList.Add(new SelectListItem() { Text = item.Value, Value = item.Key.ToString() });
            }
            #endregion

            #region PF Membership Stauts
            model.MembershipStatusList = Common.PopulatePFMembershipStatusList();
            #endregion
        }

        #endregion

        #region Employee Salary Structure

        #region Insert

        [NoCache]
        public ActionResult CreateSalaryStructure(int id, string type, string delType, string insertType)
        {
            var parentModel = new EmployeeModel();
            var model = parentModel.EmployeeSalary;
            var empSalary = _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");// == null ? new PRM_EmpSalary() : _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
            var employee = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(id);

            model.EmployeeId = employee.Id;
            model.EmpId = employee.EmpID;
            model.FullName = employee.FullName;
            model.DateofInactive = employee.DateofInactive;
            model.Designation = employee.DesignationName;
            model.PFMembershipStatus = employee.MembershipStatus;
            if (model.PFMembershipStatus == "Active")
            {
                model.PFMembershipStatus = model.PFMembershipStatus + " {Active From: " + Convert.ToDateTime(employee.PFMembershipDate).ToString(DateAndTime.GlobalDateFormat) + "}";
            }

            model.HouseRentRegionAndAmount = employee.RegionName;
            var houseRent =
                _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetHouseRentConsiderRule(DateTime.Now,
                    DateTime.Now, employee.SalaryScaleId, Convert.ToInt32(employee.RegionId), employee.BasicSalary, employee.GrossSalary);
            // "4" : problem region id not found: so houserent is null
            if (houseRent != null)
            {
                model.HouseRentRegionAndAmount = model.HouseRentRegionAndAmount + " {" + houseRent.HouseRent.ToString() + "}";
            }

            if (empSalary != null)
            {
                model.SalaryStructureId = empSalary.SalaryStructureId;
                model.SalaryScaleId = Convert.ToInt32(empSalary.SalaryScaleId);
                model.GradeId = empSalary.GradeId;
                model.StepId = empSalary.StepId;
                model.isConsolidated = empSalary.isConsolidated;
                model.GrossSalary = empSalary.GrossSalary;
                model.OrgGrossSalary = empSalary.GrossSalary;
            }
            else
            {
                empSalary = new PRM_EmpSalary();
                if (employee.IsContractual)
                {
                    model.isConsolidated = true;

                    model.GradeId = employee.JobGradeId;
                    var step = (from tr in _pgmEmpSalaryStructureService.PGMUnit.JobGradeStepRepository.Get(p => p.JobGradeId == model.GradeId)
                                select tr.Id).FirstOrDefault();

                    if (step > 0)
                    {
                        model.StepId = step;
                    }
                    //model.StepId = 1; // deafult
                }
            }
            PopulateDropdown(model);

            GettingSalaryStructureDetailList(model);
            GetSalaryHeadAmountTypeSetting(model);

            #region Button and event----------------------------------

            model.SelectedClass = "active";
            if (empSalary.EmployeeId == 0)
            {
                model.ActionType = "CreateSalaryStructure";
                model.ButtonText = "Save";
            }
            else
            {
                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
            }

            if (type == "success" || insertType == "success")
            {
                model.Message = ErrorMessages.InsertSuccessful;
                model.IsError = 0;
            }

            if (delType == "success")
            {
                model.Message = ErrorMessages.DeleteSuccessful;
                model.IsError = 0;
            }
            #endregion

            #region Job Grade ddl Or Textbox
            var salaryProcess = _pgmEmpSalaryStructureService.PGMUnit.SalaryMasterRepository.Get(x => x.EmployeeId == model.EmployeeId).Any();
            if (salaryProcess)
            {
                model.IsSalaryProcess = true;
            }
            else
            {
                model.IsSalaryProcess = false;
            }
            #endregion

            parentModel.ViewType = "EmployeeSalaryStructure";
            parentModel.EmployeeSalary = model;
            parentModel.Id = employee.Id;
            parentModel.EmpId = employee.EmpID;

            return View("CreateOrEdit", parentModel);
        }

        [HttpPost]
        [NoCache]
        public ActionResult CreateSalaryStructure(EmployeeSalaryStructureModel model)
        {
            var parentModel = new EmployeeModel();
            parentModel.ViewType = "EmployeeSalaryStructure";

            var errorList = CheckEmpSalaryBusinessRule(model);

            if (ModelState.IsValid)
            {
                if (errorList.Count > 0)
                {
                    PopulateDropdown(model);

                    model.Message = errorList.FirstOrDefault(); //Resources.ErrorMessages.InsertFailed;
                    model.IsError = 0;

                    model.ActionType = "CreateSalaryStructure";
                    model.ButtonText = "Save";
                    model.SelectedClass = "active";

                    parentModel.EmployeeSalary = model;
                    parentModel.Id = model.EmployeeId;

                    GetSalaryHeadAmountTypeSetting(model);

                    return View("CreateOrEdit", parentModel);
                }
                else
                {
                    if (model.SalaryStructureId == 0 && model.SalaryScaleId > 0 && model.GradeId > 0 && model.StepId > 0)
                    {
                        model.SalaryStructureId = _pgmEmpSalaryStructureService.PGMUnit.SalaryStructureRepository
                            .GetAll().FirstOrDefault(s => s.SalaryScaleId == model.SalaryScaleId && s.GradeId == model.GradeId && s.StepId == model.StepId).Id;
                    }

                    #region EmploymentInfo

                    var employee = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                    _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.UpdateEmploymentInfo(
                        employee.Id,
                        model.GradeId,
                        employee.BankId,
                        employee.BankBranchId,
                        employee.BankAccountNo,
                        employee.SalaryWithdrawFromZoneId,
                        employee.ZoneInfoId,
                        employee.TaxRegionId,
                        employee.TaxAssesseeType,
                        Convert.ToBoolean(employee.HavingChildWithDisability),
                        employee.IsBonusEligible,
                        employee.IsOvertimeEligible,
                        employee.IsGPFEligible,
                        employee.IsPensionEligible,
                        employee.IsLeverageEligible,
                        employee.IsRefreshmentEligible,
                        employee.MembershipStatus,
                        User.Identity.Name,
                        employee.EmialAddress,
                        employee.MobileNo);

                    #endregion

                    #region Master
                    var empSalaryEntity = new PRM_EmpSalary()
                    {
                        EmployeeId = model.EmployeeId,
                        SalaryStructureId = model.SalaryStructureId,
                        SalaryScaleId = model.SalaryScaleId,
                        GradeId = model.GradeId,
                        StepId = model.StepId,
                        GrossSalary = Math.Round(model.GrossSalary),
                        isConsolidated = model.isConsolidated,

                        IUser = User.Identity.Name,
                        IDate = Common.CurrentDateTime,
                    };
                    #endregion

                    #region Details
                    var empSalaryDetailsEntityList = empSalaryEntity.PRM_EmpSalaryDetail;//new List<PRM_EmpSalaryDetail>();

                    foreach (var item in model.SalaryStructureDetail)
                    {
                        var empSalaryDetailsEntity = new PRM_EmpSalaryDetail()
                        {
                            EmployeeId = model.EmployeeId,
                            HeadId = item.HeadId,
                            HeadType = item.HeadType,
                            AmountType = item.AmountType,
                            IsTaxable = item.IsTaxable,
                            Amount = item.Amount,

                            IUser = User.Identity.Name,
                            IDate = Common.CurrentDateTime
                        };
                        empSalaryDetailsEntityList.Add(empSalaryDetailsEntity);
                    }
                    #endregion

                    #region Save
                    try
                    {
                        _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.Add(empSalaryEntity);
                        _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                        model.IsError = 0;

                        model.ActionType = "CreateSalaryStructure";
                        model.ButtonText = "Save";
                        model.SelectedClass = "active";

                        parentModel.EmployeeSalary = model;
                        parentModel.Id = model.EmployeeId;

                        PopulateDropdown(model);
                        GetSalaryHeadAmountTypeSetting(model);

                        return View("CreateOrEdit", parentModel);
                    }
                    #endregion
                }
            }
            else
            {
                #region Process save failed

                model.Message = ErrorMessages.InsertFailed;
                model.IsError = 0;

                model.ActionType = "CreateSalaryStructure";
                model.ButtonText = "Save";
                model.SelectedClass = "active";

                parentModel.EmployeeSalary = model;
                parentModel.Id = model.EmployeeId;

                PopulateDropdown(model);
                GetSalaryHeadAmountTypeSetting(model);

                return View("CreateOrEdit", parentModel);

                #endregion
            }

            #region Save success and repopulate the form

            return RedirectToAction("CreateSalaryStructure", "EmployeeSalaryStructure", new { Id = model.EmployeeId, insertType = "success" });

            #endregion
        }

        #endregion

        #region Update

        [HttpPost]
        [NoCache]
        public ActionResult EditSalaryStructure(EmployeeSalaryStructureModel model)
        {
            var parentModel = new EmployeeModel();
            parentModel.ViewType = "EmployeeSalaryStructure";

            var errorList = CheckEmpSalaryBusinessRule(model);

            if (errorList.Count > 0)
            {
                PopulateDropdown(model);

                model.Message = errorList.FirstOrDefault(); //Resources.ErrorMessages.InsertFailed;
                model.IsError = 0;

                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                parentModel.EmployeeSalary = model;
                parentModel.Id = model.EmployeeId;

                GetSalaryHeadAmountTypeSetting(model);

                return View("CreateOrEdit", parentModel);
            }

            if (ModelState.IsValid)
            {
                #region EmploymentInfo

                var employee = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.UpdateEmploymentInfo(
                    employee.Id, model.GradeId,
                    employee.BankId,
                    employee.BankBranchId,
                    employee.BankAccountNo,
                    employee.SalaryWithdrawFromZoneId,
                    employee.ZoneInfoId,
                    employee.TaxRegionId,
                    employee.TaxAssesseeType,
                    Convert.ToBoolean(employee.HavingChildWithDisability),
                    employee.IsBonusEligible,
                    employee.IsOvertimeEligible,
                    employee.IsGPFEligible,
                    employee.IsPensionEligible,
                    employee.IsLeverageEligible,
                    employee.IsRefreshmentEligible,
                    employee.MembershipStatus,
                    User.Identity.Name,
                    employee.EmialAddress,
                    employee.MobileNo);

                #endregion

                #region Master
                var empSalary = _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetByID(model.EmployeeId, "EmployeeId");

                empSalary.EmployeeId = model.EmployeeId;
                empSalary.SalaryStructureId = model.SalaryStructureId;
                empSalary.SalaryScaleId = model.SalaryScaleId;
                empSalary.GradeId = model.GradeId;
                empSalary.StepId = model.StepId;
                empSalary.GrossSalary = Math.Round(model.GrossSalary);
                empSalary.isConsolidated = model.isConsolidated;

                empSalary.EUser = User.Identity.Name;
                empSalary.EDate = Common.CurrentDateTime;
                #endregion

                #region Details
                //delete all existing first
                empSalary.PRM_EmpSalaryDetail.ToList().ForEach(x => _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryDetailRepository.Delete(x.Id));

                var lstChild = new ArrayList();
                PRM_EmpSalaryDetail empSalaryDetailNew;
                foreach (var item in model.SalaryStructureDetail)
                {
                    empSalaryDetailNew = new PRM_EmpSalaryDetail();

                    empSalaryDetailNew.EmployeeId = model.EmployeeId;
                    empSalaryDetailNew.HeadId = item.HeadId;
                    empSalaryDetailNew.HeadType = item.HeadType;
                    empSalaryDetailNew.AmountType = item.AmountType;
                    empSalaryDetailNew.IsTaxable = item.IsTaxable;
                    empSalaryDetailNew.Amount = item.Amount;

                    empSalaryDetailNew.IUser = User.Identity.Name;
                    empSalaryDetailNew.IDate = Common.CurrentDateTime;
                    empSalaryDetailNew.EUser = User.Identity.Name;
                    empSalaryDetailNew.EDate = Common.CurrentDateTime;

                    lstChild.Add(empSalaryDetailNew);
                }

                var navigationList = new Dictionary<Type, ArrayList>();
                navigationList.Add(typeof(PRM_EmpSalaryDetail), lstChild);
                #endregion

                #region Update
                try
                {
                    _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.Update(empSalary, "EmployeeId", navigationList);
                    _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.SaveChanges();
                }
                catch (Exception ex)
                {
                    model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
                    model.IsError = 0;

                    model.ActionType = "CreateSalaryStructure";
                    model.ButtonText = "Update";
                    model.DeleteEnable = true;
                    model.SelectedClass = "active";

                    parentModel.EmployeeSalary = model;
                    parentModel.Id = model.EmployeeId;

                    GetSalaryHeadAmountTypeSetting(model);

                    return View("CreateOrEdit", parentModel);
                }
                #endregion
            }
            else
            {
                #region Process save failed

                model.Message = ErrorMessages.InsertFailed;
                model.IsError = 0;

                model.ActionType = "CreateSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                parentModel.EmployeeSalary = model;
                parentModel.Id = model.EmployeeId;

                GetSalaryHeadAmountTypeSetting(model);

                return View("CreateOrEdit", parentModel);

                #endregion
            }

            // Save success
            return RedirectToAction("CreateSalaryStructure", "EmployeeSalaryStructure", new { Id = model.EmployeeId, type = "success" });
        }

        #endregion

        #region Delete

        [NoCache]
        public ActionResult DeleteSalaryStructure(int id)
        {
            //var empSalary = _empService.PRMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
            //var empSalaryDetails = empSalary.PRM_EmpSalaryDetail;

            var empSalaryStatusChange = (from ss in _pgmEmpSalaryStructureService.PGMUnit.EmpStatusChangeRepository.GetAll()
                                         where ss.EmployeeId == id
                                         select ss).LastOrDefault();
            if (empSalaryStatusChange != null)
            {
                var parentModel = new EmployeeModel();
                var model = parentModel.EmployeeSalary;
                var empSalary = _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId") == null ? new PRM_EmpSalary() : _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
                var employee = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(id);

                model.EmployeeId = employee.Id;
                model.EmpId = employee.EmpID;
                model.FullName = employee.FullName;

                PopulateDropdown(model);

                model.GradeId = empSalary.GradeId;
                model.StepId = empSalary.StepId;
                model.isConsolidated = empSalary.isConsolidated;
                model.GrossSalary = empSalary.GrossSalary;

                if (employee.IsContractual)
                    model.isConsolidated = true;

                model.Message = "Employee promotion or increment or confirmation is already exist. So you can not delete it.";
                model.IsError = 1;

                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                GettingSalaryStructureDetailList(model);
                GetSalaryHeadAmountTypeSetting(model);

                parentModel.ViewType = "EmployeeSalaryStructure";
                parentModel.EmployeeSalary = model;
                parentModel.Id = employee.Id;
                parentModel.EmpId = employee.EmpID;

                return View("CreateOrEdit", parentModel);
            }

            try
            {
                var allTypes = new List<Type> { typeof(PRM_EmpSalaryDetail) };
                _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.Delete(id, "EmployeeId", allTypes);
                _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                var parentModel = new EmployeeModel();
                var model = parentModel.EmployeeSalary;
                var empSalary = _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId") == null ? new PRM_EmpSalary() : _pgmEmpSalaryStructureService.PGMUnit.EmpSalaryRepository.GetByID(id, "EmployeeId");
                var employee = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(id);

                model.EmployeeId = employee.Id;
                model.EmpId = employee.EmpID;
                model.FullName = employee.FullName;

                PopulateDropdown(model);

                model.GradeId = empSalary.GradeId;
                model.StepId = empSalary.StepId;
                model.isConsolidated = empSalary.isConsolidated;
                model.GrossSalary = empSalary.GrossSalary;

                if (employee.IsContractual)
                    model.isConsolidated = true;

                model.Message = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                model.IsError = 1;

                model.ActionType = "EditSalaryStructure";
                model.ButtonText = "Update";
                model.DeleteEnable = true;
                model.SelectedClass = "active";

                GettingSalaryStructureDetailList(model);
                GetSalaryHeadAmountTypeSetting(model);

                parentModel.ViewType = "EmployeeSalaryStructure";
                parentModel.EmployeeSalary = model;
                parentModel.Id = employee.Id;
                parentModel.EmpId = employee.EmpID;

                return View("CreateOrEdit", parentModel);
            }

            return RedirectToAction("CreateSalaryStructure", "EmployeeSalaryStructure", new { Id = id, delType = "success" });
        }

        #endregion

        [HttpGet]
        [NoCache]
        public ActionResult GetSalaryStructureDetails(int gradeId, int stepId, int empId, bool IsConsolidated)
        {
            var model = new EmployeeSalaryStructureModel();
            model.EmployeeId = empId;
            var salaryStructureId = 0;

            var salaryHeads = _pgmEmpSalaryStructureService.PGMUnit.SalaryHeadRepository.Fetch().OrderBy(s => s.SortOrder).ToList();
            var salaryStructureDetails = _salaryStructureService.GetSalaryStrutureDetails(gradeId, stepId, out salaryStructureId);

            if (salaryStructureDetails.Any()) //new
            {

                var empSalaryDetails = _pgmEmpSalaryStructureService.GetEmpSalaryStructureDetails(model.EmployeeId, out salaryStructureId);

                decimal basicAmount = 0;
                foreach (var item in salaryStructureDetails)
                {
                    if (item.PRM_SalaryHead.HeadType == "Provision") continue;

                    if (empSalaryDetails.Count > 0)
                    {
                        var empSalary = empSalaryDetails.FirstOrDefault(e => e.HeadId == item.HeadId);
                        if (!item.PRM_SalaryHead.IsBasicHead)
                        {
                            item.AmountType = empSalary.AmountType;
                            item.Amount = empSalary.Amount;
                        }
                    }

                    var ssdModel = item.ToModel();
                    ssdModel.AmountType = item.AmountType;
                    ssdModel.HeadAmountTypeList = Common.PopulateAmountType().ToList();
                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;
                    ssdModel.IsGrossPayHead = salaryHeads.Find(x => x.Id == item.HeadId).IsGrossPayHead;

                    if (Convert.ToBoolean(item.PRM_SalaryHead.IsHouseRentHead))
                    {
                        basicAmount = salaryStructureDetails.FirstOrDefault(s => s.PRM_SalaryHead.IsBasicHead).Amount;

                        var employee = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);
                        var houseRent =
                            _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetHouseRentConsiderRule(DateTime.Now,
                                DateTime.Now, employee.SalaryScaleId, Convert.ToInt32(employee.RegionId), basicAmount, employee.GrossSalary);

                        model.HouseRentByRule = 0;
                        if (houseRent != null)
                        {
                            model.HouseRentByRule = houseRent.HouseRent;
                            model.HouseRentSalaryHeadId = houseRent.HouseRentSalaryHeadId;
                        }

                        ssdModel.Amount = Convert.ToDecimal(model.HouseRentByRule);
                        ssdModel.AmountType = PGMEnum.AmountType.Fixed.ToString();
                    }

                    // check consolidated structure, if yes then amount type should be percent and amount must be zero
                    if (ssdModel.IsGrossPayHead == true && IsConsolidated == true)
                    {
                        ssdModel.AmountType = "Percent";
                        ssdModel.Amount = Math.Round(Convert.ToDecimal(0), 2);
                    }

                    model.SalaryStructureDetail.Add(ssdModel);
                }
            }

            model.TotalAddition = model.SalaryStructureDetail.Where(s => s.HeadType == "Addition").Sum(x => x.Amount);
            model.TotalDeduction = model.SalaryStructureDetail.Where(s => s.HeadType == "Deduction").Sum(x => x.Amount);
            model.NetPay = model.TotalAddition - model.TotalDeduction;
            model.SalaryStructureId = salaryStructureId;

            return PartialView("_SalaryStructureDetail", model);
        }


        #region Private Methods
        private void PopulateDropdown(EmployeeSalaryStructureModel model)
        {
            var employeeInfos = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(model.EmployeeId);

            #region Salary Scale

            model.SalaryScaleId = employeeInfos.SalaryScaleId;
            model.SalaryScale = (from jg in _pgmEmpSalaryStructureService.PGMUnit.SalaryScaleRepository.GetAll()
                                 where jg.Id == employeeInfos.SalaryScaleId
                                 select jg.SalaryScaleName).FirstOrDefault();

            #endregion

            #region Job Grade
            model.JobGrade = (from jg in _pgmEmpSalaryStructureService.PGMUnit.JobGradeRepository.GetAll()
                              where jg.Id == employeeInfos.JobGradeId
                              select jg.GradeName).FirstOrDefault();

            model.GradeId = employeeInfos.JobGradeId;

            //int staffCategory = _empService.PRMUnit.EmploymentInfoRepository.GetByID(model.EmployeeId).StaffCategoryId;
            //model.GradeList = Common.PopulateJobGradeDDL(_empService.PRMUnit.JobGradeRepository.Get(q => q.StaffCategoryId == staffCategory && q.IsConsolidated == model.isConsolidated).ToList());
            #endregion

            #region Step Number
            dynamic ddlList;
            ddlList = _pgmEmpSalaryStructureService.PGMUnit.JobGradeStepRepository.Get(d => d.JobGradeId == employeeInfos.JobGradeId).OrderBy(x => x.Id).ToList();
            model.StepList = Common.PopulateStepList(ddlList);

            // if consolidated then first setp number shoud be selected
            if (model.isConsolidated)
            {
                if (model.GradeId > 0 && model.StepId > 0)
                {
                    foreach (var item in model.StepList)
                    {
                        if (item.Value == model.StepId.ToString()) item.Selected = true;

                    }
                }
            }

            #endregion
        }

        private List<string> CheckEmpSalaryBusinessRule(EmployeeSalaryStructureModel model)
        {
            var errorList = new List<string>();

            if (model.SalaryStructureDetail.Count == 0)
            {
                errorList.Add("No salary details found !!");

                return errorList;
            }

            if (model.isConsolidated && model.GrossSalary == 0)
            {
                errorList.Add("For consolidated structure, Gross salary must be entered by the user.");
            }
            //if (model.isConsolidated != _empService.PRMUnit.JobGradeRepository.GetByID(model.GradeId).IsConsolidated)
            //{
            //    if (model.isConsolidated) errorList.Add("Please select consolidated grade from grade list for this employee.");
            //    else errorList.Add("Please select non consolidated grade from grade list.");
            //}

            if (model.SalaryStructureDetail.Any(q => q.AmountType == "Percent" && q.Amount > 100))
            {
                errorList.Add("Amount can't exceed 100 for amount type 'Percent'.");
            }

            if (model.SalaryStructureDetail.Where(q => _pgmEmpSalaryStructureService.PGMUnit.SalaryHeadRepository.GetByID(q.HeadId).IsGrossPayHead
                && q.AmountType == "Percent").Sum(q => q.Amount) > 100)
            {
                errorList.Add("Total Gross Pay Head Amount can't exceed 100%.");
            }


            //var summationOfGrossPayhead = _empService.GetSumOfGrossPayHeadByEmpId(model.SalaryStructureId);
            //if (Math.Round(summationOfGrossPayhead).CompareTo(Math.Round(model.GrossSalary)) != 0)

            if (model.isConsolidated)
            {
                if (Math.Round(model.OrgGrossSalary).CompareTo(Math.Round(model.GrossSalary)) != 0)
                    errorList.Add("Gross salary must be equal to the summation of all gross pay head (" + model.OrgGrossSalary + ").");
            }

            return errorList;
        }

        private void GetSalaryHeadAmountTypeSetting(EmployeeSalaryStructureModel model)
        {
            foreach (var item in model.SalaryStructureDetail)
            {
                var ssdModel = item;
                ssdModel.AmountType = item.AmountType;
                ssdModel.HeadAmountTypeList = Common.PopulateAmountType().ToList();

                // Select Amount Type
                if (ssdModel.HeadAmountTypeList.Count > 0)
                {
                    foreach (var Amttype in ssdModel.HeadAmountTypeList)
                    {
                        if (Amttype.Value == ssdModel.AmountType)
                            Amttype.Selected = true;
                    }
                }
            }
        }

        private void GettingSalaryStructureDetailList(EmployeeSalaryStructureModel model)
        {
            var salaryStructureId = 0;

            var empSalaryDetails = _pgmEmpSalaryStructureService.GetEmpSalaryStructureDetails(model.EmployeeId, out salaryStructureId);
            var salaryHeads = _pgmEmpSalaryStructureService.PGMUnit.SalaryHeadRepository.Fetch().ToList();
            var salaryStructureDetails = _salaryStructureService.GetSalaryStrutureDetails(model.GradeId, model.StepId, out salaryStructureId);

            #region Salary structure Details-----------------------------

            if (empSalaryDetails.Count == 0) //new salary structure
            {
                foreach (var item in salaryStructureDetails)
                {
                    var ssdModel = item.ToModel();
                    ssdModel.AmountType = item.AmountType;

                    ssdModel.HeadAmountTypeList = Common.PopulateAmountType().ToList();

                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;

                    ssdModel.IsGrossPayHead = salaryHeads.Find(x => x.Id == item.HeadId).IsGrossPayHead;

                    // check consolidated structure, if yes then amount type should be percent and amount must be zero
                    if (model.isConsolidated)
                    {
                        if (ssdModel.IsGrossPayHead == true)
                        {
                            ssdModel.AmountType = "Percent";
                            ssdModel.Amount = Math.Round(Convert.ToDecimal(0), 2);
                        }
                        else
                        {
                            ssdModel.Amount = Math.Round(Convert.ToDecimal(0), 2);
                        }
                    }

                    model.SalaryStructureDetail.Add(ssdModel);
                }
            }
            else //existing salary structure-
            {
                foreach (var item in empSalaryDetails)
                {
                    var ssdModel = item.ToModel();

                    ssdModel.AmountType = item.AmountType;

                    ssdModel.HeadAmountTypeList = Common.PopulateAmountType().ToList();

                    ssdModel.DisplayHeadName = salaryHeads.Find(x => x.Id == item.HeadId).HeadName;

                    ssdModel.IsGrossPayHead = salaryHeads.Find(x => x.Id == item.HeadId).IsGrossPayHead;
                    ssdModel.cssSalaryHeadClass = "";

                    model.SalaryStructureDetail.Add(ssdModel);
                }

                #region Adding new salary heads from Salary Structure Template -------------------------

                var empSalaryHeadIdList = (from h in empSalaryDetails select h.HeadId).ToList();
                var salaryAbsenceHeadList = (from ah in salaryStructureDetails
                                             where !empSalaryHeadIdList.Contains(ah.HeadId)
                                             select ah).ToList();

                if (salaryAbsenceHeadList.Count > 0)
                {
                    foreach (var newSalaryHead in salaryAbsenceHeadList)
                    {
                        var obj = new SalaryStructureDetailsModel();
                        obj.Id = newSalaryHead.Id;
                        obj.HeadId = newSalaryHead.HeadId;
                        obj.HeadType = newSalaryHead.HeadType;
                        obj.EmployeeId = model.EmployeeId;
                        obj.AmountType = newSalaryHead.AmountType;

                        obj.HeadAmountTypeList = Common.PopulateAmountType().ToList();

                        obj.DisplayHeadName = salaryHeads.Find(x => x.Id == newSalaryHead.HeadId).HeadName;

                        obj.IsGrossPayHead = salaryHeads.Find(x => x.Id == newSalaryHead.HeadId).IsGrossPayHead;

                        obj.Amount = 0;
                        obj.IsTaxable = newSalaryHead.IsTaxable;
                        obj.cssSalaryHeadClass = "cssSalaryHeadClass";

                        model.SalaryStructureDetail.Add(obj);
                    }
                }
                #endregion
            }
            #endregion

            model.TotalAddition = model.SalaryStructureDetail.Where(s => s.HeadType == "Addition").Sum(x => x.Amount);
            model.TotalDeduction = model.SalaryStructureDetail.Where(s => s.HeadType == "Deduction").Sum(x => x.Amount);
            model.NetPay = model.TotalAddition - model.TotalDeduction;
            model.SalaryStructureId = salaryStructureId;
        }
        #endregion

        #endregion

        #region Others

        [NoCache]
        public ActionResult GetDesignation()
        {
            var designations = _pgmEmpSalaryStructureService.PGMUnit.DesignationRepository.GetAll().OrderBy(x => x.Name).ToList();
            return PartialView("Select", Common.PopulateDDLList(designations));
        }

        [NoCache]
        public ActionResult GetGrade()
        {
            return PartialView("Select", Common.PopulateJobGradeDDL(_pgmEmpSalaryStructureService.GetLatestJobGrade()));
        }

        [NoCache]
        public ActionResult GetEmploymentType()
        {
            var grades = _pgmEmpSalaryStructureService.PGMUnit.EmploymentTypeRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDDLList(grades));
        }

        [NoCache]
        public ActionResult GetStaffCategory()
        {
            var grades = _pgmEmpSalaryStructureService.PGMUnit.StaffCategoryRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDDLList(grades));
        }

        [NoCache]
        public ActionResult GetEmployeeStatus()
        {
            var empStatus = _pgmEmpSalaryStructureService.PGMUnit.EmploymentStatusRepository.GetAll().OrderBy(x => x.SortOrder).ToList();
            return PartialView("Select", Common.PopulateDDLList(empStatus));
        }

        [NoCache]
        public ActionResult GetYesNoAsList()
        {
            return PartialView("Select", Common.PopulateYesNoDDLList());
        }

        [NoCache]
        public string GetBankBranchByBankId(int id)
        {
            var listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem { Text = "[Select One]", Value = "" });

            var items = (from entity in _pgmEmpSalaryStructureService.PGMUnit.AccBankBranchRepository.Fetch()
                         where entity.bankId == id
                         select entity).OrderBy(o => o.branchName).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    listItems.Add(new SelectListItem
                    {
                        Text = item.branchName,
                        Value = item.id.ToString()
                    });
                }
            }

            return new JavaScriptSerializer().Serialize(listItems);
        }

        [NoCache]
        public JsonResult GetPrevNextEmployeeId(int currentEmployeeId, bool isNext = true)
        {
            var empList = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeList(LoggedUserZoneInfoId);
            var currentEmp = _pgmEmpSalaryStructureService.PGMUnit.FunctionRepository.GetEmployeeById(currentEmployeeId);
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

        #endregion

        [NoCache]
        [HttpPost]
        public void EditSalaryZoneChangeHistory(SalaryZoneChangeHistoryModel model)
        {
            try
            {
                var salaryZoneChangeHistory = _pgmEmpSalaryStructureService.PGMUnit.SalaryZoneChangeHistoryRepository.GetByID(model.Id);
                if (!model.IsInactive)
                {
                    if (salaryZoneChangeHistory != null)
                    {
                        salaryZoneChangeHistory.ChangeDate = model.ChangeDate;
                        _pgmEmpSalaryStructureService.PGMUnit.SalaryZoneChangeHistoryRepository.Update(
                            salaryZoneChangeHistory);
                        _pgmEmpSalaryStructureService.PGMUnit.SalaryZoneChangeHistoryRepository.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Update);
            }
        }
    }
}