<%@ Page Title="AlarmWorkflow" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AlarmWorkflow.Website.Asp._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2 style="text-align: center">
        AlarmWorkflow - Aktueller Alarm
    </h2>
    <p>
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:Timer runat="server" ID="UpdateTimer" Interval="5000" OnTick="UpdateTimer_Tick" />
        <asp:UpdatePanel runat="server" ID="TimedPanel" UpdateMode="Conditional">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
            </Triggers>
            <ContentTemplate>
                <asp:Panel runat="server" Visible="false" ID="pnlNoAlarm">
                    <asp:Label ID="Label1" runat="server" Text="Aktuelle Uhrzeit: " />
                    <asp:Label runat="server" ID="DateStampLabel" />
                    <p />
                    <b>Kein aktueller Alarm!</b>
                </asp:Panel>
                <asp:Panel runat="server" Visible="true" ID="pnlAlarm">
                    <asp:Table runat="server">
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server" Text="Einsatznummer: " />
                            <asp:TableCell runat="server" ID="tcOperationNumber" Text="(Einsatznummer)" />
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server" Text="Zeitstempel: " />
                            <asp:TableCell runat="server" ID="tcTimestamp" Text="(Zeitstempel)" />
                        </asp:TableRow>
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server" Text="Einsatzort: " />
                            <asp:TableCell runat="server" ID="tcDestinationLocation" Text="(Einsatzort)" />
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </p>
</asp:Content>
