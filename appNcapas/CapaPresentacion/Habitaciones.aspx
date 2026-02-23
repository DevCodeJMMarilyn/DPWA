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
            <h2>Agregar habitaciones</h2>
            <asp:Label ID="lblnumero" runat="server" Text="Numero de habitacion"></asp:Label><br /> 
            <asp:TextBox ID="txtnumero" runat="server"></asp:TextBox><br><br />
            <asp:label ID="lbdescripcion" runat="server" Text="descripcion general"></asp:label><br />
            <asp:TextBox ID="txtdescripcion" runat="server"></asp:TextBox><br><br />
            <asp:label ID="lbcant" runat="server" Text="Cantidad de huespedes permitidos"></asp:label><br />
            <asp:TextBox ID="txtcant" runat="server"></asp:TextBox><br><br />
            <asp:Button ID="btnguardar" runat="server" Text="Guardar" OnClick="btnguardar_Click"></asp:Button>

        </div>
        <hr />
        <div>
        </div>
        <asp:GridView ID="dgvHabitaciones" runat="server" AutoGenerateColumns="false" BackColor="#cc99ff"
            DataKeyNames="id_habitacion" OnRowEditing="dgvHabitaciones_RowEditing"
            DataUpdating="dgvHabitaciones_RowUpdating"
            DataCancelingEdit="dgvHabitaciones_RowCancelingEdit"
            OnRowDeleting="dgvHabitaciones_RowDeleting" OnRowCancelingEdit="dgvHabitaciones_RowCancelingEdit" OnRowUpdating="dgvHabitaciones_RowUpdating" OnSelectedIndexChanged="dgvHabitaciones_SelectedIndexChanged" 
        >

            <Columns>
                <asp:BoundField DataField="id_habitacion" HeaderText="ID" />
                <asp:BoundField DataField="numero" HeaderText="#" />
                <asp:BoundField DataField="descripcion" HeaderText="Descipción" />
                <asp:BoundField DataField="cant_huespedes" HeaderText="Max-Personas" />
                <%-- Botones de editar y eliminar --%>
                <asp:CommandField ShowEditButton="true" EditText="Editar" />
                <asp:CommandField ShowDeleteButton="true" DeleteText="Eliminar" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
