<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="command.aspx.cs" Inherits="TLwebgui.command" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <html>
        <form action="command.aspx" method="post">
            <input type="text" name="cmd" />
            <input type="submit" value="Execute..." />
        </form>
    </html>
</asp:Content>
