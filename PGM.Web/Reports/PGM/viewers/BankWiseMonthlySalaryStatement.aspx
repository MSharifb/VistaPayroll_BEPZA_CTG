<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master" AutoEventWireup="true" 
    CodeBehind="BankWiseMonthlySalaryStatement.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.BankWiseMonthlySalaryStatement" %>

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
                            <asp:Label ID="Label4" Text="Type of Statement " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectLetterType" CssClass="required" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label5" Text="Bonus Type" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlBonusType" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>

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
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label2" Text="Bank Name" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlBankName" ClientIDMode="Static" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span><span class="label-right">
                            <asp:Label ID="Label3" Text="Branch Name" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList ID="ddlBranchName" runat="server" ClientIDMode="Static" >
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
                    <rsweb:ReportViewer ID="rvAdviceLetterRpt" runat="server" Width="100%" Height="100%" Visible="true"
                        OnReportRefresh="rvAdviceLetterRpt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
                <asp:HiddenField runat="server" ID="hfSelectedBranchId" ClientIDMode="Static" Value="0" />
            </fieldset>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvAdviceLetterRpt" />
            <asp:PostBackTrigger ControlID="ddlZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">

        var ddlBankNameId = $('#<%=ddlBankName.ClientID%>');
        var ddlletterTypeId = $('#<%=ddlSelectLetterType.ClientID%>');
        var ddlBonusTypeId = $('#<%=ddlBonusType.ClientID%>');
        var branchDDL = $('#<%=ddlBranchName.ClientID%>');
        var selectedBranchId = $('#<%=hfSelectedBranchId.ClientID%>');

        $(function () {

            var id = parseInt(ddlBankNameId.val());
            populateBranch(id);
            //---------------
            ToggleddlBonusType();

        });



        ddlBankNameId.live('change', function () {

            var id = parseInt($(this).val());
            populateBranch(id);
        });


        ddlletterTypeId.live('change',
            function () {
                ToggleddlBonusType();
            });

        function ToggleddlBonusType() {
            ddlBonusTypeId.attr("disabled", true);

            if (ddlletterTypeId.val() == "Bonus") {
                ddlBonusTypeId.attr("disabled", false);
            }
        }

        branchDDL.live('change', function () {
            selectedBranchId.val($(this).val());
        });


        function populateBranch(id) {
            //if (id > 0) {
            $.ajax({
                type: "POST",
                url: "BankWiseMonthlySalaryStatement.aspx/GetBranchByBankId",
                data: "{Id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    branchDDL.empty();
                    branchDDL.append($('<option/>', {
                        value: '0',
                        text: '[All]'
                    }));
                    $.each(response.d, function (key, value) {
                        branchDDL.append($("<option></option>").val(value.Id).html(value.Name));
                    });
                }
            });
        }



    </script>


</asp:Content>


