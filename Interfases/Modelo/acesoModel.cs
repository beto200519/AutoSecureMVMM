
namespace Interfases.Modelo
{
    public class acesoModel
    {
        public string Id { get; set; }
    
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; }
        public int Pin { get; set; }
        public DateTime UltimoAcceso { get; set; }

   
    }
}