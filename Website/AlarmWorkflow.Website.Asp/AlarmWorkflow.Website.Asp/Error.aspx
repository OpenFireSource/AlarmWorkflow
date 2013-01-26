<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs"  MasterPageFile="~/Site.master" Inherits="AlarmWorkflow.Website.Asp.Error" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent" >
    <div style="background: #ffffff; height: 100%;">
        <div style="color: #FF0000; font-size: 40px; font-weight: 700; text-align: center; text-decoration: underline; width: 100%;">
            Keine Serviceverbindung möglich!
        </div>
     
        <asp:ScriptManager ID="_ScriptManager" runat="server" />
        <asp:Timer runat="server" ID="_UpdateTimer" OnTick="UpdateTimer_Tick" Interval="10000" />
        <asp:UpdatePanel runat="server" ID="_TimedPanel" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="_UpdateTimer" EventName="Tick" />
            </Triggers>
            <ContentTemplate>
                <div style="color: #FF0000; font-size: 40px; font-weight: 700; text-align: center; text-decoration: underline; width: 100%;">
                
                    <asp:Label ID="LastUpdate" runat="server" Text="Letztes Update:" 
                               style="font-size: x-large"></asp:Label>
       
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <p style="color: #000000; font-size: x-large; text-align: center;">
            Bitte starten Sie den Servie selbst neu oder informieren sie den Ihren 
            Systemadministrator.</p>
    </div>
    
</asp:Content>