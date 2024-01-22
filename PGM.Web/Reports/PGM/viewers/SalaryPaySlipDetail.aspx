<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="SalaryPaySlipDetail.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.SalaryPaySlipDetail" %>

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
                            <asp:Label ID="lblZone" Text="Zone" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:ListBox SelectionMode="Multiple" runat="server" ID="ddlZone" ClientIDMode="Static"></asp:ListBox>
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
                            <asp:Label ID="Label8" Text="Officer/Staff Category " runat="server" />

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
                            <asp:Label ID="Label6" Text="Employee ID " runat="server" />
                        </span>
                        <span class="field">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="GroupBox">
                        <span id="lblWait" style="font-size: 12pt; color: blue; float: right; padding-top: 5px;">Please Wait...</span>
                        <div class="button-crude">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                            &nbsp;<asp:Button ID="btnSendMail" runat="server" OnClick="btnSendMail_Click" OnClientClick="return Confirm()" Text="Send Mail" UseSubmitBehavior="true" />
                        </div>
                        <%--<div class="button-crude">

                                    <asp:Button Text="Send Mail" runat="server" ID="btnSendMail" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnSendMail_Click" />
                                </div>--%>
                        <div class="clear">
                        </div>
                    </div>
                </div>

                <div class="GroupBox">
                    <rsweb:ReportViewer ID="rvSalaryPaySlipRpt" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvSalaryPaySlipRpt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvSalaryPaySlipRpt" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />

        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        $(function () {
            $('#lblWait').hide();
        });
        $('#btnViewReport').click(function () {
            $('#lblWait').show();
        });

        function Confirm() {
            //fnValidate();
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm("Do you want to send mail?")) {
                confirm_value.value = "Yes";
                return true;
            } else {
                confirm_value.value = "No";
                return false;
            }

        }
    </script>
</asp:Content>
