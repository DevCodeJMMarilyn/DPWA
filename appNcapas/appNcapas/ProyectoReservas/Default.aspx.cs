using CapaNegocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProyectoReservas
{
    public partial class Default : System.Web.UI.Page
    {
        CNPersonas bll =new CNPersonas();
       
        protected void btnLogin_Click_Click(object sender, EventArgs e)
        {
            bool acceso = bll.Login(txtUsuario.Text, txtClave.Text);

            if (acceso)
            {
                Response.Redirect("Principal.aspx");
            }
            else
            {
                lblMensaje.Text = "Usuario o clave incorrectos";
            }
        }
    }
}