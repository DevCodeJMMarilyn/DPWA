<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Principal.aspx.cs" Inherits="CapaPresentacion.Personal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Bienvenido</h1> 
            <h3>Usuario:</h3><asp:Label ID="lblusuario" runat="server" Text=""></asp:Label>
        </div>
        <asp:Button ID="btnHabitaciones" runat="server" Text="Habitaciones" OnClick="btnHabitaciones_Click" />
        <asp:Button ID="out" runat="server" Text="Cerrar Sesión" OnClick="out_Click" />
       
    </form>
</body>
</html>
