using System;
using System.Collections.Generic;
using System.Linq;

namespace Interfases.Modelo
{
    public class acesoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public string ColorRol => Rol == "Administrador" ? "#4169E1" : "#228B22";
        public TimeSpan HoraPermitida { get; set; }
        public DateTime DiaPermitido { get; set; }
        public string PuertaPermitida { get; set; }
        public bool CuentaHabilitada { get; set; }
    }
}