using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfases.Modelo
{
    public class AcesoRemotoModelo
    {
         
        public string UsuarioId { get; set; }
        public string PuertasId { get; set; }
        public string PermisosId { get; set; }
        public DateTime Fecha { get; set; }
        public string Metodo { get; set; }  // Ej: "Remoto"
        public string Estado { get; set; }  // Ej: "Exitoso"
    }
}

