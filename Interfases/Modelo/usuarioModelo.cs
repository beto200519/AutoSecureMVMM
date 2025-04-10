using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfases.Modelo
{
    public class usuarioModelo
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public int Pin { get; set; }
        public string Rol { get; set; }
        public DateTime UltimoAcceso { get; set; }
    }
}
