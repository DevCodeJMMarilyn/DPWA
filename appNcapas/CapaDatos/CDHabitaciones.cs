using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CDHabitaciones
    {
        public DataTable ObtenerHabitaciones()
        {

            DataTable dt = new DataTable();
            using(SqlConnection con = Conexion.obtenerConexion())
            {
                con.Open();
                using( SqlCommand cmd = new SqlCommand("Select * from habitaciones",
                    con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
                con.Close();
            }
                return dt;
        }

        public bool AgregarHabitacion(int numero, string descripcion, int cantidad)
        {
            using (SqlConnection con = Conexion.obtenerConexion())
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("Insert into habitaciones" +
                    "(numero, descripcion, cant_huespedes) VALUES (@numero, @des, @cant)", con))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    cmd.Parameters.AddWithValue("@des", descripcion);
                    cmd.Parameters.AddWithValue("@cant", cantidad);

                    int filas = cmd.ExecuteNonQuery();

                    con.Close();
                    return filas > 0;
                }

            }
        }
    }
}
