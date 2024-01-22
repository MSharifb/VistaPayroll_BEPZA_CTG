<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master" AutoEventWireup="true"
    CodeBehind="NightBillRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.NightBillRpt" %>


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
                            <asp:Label ID="lbl100" Text="Zone" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                        </span>
                    </div>
                    <div class="row">
                        <span class="label">Department
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlDepartment">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">Designation
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlDesignation" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">Year<label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlYear">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">Month<label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlMonth">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="row">
                        <span class="label">Employee Name
                        </span>
                        <span class="field">
                            <asp:TextBox ID="txtEmpName" runat="server" Width="200px"></asp:TextBox>
                        </span>

                        <span class="label-right">Employee ID
                        </span>
                        <span class="field">
                            <asp:TextBox ID="txtIcNo" runat="server" Width="200px"></asp:TextBox>
                        </span>

                    </div>
                     <div class="GroupBox">
                            <div class="text-center" style="text-align: center">
                                <div class="button-center">
                                    <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                                        type="submit" OnClick="btnViewReport_Click" />
                                </div>
                            </div>
                        <div class="clear">
                        </div>
                    </div>
                    <%--<div class="row">
                        <span class="label-right">&nbsp; </span>
                        <span class="field">
                            <div class="button-crude button-left">
                                <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" OnClientClick="return fnValidate();"
                                    type="submit" OnClick="btnViewReport_Click" />
                            </div>
                        </span>
                    </div>
                    <div class="clear">
                    </div>--%>
                </div>
                <div class="GroupBox">
                    <rsweb:ReportViewer ID="rvNightBill" runat="server" Width="100%"
                        Height="100%" OnReportRefresh="rvNightBill_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvNightBill" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
