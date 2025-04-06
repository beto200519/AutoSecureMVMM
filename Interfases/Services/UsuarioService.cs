using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Interfases.Modelo;
using Interfases.Modelos;
using Microsoft.Maui.Storage;

namespace Interfases.Servicios
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        // Ajusta la URL base a la de tu API de usuarios
        private const string BaseUrl = "https://qmw9l8hh-5192.usw3.devtunnels.ms/api/Usuario";

        public UsuarioService(string token)
        {
            _httpClient = new HttpClient();
            // Agrega el token JWT en el header Authorization
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Método para obtener todos los usuarios
        public async Task<List<usuarioModelo>> ObtenerUsuariosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/Get");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuarios = JsonSerializer.Deserialize<List<usuarioModelo>>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return usuarios;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
            }
            return null;
        }
    }
}
