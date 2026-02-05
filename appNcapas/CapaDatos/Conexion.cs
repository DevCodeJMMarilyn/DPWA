using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class Conexion
    {
        private static string cadena =
            "Data Source=DEVELOPER;Initial Catalog=LoginDB;Integrated Security=True";

        public static SqlConnection obtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}
