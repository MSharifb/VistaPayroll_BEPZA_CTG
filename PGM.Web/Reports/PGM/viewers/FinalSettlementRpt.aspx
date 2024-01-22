<%@ Page Title="" Language="C#" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master" AutoEventWireup="true" 
    CodeBehind="FinalSettlementRpt.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.FinalSettlementRpt" %>

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
                            <asp:Label ID="Label3" Text="Employee" runat="server" />
                        </span><span class="field">
                            <asp:DropDownList CssClass="form-control select-single" runat="server" ID="ddlEmployee" ClientIDMode="Static">
                            </asp:DropDownList>
                        </span>
                    </div>
                    <div class="GroupBox">
                        <div class="button-crude">
                            <asp:Button Text="View Report" runat="server" ID="btnViewReport" ValidationGroup="view"
                                OnClientClick="return fnValidate();" type="submit" OnClick="btnViewReport_Click" />
                        </div>
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
