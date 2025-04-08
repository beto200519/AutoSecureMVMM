using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Interfases.Modelo;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using Interfases.Servicios;
using Interfases.Vistas;

namespace Interfases.VistaModel
{
    public class PerfilMV : BindableObject
    {
        private readonly HttpClient _client;

        // Propiedades de Usuarios
        private string _nombre;
        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(); }
        }

        private string _correo;
        public string Correo
        {
            get => _correo;
            set { _correo = value; OnPropertyChanged(); }
        }

        private string _rol;
        public string Rol
        {
            get => _rol;
            set { _rol = value; OnPropertyChanged(); }
        }

        private int _pin;
        public int Pin
        {
            get => _pin;
            set { _pin = value; OnPropertyChanged(); }
        }

        private string _ultimoAcceso;
        public string UltimoAcceso
        {
            get => _ultimoAcceso;
            set { _ultimoAcceso = value; OnPropertyChanged(); }
        }

        // Propiedades de Permisos
        private string _puesto;
        public string Puesto
        {
            get => _puesto;
            set { _puesto = value; OnPropertyChanged(); }
        }

        private string _dias;
        public string Dias
        {
            get => _dias;
            set { _dias = value; OnPropertyChanged(); }
        }

        private string _horas;
        public string Horas
        {
            get => _horas;
            set { _horas = value; OnPropertyChanged(); }
        }

        // Comando para buscar el usuario
        public ICommand BuscarUsuarioCommand { get; }
        public ICommand CerrarSesionCommand { get; }

        public PerfilMV()
        {
            _client = new HttpClient();
            BuscarUsuarioCommand = new Command<string>(async (id) => await CargarUsuario(id));
            CerrarSesionCommand = new Command(CerrarSesion);
        }

        // Método para cargar el perfil de usuario
        private async Task CargarUsuario(string idUsuario)
        {
            try
            {
                // Validar que el ID no esté vacío
                if (string.IsNullOrWhiteSpace(idUsuario))
                {
                    await ShowAlert("Error", "Ingrese un ID válido");
                    return;
                }

                var token = await AuthService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    await ShowAlert("Error", "No se encontró un token de sesión.");
                    return;
                }

                var url = $"https://ml1ctcld-7149.usw3.devtunnels.ms/api/Usuario/Get{idUsuario}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var usuario = JsonSerializer.Deserialize<PerfilModelo>(jsonResponse, options);

                    if (usuario != null)
                    {
                        // Actualizar la UI con los datos obtenidos
                        UpdateUI(usuario);
                        await ShowAlert("Éxito", "Datos cargados correctamente", "OK");
                    }
                    else
                    {
                        await ShowAlert("Error", "Formato de respuesta inválido");
                    }
                }
                else
                {
                    await HandleApiError(response);
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Error crítico", $"Error: {ex.Message}");
            }
        }

        // Método para actualizar los datos del perfil en la UI
        private void UpdateUI(PerfilModelo usuario)
        {
            Nombre = usuario.Nombre;
            Correo = usuario.Correo;
            Rol = usuario.Rol;
            Pin = usuario.Pin;
            UltimoAcceso = usuario.UltimoAcceso;
            Puesto = usuario.Puesto;
            Dias = usuario.Dias;
            Horas = usuario.Horas;
        }

        // Manejo de errores de la API
        private async Task HandleApiError(HttpResponseMessage response)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = $"Error: {response.StatusCode}";

            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage += $"\nDetalles: {errorContent}";
            }

            await ShowAlert("Error del servidor", errorMessage);
        }

        // Mostrar alertas en la UI
        private async Task ShowAlert(string title, string message, string cancel = "OK")
        {
            await Application.Current.MainPage.Dispatcher.DispatchAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            });
        }

        // Cerrar sesión
        private void CerrarSesion()
        {
            AuthService.RemoveToken(); // Eliminar el token
            Application.Current.MainPage = new Login();  // Redirigir a la página de login
        }
    }
}