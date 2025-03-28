
namespace Interfases.Modelo
{
    internal class TokenRessponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}
