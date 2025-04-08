using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfases.Modelo;
using Interfases.Modelos;
using Interfases.Services;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Interfases.VistaModel
{
    public partial class AccesoRemotoVM : ObservableObject
    {
        private readonly HttpClientService _httpClientService;
        private readonly string _apiAccesosUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/Accesos";
        private readonly string _apiUsuariosUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/Usuario";
        private readonly string _apiCircuitoUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/Circuito/GetById/67ed8c3d1f95e236d4cff150";

        private bool _puertaAbierta;
        private bool _seguroActivado;
        private int _tiempoRestante;
        private System.Timers.Timer _temporizador;
        private string _correoUsuario; // Correo del usuario autenticado

        public ObservableCollection<acesoModel> Historial { get; set; } = new ObservableCollection<acesoModel>();

        public bool PuertaCerrada => !_puertaAbierta;
        public bool PuertaAbierta => _puertaAbierta;
        public string EstadoPuerta => _puertaAbierta ? "Puerta Abierta" : "Puerta Cerrada";
        public string EstadoSeguro => _seguroActivado ? "Seguro Activado" : "Seguro Desactivado";
        public bool SeguroActivado => _seguroActivado;
        public bool SeguroDesactivado => !_seguroActivado;
        public bool MostrarContador => _tiempoRestante > 0;
        public string Contador => $"{_tiempoRestante}s";

        public AccesoRemotoVM()
        {
            _httpClientService = new HttpClientService();
            _correoUsuario = Preferences.Get("correo", null); // Suponiendo que guardaste el correo en el login
            _ = CargarHistorialAsync();
        }

        [RelayCommand]
        private async Task OnPuertaTappedAsync()
        {
            if (_seguroActivado)
            {
                await Application.Current.MainPage.DisplayAlert("Acceso Denegado", "La puerta tiene el seguro activado.", "OK");
                return;
            }

            HttpClient client = await _httpClientService.GetAuthenticatedClientAsync();

            if (!_puertaAbierta)
            {
                _puertaAbierta = true;
                _tiempoRestante = 10;
                Historial.Insert(0, new acesoModel { Fecha = DateTime.Now, Metodo = "Manual", Estado = true, NombreUsuario = await GetUsuarioNombreAsync() });
                await AbrirCircuitoAsync(client);
                await RegistrarAccesoAsync(client, "Abierto");
                StartTimer();
            }
            else
            {
                _puertaAbierta = false;
                _temporizador?.Stop();
                Historial.Insert(0, new acesoModel { Fecha = DateTime.Now, Metodo = "Manual", Estado = false, NombreUsuario = await GetUsuarioNombreAsync() });
                await CerrarCircuitoAsync(client);
                await RegistrarAccesoAsync(client, "Cerrado");
            }
        }

        [RelayCommand]
        private async Task OnSeguroTappedAsync()
        {
            _seguroActivado = !_seguroActivado;
            Historial.Insert(0, new acesoModel
            {
                Fecha = DateTime.Now,
                Metodo = "Manual",
                Estado = _seguroActivado,
                NombreUsuario = await GetUsuarioNombreAsync()
            });
        }

        private async Task CargarHistorialAsync()
        {
            try
            {
                HttpClient client = await _httpClientService.GetAuthenticatedClientAsync();
                var response = await client.GetAsync(_apiAccesosUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var historialData = JsonConvert.DeserializeObject<ObservableCollection<acesoModel>>(json);
                    if (historialData != null)
                    {
                        Historial.Clear();
                        foreach (var registro in historialData)
                        {
                            registro.NombreUsuario = await GetUsuarioNombreAsync(registro.UsuarioId);
                            Historial.Add(registro);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar historial: {ex.Message}", "OK");
            }
        }

        private async Task RegistrarAccesoAsync(HttpClient client, string estado)
        {
            try
            {
                var usuario = await ObtenerUsuarioActualAsync(client);
                if (usuario == null) return;

                var acceso = new acesoModel
                {
                    UsuarioId = usuario.Id,
                    PuertasId = "puerta_default", // Ajusta según tu lógica
                    PermisosId = "permiso_default", // Ajusta según tu lógica
                    Fecha = DateTime.UtcNow,
                    Metodo = "Manual",
                    Estado = estado == "Abierto"
                };

                var json = JsonConvert.SerializeObject(acceso);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_apiAccesosUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el acceso.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al registrar acceso: {ex.Message}", "OK");
            }
        }

        private async Task<usuarioModelo> ObtenerUsuarioActualAsync(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(_apiUsuariosUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuarios = JsonConvert.DeserializeObject<usuarioModelo[]>(json);
                    return usuarios.FirstOrDefault(u => u.Correo == _correoUsuario);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<string> GetUsuarioNombreAsync(string usuarioId = null)
        {
            var usuario = await ObtenerUsuarioActualAsync(await _httpClientService.GetAuthenticatedClientAsync());
            return usuario?.Nombre ?? "Usuario desconocido";
        }

        private async Task AbrirCircuitoAsync(HttpClient client)
        {
            try
            {
                var circuito = new CircuitoModelo
                {
                    Id = "67ed8c3d1f95e236d4cff150",
                    permisoId = "a",
                    fecha = DateTime.UtcNow
                    estado = true,
                };

                var json = JsonConvert.SerializeObject(circuito);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(_apiCircuitoUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo abrir el circuito.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al abrir circuito: {ex.Message}", "OK");
            }
        }

        private async Task CerrarCircuitoAsync(HttpClient client)
        {
            try
            {
                var circuito = new CircuitoModelo
                {
                    Id = "67ed8c3d1f95e236d4cff150",
                    PermisoId = "a",
                    Fecha = DateTime.UtcNow
                    Estado = false,
                };

                var json = JsonConvert.SerializeObject(circuito);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(_apiCircuitoUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo cerrar el circuito.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cerrar circuito: {ex.Message}", "OK");
            }
        }

        private void StartTimer()
        {
            _temporizador = new System.Timers.Timer(1000); // Intervalo de 1000ms (1 segundo)
            _temporizador.Elapsed += (sender, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    _tiempoRestante--;
                    if (_tiempoRestante == 0)
                    {
                        _puertaAbierta = false;
                        _temporizador.Stop();
                        _temporizador.Dispose(); // Liberar recursos del temporizador
                        _ = CerrarCircuitoAsync(_httpClientService.GetAuthenticatedClientAsync().Result);
                        _ = RegistrarAccesoAsync(_httpClientService.GetAuthenticatedClientAsync().Result, "Cerrado");
                    }
                });
            };
            _temporizador.Start();
        }
    }
}
