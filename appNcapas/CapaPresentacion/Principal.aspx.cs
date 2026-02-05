using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CapaPresentacion
{
    public partial class Personal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           if (Session["usuario"]== null)
            {
                Response.Redirect("Default.aspx");
            }else
            {
                lblusuario.Text = Session["usuario"].ToString();
            }
        }

        protected void out_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Default.aspx");
        }

        protected void btnHabitaciones_Click(object sender, EventArgs e)
        {
            Response.Redirect("Habitaciones.aspx");
        }
    }
}