
namespace Interfases.Modelo
{
    public class acesoModel
    {
        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public string Metodo { get; set; }
        public bool Estado { get; set; }
        public string PuertasId { get; set; }
        public string PermisosId { get; set; }
        public string NombreUsuario { get; set; } // Para mostrar el nombre del usuario
        //public string Id { get; set; }
        //public string Usuario_id { get; set; }
        //public string Puesto { get; set; }
        //public string Puertas_id { get; set; }
        //public string Dias { get; set; }
        //public string Horas { get; set; } // Almacena el rango de horas en formato "HH:mm-HH:mm"

        //public string Nombre { get; set; }
        //public string Correo { get; set; }
        //public string Clave { get; set; }
        //public string Rol { get; set; }
        //public int Pin { get; set; }
        //public DateTime UltimoAcceso { get; set; }

        //// Propiedades auxiliares para las horas descompuestas
        //public string HoraInicio
        //{
        //    get => Horas?.Split('-')?.ElementAtOrDefault(0);
        //    set
        //    {
        //        var partes = Horas?.Split('-') ?? new string[2];
        //        Horas = $"{value}-{partes.ElementAtOrDefault(1)}";
        //    }
        //}

        //public string HoraFin
        //{
        //    get => Horas?.Split('-')?.ElementAtOrDefault(1);
        //    set
        //    {
        //        var partes = Horas?.Split('-') ?? new string[2];
        //        Horas = $"{partes.ElementAtOrDefault(0)}-{value}";
        //    }
        //}
    }
}