using Utility;
using PGM.Web.Areas.PGM.Models.ImportXl;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace PGM.Web.Utility
{
    public class PgmImportXlUtil
    {
        public static string ExcelContentType1
        {
            get { return "application/vnd.ms-excel"; }
        }

        public static string ExcelContentType2
        {
            get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        public static string FolderPathOfTemplateFIle
        {
            get { return HttpContext.Current.Server.MapPath("~/Content/PGM_ImportXlTemplate/"); }
        }

        public static string FolderPathOfTemppraryFile
        {
            get { return HttpContext.Current.Server.MapPath("~/Content/TempFiles/"); }
        }

        public static String GetFullPathOfXlTemplate(PGMEnum.ImportXlFileType fileType)
        {
            String templateName = String.Empty;

            switch (fileType)
            {
                case PGMEnum.ImportXlFileType.Attendance:
                    templateName = "ImportAttendanceTemplate.xlsx";
                    break;
                case PGMEnum.ImportXlFileType.Overtime:
                    templateName = "ImportOvertimeTemplate.xlsx";
                    break;
                case PGMEnum.ImportXlFileType.Refreshment:
                    templateName = "ImportRefreshmentTemplate.xlsx";
                    break;
            }

            var filePath = FolderPathOfTemplateFIle + templateName;

            return filePath;
        }

        public static byte[] PreProcessingXlData(PGMEnum.ImportXlFileType fileType, IEnumerable<dynamic> activeEmps, String year, String month, out String fileName)
        {
            byte[] outputByte = null;
            String outputFileName = String.Empty;

            var xlFilePath = CopyFileFromTemplate(fileType, year, month, out outputFileName);

            switch (fileType)
            {
                case PGMEnum.ImportXlFileType.Attendance:
                    IEnumerable<ImportAttendanceViewModel> attModel = activeEmps as IEnumerable<ImportAttendanceViewModel>;
                    outputByte = FillAttendanceData(attModel, xlFilePath);
                    break;
                case PGMEnum.ImportXlFileType.Overtime:
                    IEnumerable<ImportOvertimeViewModel> overtimeModel = activeEmps as IEnumerable<ImportOvertimeViewModel>;
                    outputByte = FillOvertimeData(overtimeModel, xlFilePath);
                    break;
                case PGMEnum.ImportXlFileType.Refreshment:
                    IEnumerable<ImportRefreshmentViewModel> refreshmentModel = activeEmps as IEnumerable<ImportRefreshmentViewModel>;
                    outputByte = FillRefreshmentData(refreshmentModel, xlFilePath);
                    break;
            }

            if (!String.IsNullOrEmpty(outputFileName))
                fileName = outputFileName;
            else
                fileName = "noname00.xlsx";

            return outputByte;
        }

        /// <summary>
        /// Copy template file to temporary folder and return full file path. Also return file name as out
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="outputFileName"></param>
        /// <returns></returns>
        private static String CopyFileFromTemplate(PGMEnum.ImportXlFileType fileType, String year, String month, out String outputFileName)
        {
            String newFileName = String.Empty;
            outputFileName = String.Empty;

            // Get template path.
            var templatePath = GetFullPathOfXlTemplate(fileType);

            // if template file not found then return null.
            if (!File.Exists(templatePath)) return String.Empty;

            String prefix = "Import"; // CAUTION: this prefix is also using in javascript. If anything change here then change in script also.

            String suffix = year + "_" + month.Substring(0, 3) + DateTime.Now.ToString("_yyyyMMddHHmmssfff") + ".xlsx";


            // Copy an instance of template file in temporary location for processing
            switch (fileType)
            {
                case PGMEnum.ImportXlFileType.Attendance:
                    newFileName = prefix + "Attendance_" + suffix;
                    break;
                case PGMEnum.ImportXlFileType.Overtime:
                    newFileName = prefix + "Overtime_" + suffix;
                    break;
                case PGMEnum.ImportXlFileType.Refreshment:
                    newFileName = prefix + "Refreshment_" + suffix;
                    break;
            }

            outputFileName = newFileName;
            var newFilePath = FolderPathOfTemppraryFile + newFileName;

            // Copy
            File.Copy(templatePath, newFilePath);

            return newFilePath;
        }

        private static byte[] FillAttendanceData(IEnumerable<ImportAttendanceViewModel> model, String xlFilePath)
        {
            byte[] outputByte;

            // Create wrapper of new file.
            FileInfo newFile = new FileInfo(xlFilePath);

            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets["Attendance"];
                int currentRow = 2;
                foreach (var emp in model)
                {
                    ws.Cells[currentRow, 1].Value = emp.Sl_No;
                    ws.Cells[currentRow, 2].Value = emp.Employee_Id;
                    ws.Cells[currentRow, 3].Value = emp.Employee_Name;
                    ws.Cells[currentRow, 4].Value = emp.Designation;
                    ws.Cells[currentRow, 5].Value = emp.Department;
                    ws.Cells[currentRow, 6].Value = emp.Att_Month;
                    ws.Cells[currentRow, 7].Value = emp.Att_Year;
                    ws.Cells[currentRow, 8].Value = emp.Calender_Days;
                    ws.Cells[currentRow, 9].Value = emp.Att_From_Date;
                    ws.Cells[currentRow, 10].Value = emp.Att_To_Date;
                    ws.Cells[currentRow, 11].Value = emp.Total_Present;
                    ws.Cells[currentRow, 12].Value = emp.Total_Casual_Leave;
                    ws.Cells[currentRow, 13].Value = emp.Total_Earned_Leave;
                    ws.Cells[currentRow, 14].Value = emp.Total_Others_Leave;

                    ws.Cells[currentRow, 15].Value = emp.Total_Attendance;
                    ws.Cells[currentRow, 15].Formula = "K" + currentRow + "+L" + currentRow + "+M" + currentRow + "+N" + currentRow + "";

                    ws.Cells[currentRow, 16].Value = emp.Remark;
                    ++currentRow;
                }

                outputByte = pck.GetAsByteArray();
            }

            return outputByte;
        }

        private static byte[] FillOvertimeData(IEnumerable<ImportOvertimeViewModel> model, String xlFilePath)
        {
            byte[] outputByte;

            // Create wrapper of new file.
            FileInfo newFile = new FileInfo(xlFilePath);

            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets["Overtime"];
                int currentRow = 2;
                foreach (var emp in model)
                {
                    ws.Cells[currentRow, 1].Value = emp.Sl_No;
                    ws.Cells[currentRow, 2].Value = emp.Employee_Id;
                    ws.Cells[currentRow, 3].Value = emp.Employee_Name;
                    ws.Cells[currentRow, 4].Value = emp.Designation;
                    ws.Cells[currentRow, 5].Value = emp.Department;
                    ws.Cells[currentRow, 6].Value = emp.Account_Number;
                    ws.Cells[currentRow, 7].Value = emp.OT_Month;
                    ws.Cells[currentRow, 8].Value = emp.OT_Year;
                    ws.Cells[currentRow, 9].Value = emp.Basic_Salary;
                    ws.Cells[currentRow, 10].Value = emp.OT_Rate;
                    ws.Cells[currentRow, 11].Value = emp.Revenue_Stamp; // K
                    ws.Cells[currentRow, 12].Value = emp.Actual_Hour; // L

                    ws.Cells[currentRow, 13].Value = emp.Approved_Hour; // M
                    ws.Cells[currentRow, 13].Formula = "ROUND((L" + currentRow + "*N" + currentRow + "/100), 0)";

                    ws.Cells[currentRow, 14].Value = emp.Deduction_Percentage; // N

                    ws.Cells[currentRow, 15].Value = emp.Net_Payable; // O
                    // Netpayable calculation formula - IF(ROUND((M3*J3), 0)>0, ROUND((M3*J3), 0)-K3, 0)
                    ws.Cells[currentRow, 15].Formula = "IF(ROUND((M" + currentRow + "*J" + currentRow + "), 0)>0, ROUND((M" + currentRow + "*J" + currentRow + "), 0)-K" + currentRow + ", 0)";

                    ++currentRow;
                }

                var netPayableColIndex = currentRow - 1;
                ws.Cells[currentRow, 15].Formula = "SUM(O2:O" + netPayableColIndex + ")";

                outputByte = pck.GetAsByteArray();
            }

            return outputByte;
        }

        private static byte[] FillRefreshmentData(IEnumerable<ImportRefreshmentViewModel> model, String xlFilePath)
        {
            byte[] outputByte;

            // Create wrapper of new file.
            FileInfo newFile = new FileInfo(xlFilePath);

            using (ExcelPackage pck = new ExcelPackage(newFile))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets["Refreshment"];
                int currentRow = 2;
                foreach (var emp in model)
                {
                    ws.Cells[currentRow, 1].Value = emp.Sl_No;
                    ws.Cells[currentRow, 2].Value = emp.Employee_Id;
                    ws.Cells[currentRow, 3].Value = emp.Employee_Name;
                    ws.Cells[currentRow, 4].Value = emp.Designation;
                    ws.Cells[currentRow, 5].Value = emp.Department;
                    ws.Cells[currentRow, 6].Value = emp.Account_Number;
                    ws.Cells[currentRow, 7].Value = emp.R_Month;
                    ws.Cells[currentRow, 8].Value = emp.R_Year;
                    ws.Cells[currentRow, 9].Value = emp.Revenue_Stamp;
                    ws.Cells[currentRow, 10].Value = emp.Per_Day_Amount;
                    ws.Cells[currentRow, 11].Value = emp.Total_Days;
                    ws.Cells[currentRow, 12].Value = emp.Net_Payable;

                    ws.Cells[currentRow, 12].Formula = "ROUND((J" + currentRow + "*K" + currentRow + ")-I" + currentRow + ", 0)";

                    ++currentRow;
                }

                var netPayableColIndex = currentRow - 1;
                ws.Cells[currentRow, 12].Formula = "SUM(L2:L" + netPayableColIndex + ")";

                outputByte = pck.GetAsByteArray();
            }

            return outputByte;
        }

    }
}