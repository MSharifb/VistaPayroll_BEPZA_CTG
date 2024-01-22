<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="BankAdviceLetterRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.BankAdviceLetterRpt" %>

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
                            <asp:Label ID="lbl100" Text="Zone" runat="server" /><label class="required-field">*</label>
                        </span>
                        <span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectZone">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label4" Text="Type of Letter " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectLetterType" CssClass="required" OnSelectedIndexChanged="ddlSelectLetterType_OnSelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </span>
                        <span class="label-right">
                            <asp:Label ID="Label1" Text="Year" runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectYear" OnSelectedIndexChanged="ddlSelectYear_OnSelectedIndexChanged" AutoPostBack="true" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span>
                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label7" Text="Month " runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlSelectMonth" OnSelectedIndexChanged="ddlSelectMonth_OnSelectedIndexChanged" AutoPostBack="true" CssClass="form-control select-single">
                            </asp:DropDownList>
                        </span><span class="label-right">
                            <asp:Label ID="Label5" Text="Bonus Type" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlBonusType" AutoPostBack="true">
                            </asp:DropDownList>
                        </span>

                    </div>

                    <div class="row">
                        <span class="label">
                            <asp:Label ID="Label2" Text="Bank Name" runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList runat="server" ID="ddlBankName" CssClass="required form-control select-single" OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged" AutoPostBack="true" >
                            </asp:DropDownList>
                        </span><span class="label-right">
                            <asp:Label ID="Label3" Text="Branch Name" runat="server" />
                            <label class="required-field">
                                *</label>
                        </span><span class="field">
                            <asp:DropDownList ID="ddlBranchName" runat="server" AutoPostBack="true">
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
                    <%--<div class="row">
                        <span class="label-right">&nbsp; </span><span class="field">
                            <div class="button-crude button-left">
                                <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                                    OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                            </div>
                        </span>
                    </div>
                    <div class="clear">
                    </div>--%>
                </div>
                <div>
                    <rsweb:ReportViewer ID="rvAdviceLetterRpt" runat="server" Width="100%" Height="100%" Visible="true"
                        OnReportRefresh="rvAdviceLetterRpt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvAdviceLetterRpt" />
            <asp:PostBackTrigger ControlID="ddlSelectZone" />
            <asp:PostBackTrigger ControlID="btnViewReport" />
        </Triggers>
    </asp:UpdatePanel>
    <%--        <script type="text/javascript">

            //cascading dropdown
            $('#<%=ddlBankName.ClientID %>').live('change', function () {
                var id = parseInt($(this).val());
                populateBranch(id);
            })

            function populateBranch(id) {
                //if (id > 0) {
                $.ajax({
                    type: "POST",
                    url: "BankAdviceLetterRpt.aspx/GetBranchByBankId",
                    data: "{Id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var branchDDL = $('#<%=ddlBranchName.ClientID %>');
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
    </script>--%>
</asp:Content>
