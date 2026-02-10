<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Habitaciones.aspx.cs" Inherits="CapaPresentacion.Habitaciones" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Agregar Habitaciones</h2>
            <asp:Label ID="lblnumero" runat="server" Text="Número de habitación"></asp:Label><br />
            <asp:TextBox ID="txtnumero" runat="server"></asp:TextBox><br />
            <asp:Label ID="lbldescripcion" runat="server" Text="Descripción General"></asp:Label><br />
            <asp:TextBox ID="txtdescripcion" runat="server"></asp:TextBox><br />
            <asp:Label ID="lblcant" runat="server" Text="Cantidad de Huespedes permitido"></asp:Label><br />
            <asp:TextBox ID="txtcant" runat="server"></asp:TextBox><br />
            <asp:Button ID="btnguardar" runat="server" Text="Button" OnClick="btnguardar_Click" Height="27px" /><br />
        </div>
        <asp:GridView ID="dgvHabitaciones" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="id_habitacion" HeaderText="ID" />
                <asp:BoundField DataField="numero" HeaderText="#" />
                <asp:BoundField DataField="descripcion" HeaderText="Descipción" />
                <asp:BoundField DataField="cant_huespedes" HeaderText="Max-Personas" />

            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
