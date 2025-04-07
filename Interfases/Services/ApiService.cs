using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Interfases.Modelos;

namespace Interfases.Servicios
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> LoginAsync(string correo, string clave)
        {
            var loginData = new LoginModelo
            {
                Correo = correo,
                Clave = clave
            };

            var json = System.Text.Json.JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://ml1ctcld-7149.usw3.devtunnels.ms/api/Usuarios", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    // Aquí puedes deserializar si la API regresa un token u otro objeto
                    // var resultado = JsonSerializer.Deserialize<LoginRespuesta>(responseBody);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Loguear el error
                Console.WriteLine($"Error al conectar: {ex.Message}");
                return false;
            }
        }
    }
}
