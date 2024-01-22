<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    AutoEventWireup="true" CodeBehind="EmployeeBasicInfo.aspx.cs"
    Inherits="PGM.Web.Reports.PGM.viewers.EmployeeBasicInfo" %>


<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <fieldset>
                <div class="GroupBox" id="message">
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red">        
                    </asp:Label>
                </div>

                <div class="GroupBox">

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label10" Text="Zone" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field-LargeWidth">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label4" Text="Designation " runat="server" />

                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectDesignation" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label11" Text="Officer/Staff Category" runat="server" />

                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlEmpCategory">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row">
                        <%--<span class="label">
                            <asp:Label ID="Label2" Text="Department " runat="server" />

                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectDivision">
                            </asp:DropDownList>
                        </span>--%>
                         <span class="label">
                            <asp:Label ID="Label6" Text="Employee ID " runat="server" />
                        </span>
                        <span class="field">
                            <asp:TextBox ID="txtEmployeeID" runat="server">
                            </asp:TextBox>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label3" Text="Job Grade " runat="server" />

                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectJobGrade" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row GroupBox">
                        <div class="button-crude button-center">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />

                        </div>
                    </div>

                    <div class="clear">
                    </div>
                </div>

                <div id="report">
                    <rsweb:ReportViewer ID="rvEmployeeBasicInfo" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvEmployeeBasicInfo_ReportRefresh">
                    </rsweb:ReportViewer>
                </div>
            </fieldset>

        </ContentTemplate>

        <Triggers>
            <asp:PostBackTrigger ControlID="rvEmployeeBasicInfo" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>

    </asp:UpdatePanel>
</asp:Content>
