<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master" AutoEventWireup="true" 
    CodeBehind="JournalVoucherRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.JournalVoucherRpt" %>

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
                    <%--<div class="row">
                        <span class="label">
                            <asp:Label ID="lbl100" Text="Zone" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field-LargeWidth">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                        </span>
                    </div>--%>
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label1" Text="Year" runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectYear" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label7" Text="Month " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectMonth" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="GroupBox">
                        <div class="text-center" style="text-align: center">
                            <div class="button-center">
                                <asp:Button CssClass="btn btn-primary" Text="View Report" runat="server"
                                    ID="btnViewReport"
                                    ValidationGroup="view"
                                    OnClientClick="return fnValidate();"
                                    type="submit"
                                    OnClick="btnViewReport_Click" />
                            </div>
                        </div>
                        <div class="clear">
                        </div>
                    </div>
                </div>
                <div>
                    <rsweb:ReportViewer ID="rvJournalVoucherRpt" runat="server" Width="100%" Height="100%" Visible="true"
                        OnReportRefresh="rvJournalVoucherRpt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvJournalVoucherRpt" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>


