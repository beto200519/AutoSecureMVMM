using System.Text.Json;
using System.Windows.Input;
using Interfases.Modelo;

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

        public PerfilMV()
        {
            _client = new HttpClient();
            BuscarUsuarioCommand = new Command<string>(async (id) => await CargarUsuario(id));
        }

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

                var url = $"https://qmw9l8hh-5192.usw3.devtunnels.ms/api/Usuario/ActUsu&Perm{idUsuario}";
                var response = await _client.GetAsync(url);

                // Comprobar si la respuesta es exitosa
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true
                    };

                    // Deserializar la respuesta en el modelo de usuario
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
                    // Manejar errores de la API
                    await HandleApiError(response);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones generales
                await ShowAlert("Error crítico", $"Error: {ex.Message}");
            }
        }

        private void UpdateUI(PerfilModelo usuario)
        {
            // Actualizar las propiedades del ViewModel con los datos del usuario
            Nombre = usuario.Nombre;
            Correo = usuario.Correo;
            Rol = usuario.Rol;
            Pin = usuario.Pin;
            UltimoAcceso = usuario.UltimoAcceso;
            Puesto = usuario.Puesto;
            Dias = usuario.Dias;
            Horas = usuario.Horas;
        }

        private async Task HandleApiError(HttpResponseMessage response)
        {
            // Obtener detalles del error si la respuesta no es exitosa
            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = $"Error: {response.StatusCode}";

            if (!string.IsNullOrEmpty(errorContent))
            {
                errorMessage += $"\nDetalles: {errorContent}";
            }

            await ShowAlert("Error del servidor", errorMessage);
        }

        private async Task ShowAlert(string title, string message, string cancel = "OK")
        {
            // Mostrar un alert en la UI
            await Application.Current.MainPage.Dispatcher.DispatchAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            });
        }
    }
}
