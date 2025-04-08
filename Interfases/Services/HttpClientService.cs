// HttpClientService.cs
using Interfases.Servicios;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Interfases.Services
{
    public class HttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            var token = await AuthService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                throw new Exception("No se encontró un token de autenticación.");
            }
            return _httpClient;
        }
    }
}