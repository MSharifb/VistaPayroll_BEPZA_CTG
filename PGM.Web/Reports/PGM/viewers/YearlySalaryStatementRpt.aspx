<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="YearlySalaryStatementRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.YearlySalaryStatementRpt" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <fieldset>
                <div class="GroupBox" id="message">
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red">
                    </asp:Label>
                    <asp:Label ID="Errmsg" runat="server" ForeColor="Red">
                    </asp:Label>
                </div>
                <div class="GroupBox">
                    <div style="display: none;">
                        <div class="row">
                            <span class="label">
                                <asp:Label ID="lbl100" Text="Zone" runat="server" /><label class="required-field">*</label>
                            </span>
                            <span class="field">
                                <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
                            </span>
                        </div>
                    </div>
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label1" Text="Financial Year" runat="server" />

                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectFinancialYear" CssClass="required form-control select-single">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label6" Text="Employee ID" runat="server" />

                            <label class="required-field">
                                *</label>
                        </span>
                        <span class="field">
                            <asp:DropDownList CssClass="form-control select-single required" runat="server" ID="ddlEmployee" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div style="display: none;">
                        <div class="row">
                              <span class="label">
                                <asp:Label ID="Label5" Text="Employment Type " runat="server" />

                            </span>
                            <span class="field">
                                <asp:DropDownList runat="server" ID="ddlSelectEmployeeType">
                                </asp:DropDownList>
                            </span>
                            <span class="label-right">
                                <asp:Label ID="Label3" Text="Job Grade " runat="server" />

                            </span>
                            <span class="field">
                                <asp:DropDownList runat="server" ID="ddlSelectJobGrade">
                                </asp:DropDownList>
                            </span>
                        </div>

                        <div class="row">
                            <span class="label">
                                <asp:Label ID="Label4" Text="Designation " runat="server" />

                            </span>
                            <span class="field">
                                <asp:DropDownList runat="server" ID="ddlSelectDesignation">
                                </asp:DropDownList>
                            </span>

                        </div>
                    </div>
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

                <div class="GroupBox">
                    <rsweb:ReportViewer ID="rvYearlySalaryStatementRpt" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvYearlySalaryStatementRpt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvYearlySalaryStatementRpt" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
