<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="MonthlyOtherAdjustment.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.MonthlyOtherAdjustment" %>

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
                            <asp:DropDownList runat="server" ID="ddlSelectYear">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label7" Text="Salary Month " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectMonth">
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
                        <span class="label-right">
                            &nbsp;
                        </span>
                        <span class="field">
                            &nbsp;
                        </span>
                    </div>

                    <div class="GroupBox">
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
                    <rsweb:ReportViewer ID="rvOtherAdjustmentDetailsRptVwr" runat="server" Width="100%"
                        Height="100%" OnReportRefresh="rvOtherAdjustmentRptVwr_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvOtherAdjustmentDetailsRptVwr" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">

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
            // document.forms[0].appendChild(confirm_value);
        }
    </script>
</asp:Content>
