using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfases.Modelo
{
    class CircuitoModelo
    {
        public string Id { get; set; }
        public string PermisoId { get; set; }
        public bool Estado { get; set; }
        public DateTime Fecha { get; set; }
    }
}
