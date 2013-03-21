<%@ Page Title="AlarmWorkflow" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AlarmWorkflow.Website.Asp.Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?sensor=true"> </script> 
    <script type="text/javascript" src="http://www.openlayers.org/api/OpenLayers.js"> </script>
    <script type="text/javascript" src="http://www.openstreetmap.org/openlayers/OpenStreetMap.js"> </script> 
    <script type="text/javascript"><%= JSScripts %></script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <asp:Table ID="OperationTable" runat="server" Width="100%" Height="100%" 
               BorderColor="#333333" BorderStyle="Solid" BorderWidth="3px" 
               GridLines="Both" Font-Size="45px" HorizontalAlign="Center">
        <asp:TableRow  ID="trInformation" runat="server" HorizontalAlign="Center" VerticalAlign="Middle">
            <asp:TableCell ID="tcPicture" runat="server" RowSpan="2">
                <asp:Label ID="lbPicture" Font-Size="55px" Font-Bold="True" runat="server"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="tcOther" runat="server" Width="40%">
                <asp:Label ID="lbOther" runat="server"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        
        <asp:TableRow ID="TableRow1" runat="server" HorizontalAlign="Center" VerticalAlign="Middle"><asp:TableCell runat="server"><asp:Label ID="lbKeyword" runat="server" Font-Bold="True"></asp:Label></asp:TableCell></asp:TableRow>
        <asp:TableRow ID="trLocation" runat="server" HorizontalAlign="Center" VerticalAlign="Middle">
            <asp:TableCell ID="tcAddress" runat="server">
                <asp:Label ID="lbAddress" runat="server"></asp:Label>
            </asp:TableCell>
            <asp:TableCell ID="tcObject" runat="server">
                <asp:Label ID="lbObject" runat="server"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trMap" Height="50%" runat="server" HorizontalAlign="Center" VerticalAlign="Middle">
            <asp:TableCell ID="tcGoogle" runat="server">
                <div id="googlemap"  style="height: 100%; width: 100%;">
                </div>
            </asp:TableCell>
            <asp:TableCell ID="tcOSM" runat="server">
                <div id="osmmap"  style="height: 100%; width: 100%;">
                    <script type="text/javascript"><%= OSMCode %></script>
                </div>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="trResources" runat="server" HorizontalAlign="Center" VerticalAlign="Middle">
            <asp:TableCell ID="tcResources" runat="server" ColumnSpan="2">
                <asp:Label ID="lbResources" runat="server"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
     <asp:Label Font-Size="15px" ID="DebugLabel" runat="server" Text="DebugInformation"/>
    <asp:LinkButton Style="padding-left: 5px" ID="ResetButton" runat="server" 
        onclick="ResetButton_Click">Reset</asp:LinkButton>
   
    <asp:ScriptManager ID="_ScriptManager" runat="server" />
    <asp:Timer runat="server" ID="_UpdateTimer" OnTick="UpdateTimer_Tick" Interval="10000" />
    <asp:UpdatePanel runat="server" ID="_TimedPanel" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="_UpdateTimer" EventName="Tick" />
        </Triggers>
        <ContentTemplate>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>