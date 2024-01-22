<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master" AutoEventWireup="true" 
    CodeBehind="LeaveEncashmentRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.LeaveEncashmentRpt" %>

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
<%--                    <div class="row">
                        <asp:RadioButton ID="RadioButton1" runat="server" GroupName="rpt" Text="Month Range" />
                        <asp:RadioButton ID="RadioButton2" runat="server" GroupName="rpt" Text="Individual Employee" />
                        <asp:RadioButton ID="RadioButton3" runat="server" GroupName="rpt" Text="All" />
                    </div>--%>
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="lbl100" Text="Zone" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                        </span>
                    </div>
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label1" Text="From Month" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlFromYear" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlFromMonth" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label2" Text="To Month" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlToYear" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlToMonth" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label3" Text="Employee" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="GroupBox">
                        <div class="button-crude">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                </div>

                <div>
                    <rsweb:ReportViewer ID="rvMyPaySlipRpt" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvMyPaySlipRpt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
<%--            <asp:PostBackTrigger ControlID="rvMyPaySlipRpt" />--%>
             <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport"/>

        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
