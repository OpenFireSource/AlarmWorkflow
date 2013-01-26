<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Idle.aspx.cs"  MasterPageFile="~/Site.master" Inherits="AlarmWorkflow.Website.Asp.Idle" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent" >
    
    <div style="font-size: 60px; font-weight: 700; text-align: center; text-decoration: underline; width: 100%;">
        Keine Alarm!
    </div>
     
    <asp:ScriptManager ID="_ScriptManager" runat="server" />
    <asp:Timer runat="server" ID="_UpdateTimer" OnTick="UpdateTimer_Tick" Interval="10000" />
    <asp:UpdatePanel runat="server" ID="_TimedPanel" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="_UpdateTimer" EventName="Tick" />
        </Triggers>
        <ContentTemplate>
            <div style="font-size: 40px; font-weight: 700; text-align: center; width: 100%;">
                <asp:Label ID="LastUpdate" runat="server">
                    Letztes Update:
                </asp:Label>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
        
    
    
</asp:Content>