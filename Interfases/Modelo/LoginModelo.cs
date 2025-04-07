using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Interfases.Modelos
{
    public class LoginModelo
    {
        [JsonPropertyName("correo")]
        public string Correo { get; set; }    // Correo electrónico del usuario
        [JsonPropertyName("clave")]
        public string Clave { get; set; }
    }
}