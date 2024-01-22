<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="MonthlySalaryRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.MonthlySalaryRpt" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

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
                            <asp:Label ID="Label12" Text="Report" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:DropDownList ID="DdlReportType" runat="server" CssClass=" required">
                                <asp:ListItem Enabled="true" Text="Select One" Value="0"></asp:ListItem>
                                <asp:ListItem Text="Montly Salary Statement" Value="1"></asp:ListItem>
                                <asp:ListItem Text="Details on Other Allowance" Value="2"></asp:ListItem>
                                <asp:ListItem Text="Details on Other Deduction" Value="3"></asp:ListItem>
                            </asp:DropDownList>

                        </span>
                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label1" Text="Salary Year" runat="server" />

                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectYear" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label7" Text="Salary Month " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectMonth" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label11" Text="Officer/Staff Category" runat="server" />

                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlEmpCategory">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label3" Text="Job Grade " runat="server" />

                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectJobGrade" CssClass="form-control select-single">
                            </asp:DropDownList>
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
                            <asp:Label ID="Label5" Text="Employment Type " runat="server" />

                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectEmployeeType">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label8" Text="Bank Name" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlBankName" AutoPostBack="True" OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span><span class="label-right">
                            <asp:Label ID="Label9" Text="Branch Name" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList ID="ddlBranchName" runat="server">
                            </asp:DropDownList>
                        </span>
                    </div>


                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label6" Text="Withheld Statement " runat="server" />
                        </span>
                        <span class="field">
                            <asp:CheckBox runat="server" ID="chkWithheldStatment"></asp:CheckBox>
                        </span>
                    </div>


                    <div class="row GroupBox">
                        
                        <div class="button-crude button-center">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" ClientIDMode="Static"
                                OnClientClick="return validation();" type="submit" OnClick="btnViewReport_Click" />
                        </div>
                        
                        <div style="padding: 10px 0 0 0;">
                            <asp:Label runat="server" ID="lblLoadingMessage" ForeColor="blue" Text="Loading..." ClientIDMode="Static"></asp:Label>
                        </div>

                    </div>

                    <div class="clear">
                    </div>
                </div>

                <div id="report">
                    <rsweb:ReportViewer ID="rvMonthlySalaryRpt" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvMonthlySalaryRpt_ReportRefresh"
                        AsyncRendering="True" ProcessingMode="Remote" SizeToReportContent="True">
                    </rsweb:ReportViewer>
                </div>
            </fieldset>

        </ContentTemplate>

        <Triggers>
            <%--<asp:PostBackTrigger ControlID="rvMonthlySalaryRpt" />--%>
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
            <asp:PostBackTrigger ControlID="ddlBankName" />
        </Triggers>

    </asp:UpdatePanel>

    <script type="text/javascript">
        $(document).ready(function () {
            var loadingLabel = $("#<%= lblLoadingMessage.ClientID %>");
            loadingLabel.hide();
        });

        function validation() {
            var loadingLabel = $("#<%= lblLoadingMessage.ClientID %>");
            var isValid = fnValidate();
            
            loadingLabel.hide();
            if (isValid) {
                loadingLabel.show();
            }

            return isValid;
        }
    </script>

</asp:Content>

