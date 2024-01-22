using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Utility;

/*
Revision History (RH):
		SL	    : 01
		Author	: Md. Amanullah
		Date	: 2016-Jan-25
        SCR     : MFS_IWM_PGM_SCR.doc (SCR#54)
		Desc.	: Report header will be change for withheld Withdraw with payment
		---------
*/

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class SalaryPaySlipDetail : PGMReportBase
    {
        #region Properties

        public string FromEmailAddress;
        public string ToEmailAddress;
        public string CCEmailAddress;
        public string BCCEmailAddress;
        public string Subject;
        public string Salutation;
        public string BodyHeader;
        public string BodyFooter;
        public string FromEmailPersonName;
        public string EmailBody;
        public string tableHeaderBackgroundColor;
        public string tableBorderColor;
        public string NoRecordFound;
        public bool IsNoRecord = true;

        #endregion

        bool status = false;
        [NoCache]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedUserZoneInfoId == 0) return;

            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #region Button Event
        [NoCache]
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                var jobGradeId = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
                //var divisionId = Convert.ToInt32(ddlSelectDivision.SelectedValue);
                var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
                var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);
                var categoryId = Convert.ToInt32(ddlEmpCategory.SelectedValue);

                int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());

                var selectedZoneLists = new List<int>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneLists.Add(Convert.ToInt32(item.Value));
                    }
                }

                lblMsg.Text = String.Empty;
                GenerateReport(selectedYear
                    , selectedMonth
                    , designationId
                    , empTypeId
                    , employeeId
                    , jobGradeId
                    , categoryId
                    , ConvertListToString(selectedZoneLists.ToArray()));

            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
        #endregion

        #region Send mail
        [NoCache]
        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = default(string);

                #region Get Data

                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                var jobGradeId = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
                //var divisionId = Convert.ToInt32(ddlSelectDivision.SelectedValue);
                var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
                var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);
                var categoryId = Convert.ToInt32(ddlEmpCategory.SelectedValue);

                int employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());

                //var data = (from s in base._pgmContext.vwPGMSalaryPaySlipRpts.Where(y => y.SalaryYear == selectedYear)
                //            select s).Where((sm => sm.SalaryMonth == selectedMonth)).OrderBy(o => o.SortPriority).DistinctBy(x => x.EmpID).ToList();

                //if (!string.IsNullOrEmpty(selectedYear)) data = data.Where(y => y.SalaryYear == selectedYear).ToList();
                //if (!string.IsNullOrEmpty(selectedMonth)) data = data.Where(m => m.SalaryMonth == selectedMonth).ToList();
                ////if (divisionId > 0) data = data.Where(q => q.DivisionId == divisionId).ToList();
                //if (designationId > 0) data = data.Where(q => q.DesignationId == designationId).ToList();
                //if (empTypeId > 0) data = data.Where(q => q.EmploymentId == empTypeId).ToList();
                //if (jobGradeId > 0) data = data.Where(q => q.GradeId == jobGradeId).ToList();

                //if (employeeId > 0) data = data.Where(q => q.EmployeeId == employeeId).ToList();

                var selectedZoneLists = new List<int>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneLists.Add(Convert.ToInt32(item.Value));
                    }
                }
                var data = _pgmExecuteFunctions.GetMonthlySalaryPayslip(selectedYear, selectedMonth, ConvertListToString(selectedZoneLists.ToArray()), employeeId);

                if (designationId > 0) data = data.Where(q => q.DesignationId == designationId).ToList();
                if (empTypeId > 0) data = data.Where(q => q.EmploymentId == empTypeId).ToList();
                if (jobGradeId > 0) data = data.Where(q => q.GradeId == jobGradeId).ToList();
                if (categoryId > 0) data = data.Where(q => q.StaffCategoryId == categoryId).ToList();

                #endregion

                #region Get Sender mail Info
                var loginUserId = MyAppSession.User.EmpId;
                FromEmailPersonName = _pgmExecuteFunctions.GetEmployeeByEmpId(loginUserId)?.FullName;
                FromEmailAddress = _pgmExecuteFunctions.GetEmployeeByEmpId(loginUserId)?.EmialAddress;

                if (String.IsNullOrEmpty(FromEmailAddress))
                {
                    lblMsg.Text = "Sender mail address is empty!";
                    return;
                }
                else if (FromEmailAddress != string.Empty && employeeId == 0)
                {
                    lblMsg.Text = "Email send process is in progress, it taking time to send mail to all receiver.";
                }
                #endregion

                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        ToEmailAddress = _pgmExecuteFunctions.GetEmployeeById(Convert.ToInt32(item.EmployeeId))?.EmialAddress;
                        CCEmailAddress = "";
                        Subject += " For the month of " + ddlSelectMonth.SelectedItem.Text + "/" + ddlSelectYear.SelectedItem.Text;
                        Salutation = "Dear Mr./Ms.";

                        #region Check Velidation

                        if (ToEmailAddress == string.Empty && employeeId > 0)
                        {
                            lblMsg.Text = "Receiver mail address is empty.";
                        }
                        else if (FromEmailAddress != string.Empty && ToEmailAddress != string.Empty && employeeId > 0)
                        {
                            lblMsg.Text = "Email has been sent successfully!";
                        }

                        #endregion

                        if (ToEmailAddress != string.Empty && FromEmailAddress != string.Empty)
                        {
                            string body = CreateBody(item);

                            EmailProcessor email = new EmailProcessor();
                            email.SendEmail(FromEmailAddress, FromEmailPersonName, ToEmailAddress, CCEmailAddress, Subject, body);
                        }
                    }
                }
                else
                {
                    status = true;
                }

                if (status == true)
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
                status = false;
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
                lblMsg.ForeColor = System.Drawing.Color.Red;
            }
        }
        [NoCache]
        private string CreateBody(DAL.PGM.sp_PGM_Payslip_Result item)
        {
            var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
            var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
            var jobGradeId = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
            //var divisionId = Convert.ToInt32(ddlSelectDivision.SelectedValue);
            var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
            var empTypeId = Convert.ToInt32(ddlSelectEmployeeType.SelectedValue);

            var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
            var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

            string content = string.Empty;

            try
            {
                DateTime rr = Convert.ToDateTime(System.DateTime.Now.Year + "/" + System.DateTime.Now.Month + "/01");

                DateTime startDate = rr.AddMonths(1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                if (item != null)
                {
                    var EmployeeId = item.EmployeeId;
                    var EmpId = item.EmpID;
                    var initial = item.EmployeeInitial;
                    var name = item.FullName;
                    var Designation = item.Designation;
                    var division = item.Division;
                    var grade = item.GradeName;
                    var grossSalary = default(decimal);
                    var totalDeduction = default(decimal);

                    content = "<div style='font: 10pt Tahoma;'>";
                    content = "<p>Salary Pay Slip</p>";
                    content += "<p>For the month of " + ddlSelectMonth.SelectedItem.Text + "/" + ddlSelectYear.SelectedItem.Text + "</p>";
                    content += "<table style='border: 1px solid black; width:80%;'><tr><td>";
                    content += "<table>";
                    content += "<tr><td><b> Employee ID</b></td> <td><b> : </b></td> <td>" + EmpId + "</td>" + "<td><b> Employee Initial</b></td> <td><b> : </b></td> <td>" + initial + "</td></tr>";
                    content += "<tr><td><b> Employee Name</b></td> <td><b> : </b></td> <td>" + name + "</td>" + "<td><b> Division</b></td> <td><b> : </b></td> <td>" + division + "</td></tr>";
                    content += "<tr><td><b> Designation</b></td> <td><b> : </b></td> <td>" + Designation + "</td>" + "<td><b> Grade</b></td> <td><b> : </b></td> <td>" + grade + "</td></tr>";
                    content += "</table>";
                    content += "<td></tr></table>";

                    // Salary PaySlip Detail Addition
                    var salaryPaySlipDetailAddition = (from ad in base._pgmContext.GetSalaryPaySlipDetailAddition(EmployeeId, selectedYear, selectedMonth, numErrorCode, strErrorMsg) select ad).OrderBy(x => x.SortPriority).ToList();

                    content += "<table style='border: 1px solid black; width:80%;'><tr><td>";
                    content += "<table style='width:60%;'>";
                    content += "<tr><td width='50%'><b>" + "Salary :" + "</b></td><td>" + "" + "</td></tr>";

                    foreach (var subItem1 in salaryPaySlipDetailAddition)
                    {
                        content += "<tr><td width='50%'>" + subItem1.HeadName + "</td><td>" + subItem1.HeadAmount + "</td></tr>";
                        grossSalary += subItem1.HeadAmount;
                    }

                    content += "<tr><td width='50%'><b>" + "Gross Salary (A)" + "</b></td><td><b>" + grossSalary + "</b></td></tr>";
                    content += "</table>";
                    content += "</td></tr></table>";

                    //Salary PaySlip Detail Deduction
                    //var salaryPaySlipDetailDeduction = (from ad in base.pgmContext.GetSalaryPaySlipDetailDeduction(EmployeeId, selectedYear, selectedMonth, numErrorCode, strErrorMsg).OrderBy(o=>o.HeadType) select ad).ToList();

                    //var salaryHeadGroupNameStart = ""; var salaryHeadGroupNameEnd = "";

                    var salaryPaySlipDetailDeduction = (from t in base._pgmContext.GetSalaryPaySlipDetailDeduction(EmployeeId, selectedYear, selectedMonth, numErrorCode, strErrorMsg).OrderBy(o => o.HeadType)
                                                        group t by new { t.SalaryHeadGroupName, t.HeadName }
                                                            into grp
                                                            select new
                                                            {
                                                                grp.Key.SalaryHeadGroupName,
                                                                grp.Key.HeadName,
                                                                HeadAmount = grp.Sum(t => t.HeadAmount)
                                                            }).OrderBy(o => o.SalaryHeadGroupName).ToList();

                    content += "<table style='border: 1px solid black; width:80%;'><tr><td>";
                    content += "<table style='width:60%;'>";
                    content += "<tr><td width='50%'><b>" + "Deduction :" + "</b></td><td>" + "" + "</td></tr>";

                    foreach (var subItem2 in salaryPaySlipDetailDeduction)
                    {
                        //salaryHeadGroupNameStart = subItem2.SalaryHeadGroupName;

                        //if (salaryHeadGroupNameStart != salaryHeadGroupNameEnd)
                        //{
                        //    content += "<tr><td width='50%'><b>" + subItem2.SalaryHeadGroupName + "</b></td><td>" + "" + "</td></tr>";
                        //}

                        content += "<tr><td width='50%'>" + subItem2.HeadName + "</td><td>" + subItem2.HeadAmount + "</td></tr>";
                        totalDeduction += subItem2.HeadAmount;

                        //salaryHeadGroupNameEnd = subItem2.SalaryHeadGroupName;
                    }

                    content += "<tr><td width='50%'><b>" + "Total Deduction (B)" + "</b></td><td><b>" + totalDeduction + "</b></td></tr>";
                    content += "<tr><td width='50%'><b>" + "Net Payable (A-B)" + "</b></td><td><b>" + (grossSalary - totalDeduction) + "</b></td></tr>";
                    content += "</table>";
                    content += "</td></tr></table>";
                    content += "</div>";
                }
                else
                {
                    IsNoRecord = false;
                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }

            return content;
        }

        #endregion

        [NoCache]
        private void GenerateReport(string selectedYear, string selectedMonth, int designationId, int empTypeId, int employeeId, int jobGradeId, int categoryId, String zoneList)
        {
            try
            {
                rvSalaryPaySlipRpt.Reset();
                var data = _pgmExecuteFunctions.GetMonthlySalaryPayslip(selectedYear, selectedMonth, zoneList, employeeId);

                //if (divisionId > 0) data = data.Where(q => q.DivisionId == divisionId).ToList();
                if (designationId > 0) data = data.Where(q => q.DesignationId == designationId).ToList();
                if (empTypeId > 0) data = data.Where(q => q.EmploymentId == empTypeId).ToList();
                if (jobGradeId > 0) data = data.Where(q => q.GradeId == jobGradeId).ToList();
                if (categoryId > 0) data = data.Where(q => q.StaffCategoryId == categoryId).ToList();

                var reportHeader = base.GetZoneInfoForReportHeader();


                if (!data.Any())
                {
                    lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
                }
                else
                {
                    rvSalaryPaySlipRpt.ProcessingMode = ProcessingMode.Local;
                    rvSalaryPaySlipRpt.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/SalaryPaySlipDetailRpt.rdlc");

                    rvSalaryPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsSalaryPaySlip", data));
                    rvSalaryPaySlipRpt.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));

                    string parameterLine2 = "";

                    if (data[0].WithheldPaymentDate == null)
                    {
                        parameterLine2 = String.Concat("For the month of ", selectedMonth, "/", selectedYear);
                    }
                    else
                    {
                        DateTime WithheldPaymentDate = Convert.ToDateTime(data[0].WithheldPaymentDate);
                        parameterLine2 = String.Concat("Withheld salary for the month of "
                                            , selectedMonth
                                            , "/"
                                            , selectedYear
                                            , ","
                                            , " paid in "
                                            , UtilCommon.GetMonthName(WithheldPaymentDate.Month)
                                            , "/"
                                            , WithheldPaymentDate.Year);
                    }

                    ReportParameter p2 = new ReportParameter("param", parameterLine2);
                    rvSalaryPaySlipRpt.LocalReport.SetParameters(new ReportParameter[] { p2 });

                    this.rvSalaryPaySlipRpt.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                    rvSalaryPaySlipRpt.DataBind();

                    //ExportToPDF
                    String fileName = "PaySlipDetail_" + Guid.NewGuid() + ".pdf";
                    ExportToPDF(rvSalaryPaySlipRpt, fileName);

                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }

        }
        [NoCache]
        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                var dsName = string.Empty;
                var numErrorCode = new ObjectParameter("numErrorCode", typeof(int));
                var strErrorMsg = new ObjectParameter("strErrorMsg", typeof(string));

                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);

                switch (e.ReportPath)
                {
                    case "_PGMZoneWiseReportHeader":

                        dsName = "DSCompanyInfo";
                        var data = base.GetZoneInfoForReportHeader();
                        e.DataSources.Add(new ReportDataSource(dsName, data));
                        break;

                    case "_SalaryPaySlipDetailAddition":

                        int EmpID = Convert.ToInt32(e.Parameters[0].Values[0]);
                        var dataAddition = (from ad in base._pgmContext.sp_PGM_GetSalaryPaySlipDetailAdditionExtended(EmpID, selectedYear, selectedMonth, numErrorCode, strErrorMsg) select ad).OrderBy(x => x.SortPriority).ToList();

                        dsName = "dsSalaryPaySlipDetailAddition";
                        e.DataSources.Add(new ReportDataSource(dsName, dataAddition));

                        break;

                    case "_SalaryPaySlipDetailDeduction":

                        EmpID = Convert.ToInt32(e.Parameters[0].Values[0]);
                        var dataDeduction = (from ad in base._pgmContext.sp_PGM_GetSalaryPaySlipDetailDeductionExtended(EmpID, selectedYear, selectedMonth, numErrorCode, strErrorMsg) select ad).OrderBy(x => x.SortPriority).ToList();

                        dsName = "dsSalaryPaySlipDetailDeduction";
                        e.DataSources.Add(new ReportDataSource(dsName, dataDeduction));
                        break;
                }

            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }
        [NoCache]
        protected void rvSalaryPaySlipRpt_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Others
        [NoCache]
        private static string NumberToText(int number)
        {
            if (number == 0) return "Zero";

            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";

            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }

            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };

            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };

            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };

            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }

            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens

                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");

                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

        [NoCache]
        private void PopulateDropdownList()
        {
            int j = 0;
            foreach (var year in Common.PopulateYearList().ToList())
            {
                ddlSelectYear.Items.Insert(j, new ListItem() { Text = year.Value.ToString(), Value = year.Value.ToString() });
                j++;
            }

            int k = 0;
            foreach (var month in Common.PopulateMonthListForReport().ToList())
            {
                ddlSelectMonth.Items.Insert(k, new ListItem() { Text = month.Text.ToString(), Value = month.Value.ToString() });
                k++;
            }
            ddlSelectMonth.Items.FindByValue(DateTime.Now.Month.ToString()).Selected = true;

            ddlSelectEmployeeType.DataSource = _pgmContext.PRM_EmploymentType.OrderBy(x => x.Name).ToList();
            ddlSelectEmployeeType.DataValueField = "Id";
            ddlSelectEmployeeType.DataTextField = "Name";
            ddlSelectEmployeeType.DataBind();
            ddlSelectEmployeeType.Items.Insert(0, new ListItem("All", "0"));

            ddlEmpCategory.DataSource = _pgmContext.PRM_StaffCategory.OrderBy(x => x.Name).ToList();
            ddlEmpCategory.DataValueField = "Id";
            ddlEmpCategory.DataTextField = "Name";
            ddlEmpCategory.DataBind();
            ddlEmpCategory.Items.Insert(0, new ListItem("All", "0"));

            ddlSelectDesignation.DataSource = _pgmContext.PRM_Designation.OrderBy(x => x.Name).ToList();
            ddlSelectDesignation.DataValueField = "Id";
            ddlSelectDesignation.DataTextField = "Name";
            ddlSelectDesignation.DataBind();
            ddlSelectDesignation.Items.Insert(0, new ListItem("All", "0"));

            DateTime cDate = DateTime.Now;
            var prm_SalaryScaleEntity = _pgmContext.PRM_SalaryScale.Where(t => t.DateOfEffective <= cDate).OrderByDescending(t => t.DateOfEffective).First();

            ddlSelectJobGrade.DataSource = _pgmContext.PRM_JobGrade.Where(t => t.SalaryScaleId == prm_SalaryScaleEntity.Id).ToList();
            ddlSelectJobGrade.DataValueField = "Id";
            ddlSelectJobGrade.DataTextField = "GradeName";
            ddlSelectJobGrade.DataBind();
            ddlSelectJobGrade.Items.Insert(0, new ListItem("All", "0"));

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            ddlEmployee.DataSource = GetEmployeeList();
            ddlEmployee.DataValueField = "Id";
            ddlEmployee.DataTextField = "DisplayText";
            ddlEmployee.DataBind();
            ddlEmployee.Items.Insert(0, new ListItem("All", "0"));

        }

        #endregion

    }
}