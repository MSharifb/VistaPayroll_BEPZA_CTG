<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="IncomeTaxSummaryRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.IncomeTaxSummaryRpt" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <fieldset>
                <div class="GroupBox" style="max-height: 15px" id="message">
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red">
                    </asp:Label>
                    <asp:Label ID="lblErrmsg" runat="server" ForeColor="Red"></asp:Label>
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
                        <span class="label">
                            <asp:Label ID="Label4" Text="Income Year " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectIncomeYear" CssClass="required form-control select-single" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>

                        <span class="label-right">
                            <asp:Label ID="Label6" Text="Employee ID" runat="server" />
                        </span>
                        <span class="field">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="clear"></div>
                    <div class="row GroupBox">
                        <div class="button-crude button-center">
                            <asp:Button Text="View Report" runat="server"
                                ID="btnViewReport"
                                ValidationGroup="view"
                                OnClientClick="return fnValidate();"
                                type="submit"
                                OnClick="btnViewReport_Click" />
                        </div>
                    </div>
                    </span>
                <div class="clear">
                </div>
                </div>
                <div class="GroupBox">
                    <rsweb:ReportViewer ID="rvIncomeTaxSummaryRt" runat="server" Width="100%" Height="100%" Visible="true"
                        OnReportRefresh="rvIncomeTaxSummaryRt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvIncomeTaxSummaryRt" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>

    </asp:UpdatePanel>

</asp:Content>
