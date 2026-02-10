using CapaDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    
    public class CNHabitaciones
    {
        CDHabitaciones _habitaciones = new CDHabitaciones();
        public DataTable ObtenerhabitacionesN()
        {
            return _habitaciones.ObtenerHabitaciones();
        }
    

    public bool agregar_habitaciones(int numero, string descripcion, int cantidad)
    {
        return _habitaciones.AgregarHabitacion(numero, descripcion, cantidad);
    }
    }
}