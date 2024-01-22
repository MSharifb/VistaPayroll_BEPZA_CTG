<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports/RptCommon/MasterPage/ReportMaster.Master"
CodeBehind="BankAdviceLetterCommonView.aspx.cs" Inherits="PGM.Web.Reports.PGM.viewers.BankAdviceLetterCommonView" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="GroupBox" style="width: 98%; margin: 10px auto; min-height: 15px" id="message">
                <div class="row">
                    
                   <span class="field">
                    <div class="button-crude button-left">
                        <asp:Button Text="Go Back" runat="server" ID="btnBack" ValidationGroup="view" type="submit"
                            OnClick="btnBack_Click" />
                    </div>
                    </span>
                    <span>
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red">        
                    </asp:Label>
                    </span>
                </div>
            </div>
            <div class="GroupBox" style="width: 98%; margin: 10px auto;">
                <rsweb:ReportViewer ID="rvBankLetterCommonRpt" runat="server" Width="100%" Height="100%"
                    OnReportRefresh="rvBankLetterCommonRpt_ReportRefresh">
                </rsweb:ReportViewer>
                <div class="clear">
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="rvBankLetterCommonRpt" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
