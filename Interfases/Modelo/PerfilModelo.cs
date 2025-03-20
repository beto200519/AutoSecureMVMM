using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfases.Modelo
{
    public class PerfilModelo
    {
        public int IdPerfil { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string TipoPerfil { get; set; }
        public string Foto { get; set; }
        public bool EstadoActivo { get; set; }
    }
}