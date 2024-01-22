<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master" AutoEventWireup="true"
    CodeBehind="BankWiseMonthlyElectricityBillRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.BankWiseMonthlyElectricityBillRpt" %>

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
                        <span class="label">Bank
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlBank" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">Branch
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlBranch" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label3" Text="Department" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlDepartment">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">Month
                        <label class="required-field">*</label></span><span class="field"><asp:DropDownList runat="server" ID="ddlMonth">
                        </asp:DropDownList>
                        </span>
                    </div>
                    <div class="row">

                        <span class="label">
                            <asp:Label ID="Label1" Text="Employee ID" runat="server" />

                        </span>
                        <span class="field">
                            <asp:TextBox ID="txtIcNo" runat="server" Width="200px"></asp:TextBox>
                        </span>
                        <span class="label-right">Year
                        <label class="required-field">*</label></span><span class="field"><asp:DropDownList runat="server" ID="ddlYear">
                        </asp:DropDownList>
                        </span>
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

                </div>
                <div class="GroupBox">
                    <rsweb:ReportViewer ID="rvBankWiseMonthlyElectricityBill" runat="server" Width="100%"
                        Height="100%" OnReportRefresh="rvBankWiseMonthlyElectricityBill_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvBankWiseMonthlyElectricityBill" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">

        //cascading dropdown
        $('#<%=ddlBank.ClientID %>').change(function () {
            var id = parseInt($(this).val());
            populateBranch(id);
        })

        function populateBranch(id) {
            //if (id > 0) {
            $.ajax({
                type: "POST",
                url: "BankWiseMonthlyElectricityBillRpt.aspx/GetBranchByBankId",
                data: "{Id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var branchDDL = $('#<%=ddlBranch.ClientID %>');
                    branchDDL.empty();
                    branchDDL.append($('<option/>', {
                        value: '0',
                        text: 'All'
                    }));
                    $.each(response.d, function (key, value) {
                        branchDDL.append($("<option></option>").val(value.Id).html(value.Name));
                    });
                }
            });
        }

                // }
    </script>
</asp:Content>
