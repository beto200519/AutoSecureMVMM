using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfases.Modelo
{
    class CircuitoModelo
    {
        public string id { get; set; }
        public string permisoId { get; set; }
        public bool estado { get; set; }
        public DateTime fecha { get; set; }
    }
}
