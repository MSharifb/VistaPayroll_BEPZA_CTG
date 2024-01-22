<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="HeadWiseSummaryReport.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.HeadWiseSummaryReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="max-height: 15px" id="message">
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
                    <span class="label">From Month
                        <label class="required-field">*</label>
                    </span>
                    <span class="field">
                        <asp:DropDownList runat="server" ID="ddlFromYear" Style="width: 50%" CssClass="form-control select-single"></asp:DropDownList>
                        <asp:DropDownList runat="server" ID="ddlFromMonth" Style="width: 50%" CssClass="form-control select-single"></asp:DropDownList>
                    </span>
                    <span class="label-right">To Month
                        <label class="required-field">*</label>
                    </span>
                    <span class="field">
                        <asp:DropDownList runat="server" ID="ddlToYear" Style="width: 50%" CssClass="form-control select-single"></asp:DropDownList>
                        <asp:DropDownList runat="server" ID="ddlToMonth" Style="width: 50%" CssClass="form-control select-single"></asp:DropDownList>
                    </span>
                </div>
                <div class="row">
                    <span class="label">
                        <asp:Label ID="Label4" Text="Salary Head " runat="server" />
                        <label class="required-field">*</label>
                    </span><span class="field">
                        <asp:DropDownList runat="server" ID="ddlSingleHead" CssClass="form-control select-single" ClientIDMode="Static">
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
            <div>
                <rsweb:ReportViewer ID="rvHeadWiseSummaryReport" runat="server" Width="100%" Height="100%" Visible="true"
                    OnReportRefresh="rvHeadWiseReportByMonthRange_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvHeadWiseSummaryReport" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>

    </asp:UpdatePanel>

</asp:Content>
