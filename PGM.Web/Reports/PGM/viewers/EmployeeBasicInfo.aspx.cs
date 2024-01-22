using PGM.Web.Utility;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using PGM.Web.Resources;

namespace PGM.Web.Reports.PGM.viewers
{
    public partial class EmployeeBasicInfo : PGMReportBase
    {
        bool status = false;

        [NoCache]
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (LoggedUserZoneInfoId == 0) return;

                PopulateDropdownList();
            }
        }

        #region Events

        [NoCache]
        protected void btnViewReport_Click(object sender, EventArgs e)
        {
            try
            {
                var jobGrade = Convert.ToInt32(ddlSelectJobGrade.SelectedValue);
                //var divisionId = Convert.ToInt32(ddlSelectDivision.SelectedValue);
                var designationId = Convert.ToInt32(ddlSelectDesignation.SelectedValue);
                var categoryId = Convert.ToInt32(ddlEmpCategory.SelectedValue);

                int employeeId = default(int);
                if (txtEmployeeID.Text != string.Empty)
                {
                    var empId = txtEmployeeID.Text.Trim().ToString();
                    employeeId = (from p in _pgmExecuteFunctions.GetEmployeeList()
                                  where p.EmpID.Trim().ToUpper() == empId.Trim().ToUpper()
                                  select p.Id).FirstOrDefault();
                }


                var zoneList = new List<FilteredZoneList>();
                foreach (ListItem item in ddlZone.Items)
                {
                    if (item.Selected)
                    {
                        var obj = new FilteredZoneList(Convert.ToInt32(item.Value), item.Text);
                        zoneList.Add(obj);
                    }
                }

                if (zoneList == null || zoneList.Count == 0)
                {
                    lblMsg.Text = ErrorMessages.ZoneRequired;
                }
                else
                {
                    lblMsg.Text = "";
                    GenerateReport(designationId
                        , jobGrade
                        , categoryId
                        , employeeId
                        , zoneList);
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
            }
        }

        [NoCache]
        void localReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            dynamic data = null;
            var dsName = "DSCompanyInfo";
            data = base.GetZoneInfoForReportHeader();
            e.DataSources.Add(new ReportDataSource(dsName, data));
        }

        [NoCache]
        protected void rvEmployeeBasicInfo_ReportRefresh(object sender, System.ComponentModel.CancelEventArgs e)
        {
            btnViewReport_Click(null, null);
        }
        #endregion

        #region Private Method

        private void GenerateReport(int designationId, int jobGrade, int categoryId, int employeeId, IEnumerable<FilteredZoneList> zoneList)
        {
            #region Processing Report Data

            var data = (from e in _pgmExecuteFunctions.GetEmployeeList()
                        select new
                        {
                            e.Id,
                            e.EmpID,
                            e.FullName,
                            e.DesignationName,
                            e.StaffCategoryName,
                            e.JobGradeName,
                            e.StepName,
                            e.BankAccountNo,
                            e.ETIN,
                            e.SortingOrder,
                            e.DateofJoining,
                            e.ZoneName,
                            e.DivisionId,
                            e.DesignationId,
                            e.JobGradeId,
                            e.StaffCategoryId,
                            e.SalaryWithdrawFromZoneId,
                            e.SalaryZoneName
                        })
                .OrderBy(x => x.SortingOrder)
                .ThenBy(x => x.DateofJoining)
                .ToList();

            if (txtEmployeeID.Text != string.Empty)
            {
                if (employeeId > 0) data = data.Where(q => q.Id == employeeId).ToList();
            }
            else
            {
                if (designationId > 0) data = data.Where(q => q.DesignationId == designationId).ToList();
                if (jobGrade > 0) data = data.Where(q => q.JobGradeId == jobGrade).ToList();
                if (categoryId > 0) data = data.Where(q => q.StaffCategoryId == categoryId).ToList();

                data = data.Where(x => zoneList.Select(n => n.Id).Contains(Convert.ToInt32(x.SalaryWithdrawFromZoneId)))
                    .OrderBy(x => x.SortingOrder)
                    .ThenBy(x => x.DateofJoining)
                    .ToList();
            }

            string searchParameters = "As on : " + DateTime.Now.ToString(DateAndTime.GlobalDateFormat);

            if (data.Count() > 0)
            {
                lblMsg.Text = "";
                rvEmployeeBasicInfo.Reset();
                rvEmployeeBasicInfo.ProcessingMode = ProcessingMode.Local;
                rvEmployeeBasicInfo.LocalReport.ReportPath = Server.MapPath("~/Reports/PGM/rdlc/EmployeeBasicInfo.rdlc");

                rvEmployeeBasicInfo.LocalReport.DataSources.Add(new ReportDataSource("dsEmployeeBasicInfo", data));

                ReportParameter p1 = new ReportParameter("param", searchParameters);
                rvEmployeeBasicInfo.LocalReport.SetParameters(new ReportParameter[] { p1 });
            }
            else
            {
                status = true;
                rvEmployeeBasicInfo.Reset();
            }

            this.rvEmployeeBasicInfo.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(localReport_SubreportProcessing);
            rvEmployeeBasicInfo.DataBind();

            //ExportToPDF
            String fileName = "EmployeeBasicInfo_" + Guid.NewGuid() + ".pdf";
            ExportToPDF(rvEmployeeBasicInfo, fileName);

            #endregion
        }


        [NoCache]
        private void PopulateDropdownList()
        {

            //ddlSelectDivision.DataSource = context.PRM_Division
            //    .OrderBy(x => x.Name).ToList();
            //ddlSelectDivision.DataValueField = "Id";
            //ddlSelectDivision.DataTextField = "Name";
            //ddlSelectDivision.DataBind();
            //ddlSelectDivision.Items.Insert(0, new ListItem("All", "0"));


            ddlSelectDesignation.DataSource = _pgmContext.PRM_Designation
                .OrderBy(x => x.Name).ToList();
            ddlSelectDesignation.DataValueField = "Id";
            ddlSelectDesignation.DataTextField = "Name";
            ddlSelectDesignation.DataBind();
            ddlSelectDesignation.Items.Insert(0, new ListItem("All", "0"));

            ddlEmpCategory.DataSource = _pgmContext.PRM_StaffCategory.OrderBy(x => x.Name).ToList();
            ddlEmpCategory.DataValueField = "Id";
            ddlEmpCategory.DataTextField = "Name";
            ddlEmpCategory.DataBind();
            ddlEmpCategory.Items.Insert(0, new ListItem("All", "0"));

            DateTime cDate = DateTime.Now;
            var prm_SalaryScaleEntity = _pgmContext.PRM_SalaryScale
                .Where(t => t.DateOfEffective <= cDate)
                .OrderByDescending(t => t.DateOfEffective).First();

            ddlSelectJobGrade.DataSource = _pgmContext.PRM_JobGrade
                .Where(t => t.SalaryScaleId == prm_SalaryScaleEntity.Id)
                .ToList();
            ddlSelectJobGrade.DataValueField = "Id";
            ddlSelectJobGrade.DataTextField = "GradeName";
            ddlSelectJobGrade.DataBind();
            ddlSelectJobGrade.Items.Insert(0, new ListItem("All", "0"));

            ddlZone.DataSource = ZoneListCached;
            ddlZone.DataValueField = "Value";
            ddlZone.DataTextField = "Text";
            ddlZone.DataBind();
            ddlZone.Items.FindByValue(LoggedUserZoneInfoId.ToString()).Selected = true;
        }


        #endregion

    }
}