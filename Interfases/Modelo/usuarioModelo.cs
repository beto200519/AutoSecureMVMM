using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfases.Modelo
{
    public class usuarioModelo
    {
        public string Id { get; set; }              // Identificador único del usuario
        public string Nombre { get; set; }          // Nombre completo
        public string Correo { get; set; }          // Correo electrónico (usado para login)
        public string Clave { get; set; }           // Contraseña (aunque en producción se recomienda almacenar un hash)
        public int Pin { get; set; }                // Código PIN (si es parte de la seguridad)
        public string Rol { get; set; }             // Rol del usuario (por ejemplo, administrador, usuario, etc.)
        public DateTime? UltimoAcceso { get; set; }
    }
}
