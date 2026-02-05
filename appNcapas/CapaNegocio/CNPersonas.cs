using CapaDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CNPersonas
    {
        CDPersonas dal = new CDPersonas();

        public bool Login(string usuario, string clave)
        {
            if (usuario == "" || clave == "")
                return false;

            return dal.ValidarUsuario(usuario, clave);
        }
    }

}
