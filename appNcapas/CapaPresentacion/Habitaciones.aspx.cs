using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion
{
    public partial class Habitaciones : System.Web.UI.Page
    {
        CNHabitaciones _habitaciones = new CNHabitaciones();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarGrid();
            }
        }
        protected void CargarGrid()
        {
            dgvHabitaciones.DataSource = _habitaciones.ObtenerhabitacionesN();
            dgvHabitaciones.DataBind();
        }

        protected void btnguardar_Click(object sender, EventArgs e)
        {
            int numero = Convert.ToInt32(txtnumero.Text);
            string descripcion = txtdescripcion.Text;
            int cant = Convert.ToInt32(txtcant.Text);

            bool correcto = _habitaciones.agregar_habitaciones(numero, descripcion, cant);
            if (correcto){
                Response.Write("<script>alert('Habitacion agregada');</script>");
                CargarGrid();
            }else{
                Response.Write("<script>alert('Error al insertar ');</script>");
            }
        }
    }
}