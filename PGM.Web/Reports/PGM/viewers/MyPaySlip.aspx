<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
    CodeBehind="MyPaySlip.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.MyPaySlip" %>

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
                  
                    <div class="GroupBox">
                                <div class="button-crude">
                                    <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                                        OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                                   
                                </div>
                                <%--<div class="button-crude">

                                    <asp:Button Text="Send Mail" runat="server" ID="btnSendMail" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnSendMail_Click" />
                                </div>--%>
                        <div class="clear">
                        </div>
                    </div>
                </div>

                <div>
                    <rsweb:ReportViewer ID="rvMyPaySlipRpt" runat="server" Width="100%" Visible="true"
                        Height="100%" OnReportRefresh="rvMyPaySlipRpt_ReportRefresh">
                    </rsweb:ReportViewer>
                    <div class="clear">
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvMyPaySlipRpt" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
