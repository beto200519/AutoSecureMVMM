
using System.Text.Json.Serialization;

namespace Interfases.Modelo
{
    internal class TokenRessponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
