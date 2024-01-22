<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="IncomeTaxComputationRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.IncomeTaxComputationRpt" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="max-height: 15px" id="message">
                <asp:Label ID="lblMsg" runat="server" ForeColor="Red" ClientIDMode="Static">        
                </asp:Label>
                <asp:Label ID="lblErrmsg" runat="server" ForeColor="Red" ClientIDMode="Static"></asp:Label>
                <div class="clear">
                </div>
            </div>
            <div class="GroupBox">
                <div class="row">
                    <span class="label">
                        <asp:Label ID="Label2" Text="Income Year " runat="server" />
                        <label class="required-field">
                            *</label>
                    </span><span class="field">
                        <asp:DropDownList runat="server" ID="ddlIncomeYear" CssClass="required form-control select-single">
                        </asp:DropDownList>
                    </span><span class="label-right">
                        <asp:Label ID="Label4" Text="Employee Id" runat="server" />
                        <label class="required-field">
                            *</label>
                    </span><span class="field">
                        <asp:TextBox ID="txtEmployeeInitial" runat="server" CssClass="required">
                        </asp:TextBox>
                        <asp:HiddenField runat="server" ID="hfEmploId" />
                    </span>
                </div>
                <div class="row GroupBox">
                    <div class="button-crude button-center">
                        <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view" ClientIDMode="Static"
                            OnClientClick=" return validation(); " type="submit" OnClick="btnViewReport_Click" />
                    </div>
                    <div style="padding: 10px 0 0 0;">
                        <asp:Label runat="server" ID="lblLoadingMessage" ForeColor="blue" Text="Loading..." ClientIDMode="Static"></asp:Label>
                    </div>
                </div>
            </div>
            <div>
                <rsweb:ReportViewer ID="rvIncomeTaxComputationRpt" runat="server" Width="100%" Height="100%" Visible="true"
                    OnReportRefresh="rvIncomeTaxComputationRpt_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvIncomeTaxComputationRpt" />
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

            if (!isValid) {
                loadingLabel.hide();
            } else {
                loadingLabel.show();
            }

            return isValid;
        }

    </script>
</asp:Content>


