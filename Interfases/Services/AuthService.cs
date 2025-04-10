﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Interfases.Modelo;
using Interfases.Modelos;
using Microsoft.Maui.Storage; // Asegúrate de tener la referencia de MAUI Essentials

namespace Interfases.Servicios
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/auth";

        public AuthService()
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

            var json = JsonSerializer.Serialize(loginData);
            Console.WriteLine($"Enviando JSON: {json}");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                //var response = await _httpClient.PostAsync($"{BaseUrl}", content);
                var response = await _httpClient.PostAsync($"{BaseUrl}/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Respuesta cruda del servidor:");
                Console.WriteLine(responseBody);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var loginRespuesta = JsonSerializer.Deserialize<TokenRessponse>(responseBody,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (loginRespuesta != null && !string.IsNullOrEmpty(loginRespuesta.Token))
                        {
                            await SecureStorage.SetAsync("jwt_token", loginRespuesta.Token);
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("No se pudo deserializar correctamente el token.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al deserializar: {ex.Message}");
                    }

                    return false;
                }
                else
                {
                    Console.WriteLine($"Error del servidor: {response.StatusCode}");
                    return false;
                }

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de conexión: {ex.Message}");
                return false;  // Manejo de fallo de red
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;  // Otro tipo de error
            }
        }

        public static async Task<string> GetTokenAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                return token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el token: {ex.Message}");
                return null;
            }
        }

        public static void RemoveToken()
        {
            try
            {
                SecureStorage.Remove("jwt_token");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el token: {ex.Message}");
            }
        }
    }
}
