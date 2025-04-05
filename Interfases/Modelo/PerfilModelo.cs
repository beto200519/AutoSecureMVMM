namespace Interfases.Modelo
{
    public class PerfilModelo
    {
      
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public int Pin { get; set; }
        public string UltimoAcceso { get; set; }

        // Propiedades de Permisos
        public string Puesto { get; set; }
        public string Dias { get; set; }
        public string Horas { get; set; }
    }
}