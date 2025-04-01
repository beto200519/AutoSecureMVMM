using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Interfases.Modelo;

namespace MiAppMaui.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string _baseUrl = "http://172.16.42.88:5000/api"; // Cambia esto a la IP de tu API

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        // Método para obtener datos (GET)
        public async Task<T> GetDataAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GET {endpoint}: {ex.Message}");
                // Aquí puedes agregar un log o un mecanismo para notificar al usuario.
            }

            return default;
        }

        // Método para enviar datos (POST)
        public async Task<bool> PostDataAsync<T>(string endpoint, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en POST {endpoint}: {ex.Message}");
                return false;
            }
        }

        // Método para actualizar datos (PUT)
        public async Task<bool> PutDataAsync<T>(string endpoint, T data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PUT {endpoint}: {ex.Message}");
                return false;
            }
        }

        // Método para eliminar datos (DELETE)
        public async Task<bool> DeleteDataAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}").ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DELETE {endpoint}: {ex.Message}");
                return false;
            }
        }

        // Método para configurar token de autenticación
        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Método para obtener datos del perfil (usando GetDataAsync genérico)
        public async Task<T> GetProfileDataAsync<T>(string endpoint)
        {
            return await GetDataAsync<T>(endpoint);
        }

        internal async Task<bool> LoginAsync(LoginModelo loginData)
        {
            throw new NotImplementedException();
        }
    }
}
