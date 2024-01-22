<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master" AutoEventWireup="true"
    CodeBehind="HeadWiseReport.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.HeadWiseReport" %>

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
                          <span class="label">
                            <asp:Label ID="heads" Text="Head" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlHead" ClientIDMode="Static"></asp:ListBox>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">Month
                        <label class="required-field">*</label></span><span class="field">
                            <asp:DropDownList  runat="server" ID="ddlYear" style="width:50%" CssClass="form-control select-single"></asp:DropDownList>
                            <asp:DropDownList runat="server" ID="ddlMonth" style="width:50%" CssClass="form-control select-single"></asp:DropDownList>
                        </span>
                    </div>
                    <div class="row">
                        <span class="label">Officer / Staff Category
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlEmployeeStaff">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label2" Text="Bank Name" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlBankName" AutoPostBack="True"
                                OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged"
                                CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span><span class="label-right">
                            <asp:Label ID="Label3" Text="Branch Name" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList ID="ddlBranchName" runat="server" >
                            </asp:DropDownList>
                        </span>
                    </div>
                     <div class="GroupBox">
                            <div class="text-center" style="text-align: center">
                                <div class="button-center">
                                    <asp:Button CssClass="btn btn-primary" Text="View Report"
                                        runat="server" ID="btnViewReport" ValidationGroup="view"
                                        OnClientClick="return fnValidate();"
                                        type="submit" OnClick="btnViewReport_Click" />
                                </div>
                            </div>
                        <div class="clear">
                        </div>
                    </div>
                </div>
                <div class="GroupBox">
                    <rsweb:ReportViewer ID="rvHeadWiseReport" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvHeadWiseReport_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvHeadWiseReport" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="ddlHead" />
            <asp:PostBackTrigger ControlID="ddlBankName" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
