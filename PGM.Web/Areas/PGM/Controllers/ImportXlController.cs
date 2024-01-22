using Domain.PGM;
using Utility;
using PGM.Web.Areas.PGM.Models.ImportXl;
using PGM.Web.Areas.PGM.Models.OverTime;
using PGM.Web.Controllers;
using PGM.Web.Utility;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PGM.Web.Areas.PGM.Controllers
{

    /// <summary>
    /// "Need 2007 Office System Driver: Data Connectivity Components"
    /// - installed in server pc to upload excel file (here ImportData action)
    /// --------------------------------------------------------------
    /// download link is - https://www.microsoft.com/en-us/download/confirmation.aspx?id=23734
    /// or https://www.microsoft.com/en-in/download/details.aspx?id=13255
    /// --
    /// details can be found in https://stackoverflow.com/questions/6649363/microsoft-ace-oledb-12-0-provider-is-not-registered-on-the-local-machine
    /// </summary>

    public class ImportXlController : BaseController
    {
        #region Variables and Constructors
        private PGMCommonService _pgmCommonService;

        public ImportXlController(PGMCommonService pgmCommonService)
        {
            _pgmCommonService = pgmCommonService;
        }
        #endregion

        #region Action Results
        [NoCache]
        public ActionResult Index()
        {
            ImportXlViewModel model = new ImportXlViewModel();
            PopulateDropdown(model);
            return View(model);
        }

        [HttpPost]
        [NoCache]
        public ActionResult ImportData(ImportXlViewModel model)
        {
            model.IsError = 1;
            model.errClass = "failed";

            if (ModelState.IsValid)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase fileUpload = null;

                    foreach (string fileTagName in files)
                    {
                        fileUpload = Request.Files[fileTagName];
                    }

                    if (fileUpload != null)
                    {
                        if (fileUpload.ContentType == PgmImportXlUtil.ExcelContentType1
                            || fileUpload.ContentType == PgmImportXlUtil.ExcelContentType2)
                        {

                            String fileName = fileUpload.FileName;
                            String targetPath = PgmImportXlUtil.FolderPathOfTemppraryFile;
                            String pathToExcelFile = targetPath + fileName;
                            if ((System.IO.File.Exists(pathToExcelFile)))
                            {
                                System.IO.File.SetAttributes(pathToExcelFile, FileAttributes.Normal);
                                System.IO.File.Delete(pathToExcelFile);
                            }
                            fileUpload.SaveAs(pathToExcelFile);

                            int fileType = Common.GetInteger(model.FileTypeId);
                            int count = 0;
                            switch ((PGMEnum.ImportXlFileType)fileType)
                            {
                                case PGMEnum.ImportXlFileType.Attendance:
                                    count = ImportAndSaveAttendanceData(pathToExcelFile);
                                    break;
                                case PGMEnum.ImportXlFileType.Refreshment:
                                    count = ImportAndSaveRefreshmentData(pathToExcelFile);
                                    break;
                                case PGMEnum.ImportXlFileType.Overtime:
                                    count = ImportAndSaveOvertimeData(pathToExcelFile);
                                    break;
                            }

                            model.IsError = 0;
                            model.ErrMsg = "Data imported successfully. Total processed: " + count;
                            model.errClass = "success";
                        }
                        else
                        {
                            model.ErrMsg = "Wrong file format is uploaded!";
                        }
                    }
                    else
                    {
                        model.ErrMsg = "No file is selected to upload!";
                    }
                }
                catch (Exception ex)
                {
                    model.ErrMsg = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.Save);
                }
            }
            else
            {
                model.ErrMsg = "Please fillup mendatory fields";
            }

            PopulateDropdown(model);
            return View("Index", model);
        }

        [HttpGet]
        [NoCache]
        public ActionResult DownloadFile(String paramValues)
        {
            ImportXlViewModel model = new ImportXlViewModel();
            model.IsError = 1;
            model.errClass = "failed";

            if (String.IsNullOrEmpty(paramValues))
            {
                model.ErrMsg = "Supplied parameters are empty!";
                PopulateDropdown(model);
                return View("Index", model);
            }

            try
            {
                // Split input parameter into type, year and month.
                char separator = ',';
                int type = Convert.ToInt32(paramValues.Split(separator)[0]);
                model.FileTypeId = type;
                String year = paramValues.Split(separator)[1];
                model.Year = year;
                String monthText = paramValues.Split(separator)[2];
                String monthValue = paramValues.Split(separator)[3];
                model.Month = monthValue;
                decimal dailyAllow = Common.GetDecimal(paramValues.Split(separator)[4]);
                model.DailyAllowance = dailyAllow;
                decimal revenueStamp = Common.GetDecimal(paramValues.Split(separator)[5]);
                model.RevenueStamp = revenueStamp;
                decimal deductionPercentage = Common.GetDecimal(paramValues.Split(separator)[6]);
                model.DeductionPercentage = deductionPercentage;
                String attFromDate = Common.GetDate2(paramValues.Split(separator)[7]).Replace(" 00:00:00 AM", String.Empty);
                model.AttFromDate = Common.GetDate(attFromDate);
                String attToDate = Common.GetDate2(paramValues.Split(separator)[8]).Replace(" 00:00:00 AM", String.Empty);
                model.AttToDate = Common.GetDate(attToDate);


                // Get employee data and insert into excel file. Finally get byte for download.
                IEnumerable<dynamic> dataModel = null;
                switch ((PGMEnum.ImportXlFileType)type)
                {
                    case PGMEnum.ImportXlFileType.Attendance:
                        dataModel = GetEmployeeListForAttendance(year, monthText, attFromDate, attToDate);
                        break;
                    case PGMEnum.ImportXlFileType.Overtime:
                        dataModel = GetEmployeeListForOvertime(year, monthText, monthValue, revenueStamp, deductionPercentage);
                        break;
                    case PGMEnum.ImportXlFileType.Refreshment:
                        dataModel = GetEmployeeListForRefreshment(year, monthText, dailyAllow, revenueStamp);
                        break;
                }

                if (dataModel.Count() == 0)
                {
                    model.ErrMsg = "No eligible employee found. Please check.";
                    PopulateDropdown(model);
                    return View("Index", model);
                }
                else
                {
                    // Get file as byte array and file name to display.
                    String fileName;
                    byte[] fileContent = PgmImportXlUtil.PreProcessingXlData((PGMEnum.ImportXlFileType)type
                                                                            , dataModel
                                                                            , year
                                                                            , monthText
                                                                            , out fileName);

                    // return file for download
                    return File(fileContent, PgmImportXlUtil.ExcelContentType2, fileName);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "OTRateDividingFactorNotFound")
                    model.ErrMsg = "Please setup OT rate dividing factor in configuration table.";
                else
                    model.ErrMsg = "Download failed!";

                PopulateDropdown(model);
                return View("Index", model);
            }
        }
        #endregion

        #region Private Methods
        [NoCache]
        private IEnumerable<ImportCommonEmployeeInfoViewModel> GetCommonEmployeeInfo(string year, string month)
        {
            // Get employee data
            var designation = _pgmCommonService.PGMUnit.DesignationRepository.GetAll();
            var department = _pgmCommonService.PGMUnit.DivisionRepository.GetAll();

            var activeEmps = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeList()
                .Where(e => e.DateofInactive == null
                            && e.EmploymentStatusId == 1
                            && e.SalaryWithdrawFromZoneId == LoggedUserZoneInfoId);

            var emps = (from e in activeEmps
                        join d in designation on e.DesignationId equals d.Id into d1
                        from desig in d1.DefaultIfEmpty()
                        join div in department on e.DivisionId equals div.Id into div1
                        from dept in div1.DefaultIfEmpty()
                        orderby desig.SortingOrder, e.DateofJoining
                        select new ImportCommonEmployeeInfoViewModel()
                        {
                            Id = e.Id,
                            EmpID = e.EmpID,
                            FullName = e.FullName,
                            DesigName = desig != null ? desig.Name : String.Empty,
                            DeptName = dept != null ? dept.Name : String.Empty,
                            AccountNumber = e.BankAccountNo,
                            BasicSalary = _pgmCommonService.PGMUnit.FunctionRepository.GetBasicSalary(e.Id, year, month),
                            IsEligibleForOvertime = e.IsOvertimeEligible,
                            IsEligibleForRefreshment = Common.GetBoolean(e.IsRefreshmentEligible)
                        }).ToList();

            return emps;
        }

        [NoCache]
        private IEnumerable<ImportRefreshmentViewModel> GetEmployeeListForRefreshment(string year, string month, decimal dailyAllowance, decimal revenueStamp)
        {
            IEnumerable<ImportRefreshmentViewModel> model;
            IEnumerable<ImportCommonEmployeeInfoViewModel> emps = GetCommonEmployeeInfo(year, month).Where(e => e.IsEligibleForRefreshment);
            int serialNumber = 1;
            // Refine data for refreshment.

            var refreshments = _pgmCommonService
                .PGMUnit
                .RefreshmentRepository
                .GetAll()
                .Where(o => o.RMonth == month && o.RYear == year).ToList();

            var refreshments1 = from r in refreshments
                                select new { r.EmployeeId, r.PerDayAmount, r.TotalDays, r.RevenueStamp, r.NetPayable };

            model = (from e in emps
                     join r in refreshments1 on e.Id equals r.EmployeeId into refreshmentJoin
                     from r1 in refreshmentJoin.DefaultIfEmpty()
                     select new ImportRefreshmentViewModel()
                     {
                         Sl_No = serialNumber++,
                         Employee_Id = e.EmpID,
                         Employee_Name = e.FullName,
                         Designation = e.DesigName,
                         Department = e.DeptName,
                         Account_Number = e.AccountNumber,
                         R_Month = month,
                         R_Year = year,
                         Revenue_Stamp = r1 != null ? Common.GetDecimal(r1.RevenueStamp) : revenueStamp,
                         Per_Day_Amount = r1 != null ? Common.GetDecimal(r1.PerDayAmount) : dailyAllowance,
                         Total_Days = r1 != null ? Common.GetDecimal(r1.TotalDays) : 0.0M,
                         Net_Payable = r1 != null ? Common.GetDecimal(r1.NetPayable) : 0.0M
                     }).ToList();

            return model;
        }

        [NoCache]
        private IEnumerable<ImportOvertimeViewModel> GetEmployeeListForOvertime(string year, string monthText, string monthValue, decimal revenueStamp, decimal deductionPercentage)
        {
            IEnumerable<ImportOvertimeViewModel> model;
            var emps = GetCommonEmployeeInfo(year, monthText).Where(e => e.IsEligibleForOvertime);
            String otRateBasedOn = String.Empty;
            decimal otRateDividingFactor = 0.0M;
            int serialNumber = 1;
            // Refine data for overtime.

            if (_pgmCommonService.PGMUnit.PgmConfiguration.GetAll().FirstOrDefault() != null)
            {
                //TODO: Need to work with based on - basic/gross
                otRateBasedOn = _pgmCommonService.PGMUnit.PgmConfiguration.GetAll().FirstOrDefault().OTRateBasedOn;
                otRateDividingFactor = Common.GetDecimal(_pgmCommonService.PGMUnit.PgmConfiguration.GetAll().FirstOrDefault().OTRateDividingFactor);

                if (Common.GetDecimal(otRateDividingFactor) == 0)
                {
                    throw new Exception("OTRateDividingFactorNotFound");
                }
            }

            var overtimes = _pgmCommonService.PGMUnit.OvertimeRepository.GetAll()
                .Where(o => o.OTMonth == Common.GetInteger(monthValue) && o.OTYear == Common.GetInteger(year)).ToList();

            var overtimes1 = from o in overtimes
                             select new
                             {
                                 o.EmployeeId,
                                 o.WorkedHours,
                                 o.ApprovedHours,
                                 o.BasicSalary,
                                 o.OvertimeRate,
                                 o.RevenueStamp,
                                 o.DeductionPercentage,
                                 o.NetPayable
                             };

            model = (from e in emps
                     join o in overtimes1 on e.Id equals o.EmployeeId into overtimeJoin
                     from o1 in overtimeJoin.DefaultIfEmpty()
                     select new ImportOvertimeViewModel()
                     {
                         Sl_No = serialNumber++,
                         Employee_Id = e.EmpID,
                         Employee_Name = e.FullName,
                         Designation = e.DesigName,
                         Department = e.DeptName,
                         Account_Number = e.AccountNumber,
                         OT_Month = monthText,
                         OT_Year = year,
                         Basic_Salary = o1 != null ? Common.GetDecimal(o1.BasicSalary) : e.BasicSalary,
                         OT_Rate = o1 != null ? Math.Round(Common.GetDecimal(o1.OvertimeRate), 2) : Math.Round(e.BasicSalary / otRateDividingFactor, 2),
                         Revenue_Stamp = o1 != null ? Common.GetDecimal(o1.RevenueStamp) : revenueStamp,
                         Actual_Hour = o1 != null ? Common.GetDecimal(o1.WorkedHours) : 0,
                         Approved_Hour = o1 != null ? Common.GetDecimal(o1.ApprovedHours) : 0,
                         Deduction_Percentage = o1 != null ? Common.GetDecimal(o1.DeductionPercentage) : deductionPercentage,
                         Net_Payable = o1 != null ? Common.GetDecimal(o1.NetPayable) : 0.0M
                     }).ToList();

            return model;
        }

        [NoCache]
        private IEnumerable<ImportAttendanceViewModel> GetEmployeeListForAttendance(string year, string month, string fromDate, string toDate)
        {
            IEnumerable<ImportAttendanceViewModel> model;
            IEnumerable<ImportCommonEmployeeInfoViewModel> emps = GetCommonEmployeeInfo(year, month);
            int serialNumber = 1;
            // Refine data for attendance.
            var calenderDays = DateTime.DaysInMonth(Convert.ToInt32(year), Common.GetMonthNo(month));

            var att = _pgmCommonService
                .PGMUnit
                .AttendanceRepository
                .GetAll()
                .Where(at => at.AttMonth == month && at.AttYear == year).ToList();

            var att1 = from a in att
                       select new
                       {
                           a.EmployeeId,
                           a.AttFromDate,
                           a.AttToDate,
                           a.TotalPresent,
                           a.TotalCasualLeave,
                           a.TotalEarnedLeave,
                           a.TotalOthersLeave,
                           a.TotalAttendance,
                           a.Remark
                       };

            model = (from e in emps
                     join a in att1 on e.Id equals a.EmployeeId into attJoin
                     from a1 in attJoin.DefaultIfEmpty()
                     select new ImportAttendanceViewModel()
                     {
                         Sl_No = serialNumber++,
                         Employee_Id = e.EmpID,
                         Employee_Name = e.FullName,
                         Designation = e.DesigName,
                         Department = e.DeptName,
                         Att_Month = month,
                         Att_Year = year,
                         Calender_Days = calenderDays,
                         Att_From_Date = a1 != null ? Common.GetDate2(a1.AttFromDate) : fromDate,
                         Att_To_Date = a1 != null ? Common.GetDate2(a1.AttFromDate) : toDate,
                         Total_Present = a1 != null ? Common.GetDecimal(a1.TotalPresent) : 0,
                         Total_Casual_Leave = a1 != null ? Common.GetDecimal(a1.TotalCasualLeave) : 0,
                         Total_Earned_Leave = a1 != null ? Common.GetDecimal(a1.TotalEarnedLeave) : 0,
                         Total_Others_Leave = a1 != null ? Common.GetDecimal(a1.TotalOthersLeave) : 0,
                         Total_Attendance = a1 != null ? Common.GetDecimal(a1.TotalAttendance) : 0,
                         Remark = a1 != null ? Common.GetString(a1.Remark) : String.Empty
                     }).ToList();

            return model;
        }

        [NoCache]
        private Int32 ImportAndSaveAttendanceData(String filePath)
        {
            var xlFile = new ExcelQueryFactory(filePath);

            var xlSheetData = from att in xlFile.Worksheet<ImportAttendanceViewModel>("Attendance") select att;
            int count = 0;

            foreach (var data in xlSheetData)
            {
                var empId = data.Employee_Id;
                if (!String.IsNullOrEmpty(empId))
                {
                    var employeeId = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeByEmpId(empId).Id;

                    var isdataexists = _pgmCommonService.PGMUnit.AttendanceRepository.GetAll()
                        .Any(a => a.EmployeeId == employeeId && a.AttMonth == data.Att_Month &&
                                  a.AttYear == data.Att_Year);

                    if (!isdataexists)
                    {
                        var entity = data.ToEntity();
                        entity.EmployeeId = employeeId;
                        entity.AttMonth = data.Att_Month;
                        entity.AttYear = data.Att_Year;
                        entity.AttFromDate = Common.GetDate(data.Att_From_Date);
                        entity.AttToDate = Common.GetDate(data.Att_To_Date);
                        entity.CalenderDays = data.Calender_Days;
                        entity.TotalPresent = data.Total_Present;
                        entity.TotalCasualLeave = data.Total_Casual_Leave;
                        entity.TotalEarnedLeave = data.Total_Earned_Leave;
                        entity.TotalOthersLeave = data.Total_Others_Leave;
                        entity.TotalAttendance = data.Total_Attendance;
                        entity.Remark = data.Remark;
                        entity.ZoneIdDuringAttendance = LoggedUserZoneInfoId;

                        if (entity.TotalAttendance > 0)
                        {
                            ++count;
                            _pgmCommonService.PGMUnit.AttendanceRepository.Add(entity);
                        }
                    }
                    else
                    {
                        var entity = _pgmCommonService.PGMUnit.AttendanceRepository.GetAll()
                            .FirstOrDefault(a => a.EmployeeId == employeeId && a.AttMonth == data.Att_Month &&
                                                 a.AttYear == data.Att_Year);
                        if (entity != null)
                        {
                            entity.TotalPresent = data.Total_Present;
                            entity.TotalCasualLeave = data.Total_Casual_Leave;
                            entity.TotalEarnedLeave = data.Total_Earned_Leave;
                            entity.TotalOthersLeave = data.Total_Others_Leave;
                            entity.TotalAttendance = data.Total_Attendance;
                            entity.Remark = data.Remark;

                            if ((entity.TotalPresent + entity.TotalCasualLeave + entity.TotalEarnedLeave +
                                 entity.TotalOthersLeave) == entity.TotalAttendance)
                            {
                                if (entity.TotalAttendance <= data.Calender_Days)
                                {
                                    ++count;
                                    _pgmCommonService.PGMUnit.AttendanceRepository.Update(entity);
                                }
                            }
                        }
                    }
                }
            }

            _pgmCommonService.PGMUnit.AttendanceRepository.SaveChanges();

            return count;
        }

        [NoCache]
        private Int32 ImportAndSaveRefreshmentData(String filePath)
        {
            var xlFile = new ExcelQueryFactory(filePath);

            var xlSheetData = from att in xlFile.Worksheet<ImportRefreshmentViewModel>("Refreshment") select att;
            int count = 0;

            foreach (var data in xlSheetData)
            {

                var empId = data.Employee_Id;
                if (!String.IsNullOrEmpty(empId))
                {
                    var employeeId = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeByEmpId(empId).Id;

                    var isdataexists = _pgmCommonService.PGMUnit.RefreshmentRepository.GetAll()
                        .Any(a => a.EmployeeId == employeeId && a.RMonth == data.R_Month &&
                                  a.RYear == data.R_Year);

                    if (!isdataexists)
                    {
                        var entity = data.ToEntity();
                        entity.EmployeeId = employeeId;
                        entity.RMonth = data.R_Month;
                        entity.RYear = data.R_Year;
                        entity.PerDayAmount = data.Per_Day_Amount;
                        entity.TotalDays = data.Total_Days;
                        entity.RevenueStamp = data.Revenue_Stamp;
                        entity.NetPayable = data.Net_Payable;
                        entity.ZoneIdDuringRefreshment = LoggedUserZoneInfoId;

                        _pgmCommonService.PGMUnit.RefreshmentRepository.Add(entity);
                    }
                    else
                    {
                        var entity = _pgmCommonService.PGMUnit.RefreshmentRepository.GetAll()
                            .FirstOrDefault(a => a.EmployeeId == employeeId && a.RMonth == data.R_Month &&
                                                 a.RYear == data.R_Year);
                        if (entity != null)
                        {
                            entity.PerDayAmount = data.Per_Day_Amount;
                            entity.TotalDays = data.Total_Days;
                            entity.RevenueStamp = data.Revenue_Stamp;
                            entity.NetPayable = data.Net_Payable;

                            _pgmCommonService.PGMUnit.RefreshmentRepository.Update(entity);
                        }
                    }

                    ++count;
                }
            }

            _pgmCommonService.PGMUnit.RefreshmentRepository.SaveChanges();

            return count;
        }

        [NoCache]
        private Int32 ImportAndSaveOvertimeData(String filePath)
        {
            var xlFile = new ExcelQueryFactory(filePath);

            var xlSheetData = from att in xlFile.Worksheet<ImportOvertimeViewModel>("Overtime") select att;
            int count = 0;

            foreach (var data in xlSheetData)
            {
                var empId = data.Employee_Id;
                if (!String.IsNullOrEmpty(empId))
                {
                    var emp = _pgmCommonService.PGMUnit.FunctionRepository.GetEmployeeByEmpId(empId);
                    var employeeId = emp.Id;
                    var designationId = emp.DesignationId;
                    var isdataexists = _pgmCommonService.PGMUnit.OvertimeRepository.GetAll()
                        .Any(a => a.EmployeeId == employeeId && a.OTMonth == Common.GetMonthNo(data.OT_Month) &&
                                  a.OTYear == Common.GetInteger(data.OT_Year));

                    if (!isdataexists)
                    {
                        OvertimeModel otModel = new OvertimeModel();
                        otModel.EmployeeId = employeeId;
                        otModel.DesignationId = designationId;
                        otModel.AccountNo = data.Account_Number;
                        otModel.IsImpactToSalary = false;
                        otModel.OTYear = Common.GetInteger(data.OT_Year);
                        otModel.OTMonth = Common.GetMonthNo(data.OT_Month);
                        otModel.BasicSalary = data.Basic_Salary;
                        otModel.WorkedHours = data.Actual_Hour;
                        otModel.ApprovedHours = data.Approved_Hour;
                        otModel.OvertimeRate = data.OT_Rate;
                        otModel.RevenueStamp = data.Revenue_Stamp;
                        otModel.DeductionPercentage = data.Deduction_Percentage;
                        otModel.NetPayable = data.Net_Payable;
                        otModel.ZoneIdDuringOvertime = LoggedUserZoneInfoId;

                        var entity = otModel.ToEntity();

                        _pgmCommonService.PGMUnit.OvertimeRepository.Add(entity);
                    }
                    else
                    {
                        var entity = _pgmCommonService.PGMUnit.OvertimeRepository.GetAll()
                            .FirstOrDefault(a => a.EmployeeId == employeeId
                                            && a.OTMonth == Common.GetMonthNo(data.OT_Month)
                                            && a.OTYear == Common.GetInteger(data.OT_Year));

                        if (entity != null)
                        {
                            entity.WorkedHours = data.Actual_Hour;
                            entity.ApprovedHours = data.Approved_Hour;
                            entity.RevenueStamp = data.Revenue_Stamp;
                            entity.DeductionPercentage = data.Deduction_Percentage;
                            entity.NetPayable = data.Net_Payable;

                            _pgmCommonService.PGMUnit.OvertimeRepository.Update_64Bit(entity);
                        }
                    }

                    ++count;
                }
            }

            _pgmCommonService.PGMUnit.OvertimeRepository.SaveChanges();
            return count;
        }

        [NoCache]
        private void PopulateDropdown(ImportXlViewModel model)
        {
            model.FileTypeList = Common.PopulateImportXlFileTypeDDL();
            model.YearList = Common.PopulateYearList();
            model.MonthList = Common.PopulateMonthList3();
        }
        #endregion
    }
}