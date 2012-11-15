<%@ Page Title="AlarmWorkflow" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AlarmWorkflow.Website.Asp._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:Timer runat="server" ID="UpdateTimer" OnTick="UpdateTimer_Tick" Interval="10000" />
    <asp:UpdatePanel runat="server" ID="TimedPanel" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel runat="server" Visible="true" ID="pnlProgress">
                <asp:Label runat="server" ID="lblProgressText" Text="Verbindung..." />
            </asp:Panel>
            <asp:Panel runat="server" Visible="false" ID="pnlNoAlarm">
                <asp:Label ID="Label1" runat="server" Text="Aktuelle Uhrzeit: " />
                <asp:Label runat="server" ID="DateStampLabel" />
                <p />
                <b>Kein aktueller Alarm!</b>
            </asp:Panel>
            <asp:Panel runat="server" Visible="false" ID="pnlAlarm">
                <asp:Table runat="server">
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server" Text="Einsatznummer: " Font-Bold="true" />
                        <asp:TableCell runat="server" ID="tcOperationNumber" Text="(Einsatznummer)" />
                        <asp:TableCell runat="server" Text="Stichwort: " Font-Bold="true" />
                        <asp:TableCell runat="server" ID="tcKeyword" Text="(Stichwort)" />
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server" Text="Zeitstempel: " Font-Bold="true" />
                        <asp:TableCell runat="server" ID="tcTimestamp" Text="(Zeitstempel)" />
                        <asp:TableCell runat="server" Text="Melder: " Font-Bold="true" />
                        <asp:TableCell runat="server" ID="tcMessenger" Text="(Melder)" />
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server" Text="Einsatzort: " Font-Bold="true" />
                        <asp:TableCell runat="server" ID="tcDestinationLocation" Text="(Einsatzort)" />
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server" Text="Kommentar: " Font-Bold="true" />
                        <asp:TableCell runat="server" ID="tcComment" Text="(Kommentar)" />
                    </asp:TableRow>
                </asp:Table>
                <p />
                <asp:Label runat="server" Text="Anfahrtsplan:" Font-Bold="true" />
                <br />
                <asp:Image ID="imgRouteImage" runat="server" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
