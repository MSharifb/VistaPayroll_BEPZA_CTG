using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class MonthlyOtherAdjustment : PGMReportBase
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoggedUserZoneInfoId == 0) return;

            if (!this.IsPostBack)
            {
                PopulateDropdownList();
            }
        }

        #region Button Event

        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = string.Empty;

                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);
                var employeeId = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());

                var selectedZoneLists = new List<int>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        selectedZoneLists.Add(Convert.ToInt32(item.Value));
                    }
                }

                GenerateReport(selectedYear
                    , selectedMonth
                    , employeeId
                    , ConvertListToString(selectedZoneLists.ToArray()));
            }
            catch (Exception ex)
            {
                lblMsg.Text = CommonExceptionMessage.GetExceptionMessage(ex, CommonAction.General);
            }
        }

        #endregion

        #region Send mail

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                lblMsg.Text = default(string);

                #region Get Data

                var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
                var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);

                int selectedEmployeeID = Convert.ToInt32(ddlEmployee.SelectedValue.ToString());

                var data = (from s in base._pgmContext.vwPGMSalaryPaySlipRpts.Where(y => y.SalaryYear == selectedYear)
                            select s).Where((sm => sm.SalaryMonth == selectedMonth)).OrderBy(o => o.SortPriority).DistinctBy(x => x.EmpID).ToList();


                #endregion

                #region Get Sender mail Info
                var loginUserId = Convert.ToInt32(MyAppSession.EmpId);
                FromEmailPersonName = loginUserId != 0 ? _pgmExecuteFunctions.GetEmployeeList().Where(q => q.Id == loginUserId).SingleOrDefault().FullName : null;
                FromEmailAddress = loginUserId != 0 ? _pgmExecuteFunctions.GetEmployeeList().Where(q => q.Id == loginUserId).SingleOrDefault().EmialAddress : null;

                if (FromEmailAddress == string.Empty || FromEmailAddress == null)
                {
                    lblMsg.Text = "Sender mail address is empty.";
                    return;
                }
                else if (FromEmailAddress != string.Empty)
                {
                    lblMsg.Text = "Email send process is in progress, it taking time to send mail to all receiver.";
                }
                #endregion

                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        int EmpID = Convert.ToInt32(item.EmployeeId);
                        ToEmailAddress = _pgmExecuteFunctions.GetEmployeeList().Where(q => q.Id == EmpID).SingleOrDefault().EmialAddress == null ? "" : _pgmExecuteFunctions.GetEmployeeList().Where(q => q.Id == EmpID).SingleOrDefault().EmialAddress;
                        CCEmailAddress = "";
                        Subject = "Salary Pay Slip";
                        Salutation = "Dear Mr./Ms.";

                        #region Check Velidation

                        if (ToEmailAddress == string.Empty)
                        {
                            lblMsg.Text = "Receiver mail address is empty.";
                        }
                        else if (FromEmailAddress != string.Empty && ToEmailAddress != string.Empty)
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

        private string CreateBody(DAL.PGM.vwPGMSalaryPaySlipRpt item)
        {
            var selectedYear = Convert.ToString(ddlSelectYear.SelectedValue);
            var selectedMonth = Convert.ToString(ddlSelectMonth.SelectedItem);

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
                    var OtherAllowanceDetailsDetailAddition = (from ad in base._pgmContext.GetSalaryPaySlipDetailAddition(EmployeeId, selectedYear, selectedMonth, numErrorCode, strErrorMsg) select ad).OrderBy(x => x.SortPriority).ToList();

                    content += "<table style='border: 1px solid black; width:80%;'><tr><td>";
                    content += "<table style='width:60%;'>";
                    content += "<tr><td width='50%'><b>" + "Salary :" + "</b></td><td>" + "" + "</td></tr>";

                    foreach (var subItem1 in OtherAllowanceDetailsDetailAddition)
                    {
                        content += "<tr><td width='50%'>" + subItem1.HeadName + "</td><td>" + subItem1.HeadAmount + "</td></tr>";
                        grossSalary += subItem1.HeadAmount;
                    }

                    content += "<tr><td width='50%'><b>" + "Gross Salary (A)" + "</b></td><td><b>" + grossSalary + "</b></td></tr>";
                    content += "</table>";
                    content += "</td></tr></table>";

                    //Salary PaySlip Detail Deduction
                    //var OtherAllowanceDetailsDetailDeduction = (from ad in base.pgmContext.GetOtherAllowanceDetailsDetailDeduction(EmployeeId, selectedYear, selectedMonth, numErrorCode, strErrorMsg).OrderBy(o=>o.HeadType) select ad).ToList();

                    var salaryHeadGroupNameStart = ""; var salaryHeadGroupNameEnd = "";

                    var OtherAllowanceDetailsDetailDeduction = (from t in base._pgmContext.GetSalaryPaySlipDetailDeduction(EmployeeId, selectedYear, selectedMonth, numErrorCode, strErrorMsg).OrderBy(o => o.HeadType)
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

                    foreach (var subItem2 in OtherAllowanceDetailsDetailDeduction)
                    {
                        salaryHeadGroupNameStart = subItem2.SalaryHeadGroupName;

                        if (salaryHeadGroupNameStart != salaryHeadGroupNameEnd)
                        {
                            content += "<tr><td width='50%'><b>" + subItem2.SalaryHeadGroupName + "</b></td><td>" + "" + "</td></tr>";
                        }

                        content += "<tr><td width='50%'>" + subItem2.HeadName + "</td><td>" + subItem2.HeadAmount + "</td></tr>";
                        totalDeduction += subItem2.HeadAmount;

                        salaryHeadGroupNameEnd = subItem2.SalaryHeadGroupName;
                    }

                    content += "<tr><td width='50%'><b>" + "Total Deduction (B)" + "</b></td><td><b>" + totalDeduction + "</b></td></tr>";
                    content += "<tr><td width='50%'><b>" + "Net Payable (A-B)" + "</b></td><td><b>" + (grossSalary - totalDeduction) + "</b></td></tr>";
                    content += "</table>";
                    content += "</td></tr></table>";
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

        private void GenerateReport(string selectedYear, string selectedMonth, int employeeId, String zoneList)
        {
            #region Processing Report Data
            rvOtherAdjustmentDetailsRptVwr.Reset();

            var data = _pgmExecuteFunctions.GetOtherAdjustmentReport(selectedYear, selectedMonth, zoneList, employeeId);
            var reportHeader = base.GetZoneInfoForReportHeader();

            if (!data.Any())
            {
                lblMsg.Text = Common.GetCommomMessage(CommonMessage.DataNotFound);
            }
            else
            {
                rvOtherAdjustmentDetailsRptVwr.ProcessingMode = ProcessingMode.Local;
                rvOtherAdjustmentDetailsRptVwr.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/MonthlyOtherAdjustmentRpt.rdlc");

                rvOtherAdjustmentDetailsRptVwr.LocalReport.DataSources.Add(new ReportDataSource("DSOtherAdjustment", data));
                //rvOtherAdjustmentDetailsRptVwr.LocalReport.DataSources.Add(new ReportDataSource("dsCompanyInfo", reportHeader));

                string parameterLine2 = String.Concat("For the month of ", selectedMonth, "/", selectedYear);

                ReportParameter p1 = new ReportParameter("param", parameterLine2);
                rvOtherAdjustmentDetailsRptVwr.LocalReport.SetParameters(new ReportParameter[] { p1 });

                this.rvOtherAdjustmentDetailsRptVwr.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
                rvOtherAdjustmentDetailsRptVwr.DataBind();

                //ExportToPDF
                String fileName = "MonthlyOtherAdjustments_" + Guid.NewGuid() + ".pdf";
                ExportToPDF(rvOtherAdjustmentDetailsRptVwr, fileName);
            }
            #endregion
        }

        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var dsName = string.Empty;
            switch (e.ReportPath)
            {
                case "_PGMZoneWiseReportHeaderA4LandS":
                    dsName = "DSCompanyInfo";
                    var data = base.GetZoneInfoForReportHeader();
                    e.DataSources.Add(new ReportDataSource(dsName, data));
                    break;
            }
        }

        protected void rvOtherAdjustmentRptVwr_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }

        #region Others

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

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;

            var empList = GetEmployeeList();
            if (empList != null)
            {
                ddlEmployee.DataSource = empList;
                ddlEmployee.DataValueField = "Id";
                ddlEmployee.DataTextField = "DisplayText";
                ddlEmployee.DataBind();
                ddlEmployee.Items.Insert(0, new ListItem("All", "0"));
            }
        }

        #endregion
    }
}