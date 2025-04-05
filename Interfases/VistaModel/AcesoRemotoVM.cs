using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Interfases.Modelo;
namespace Interfases.VistaModel
{
    public class AcesoRemotoVM : INotifyPropertyChanged
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string apiAccesosUrl = "https://qmw9l8hh-5192.usw3.devtunnels.ms/api/Accesos"; // URL para la API de accesos
        private readonly string apiUsuariosUrl = "https://qmw9l8hh-5192.usw3.devtunnels.ms/api/Usuarios"; // URL para obtener datos del usuario

        private bool puertaAbierta;
        private bool seguroActivado;
        private int tiempoRestante;
        private System.Timers.Timer temporizador;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Historial { get; set; } = new ObservableCollection<string>();

        // Propiedades existentes
        public bool PuertaCerrada => !puertaAbierta;
        public bool PuertaAbierta => puertaAbierta;
        public string EstadoPuerta => puertaAbierta ? "Puerta Abierta" : "Puerta Cerrada";
        public string EstadoSeguro => seguroActivado ? "Seguro Activado" : "Seguro Desactivado";
        public bool SeguroActivado => seguroActivado;
        public bool SeguroDesactivado => !seguroActivado;
        public bool MostrarContador => tiempoRestante > 0;
        public string Contador => $"{tiempoRestante}s";

        public ICommand PuertaTappedCommand { get; }
        public ICommand SeguroTappedCommand { get; }

        // Constructor
        public AcesoRemotoVM()
        {
            PuertaTappedCommand = new Command(async () => await OnPuertaTapped());
            SeguroTappedCommand = new Command(OnSeguroTapped);

            _ = CargarHistorial(); // Carga inicial del historial
        }

        private async Task OnPuertaTapped()
        {
            if (seguroActivado)
            {
                await Application.Current.MainPage.DisplayAlert("Acceso Denegado", "La puerta tiene el seguro activado.", "OK");
                return;
            }

            if (!puertaAbierta)
            {
                puertaAbierta = true;
                tiempoRestante = 10;
                Historial.Add($"Puerta abierta a las {DateTime.Now.ToShortTimeString()}");
                await RegistrarAccesoAsync("Abierto");
                StartTimer();
            }
            else
            {
                puertaAbierta = false;
                temporizador?.Stop();
                Historial.Add($"Puerta cerrada manualmente a las {DateTime.Now.ToShortTimeString()}");
                await RegistrarAccesoAsync("Cerrado");
            }

            NotifyPropertyChanged(nameof(PuertaAbierta));
            NotifyPropertyChanged(nameof(PuertaCerrada));
            NotifyPropertyChanged(nameof(EstadoPuerta));
            NotifyPropertyChanged(nameof(MostrarContador));
        }

        private void OnSeguroTapped()
        {
            seguroActivado = !seguroActivado;
            Historial.Add($"{(seguroActivado ? "Seguro activado" : "Seguro desactivado")} a las {DateTime.Now.ToShortTimeString()}");
            NotifyPropertyChanged(nameof(SeguroActivado));
            NotifyPropertyChanged(nameof(SeguroDesactivado));
            NotifyPropertyChanged(nameof(EstadoSeguro));
        }

        private async Task CargarHistorial()
        {
            try
            {
                var response = await _httpClient.GetAsync(apiAccesosUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var historialData = JsonConvert.DeserializeObject<ObservableCollection<string>>(json);

                    if (historialData != null)
                    {
                        Historial.Clear();
                        foreach (var registro in historialData)
                        {
                            Historial.Add(registro);
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo cargar el historial desde la base de datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error de conexión: {ex.Message}", "OK");
            }
        }

        private async Task RegistrarAccesoAsync(string estado)
        {
            try
            {
                // Aquí deberías obtener dinámicamente el `Usuario_id` y `Puertas_id` desde la API o el contexto actual
                var usuarioActual = await ObtenerUsuarioActualAsync();
                if (usuarioActual == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo obtener el usuario actual.", "OK");
                    return;
                }

                var acceso = new
                {
                    Usuario_id = usuarioActual.Id, // ID obtenido de la API
                    Puertas_id = usuarioActual.PuertaId, // Puerta asignada al usuario
                    Fecha = DateTime.Now,
                    Metodo = "Manual",
                    Estado = estado
                };

                var json = JsonConvert.SerializeObject(acceso);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiAccesosUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el acceso en la base de datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al registrar acceso: {ex.Message}", "OK");
            }
        }

        private async Task<dynamic> ObtenerUsuarioActualAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{apiUsuariosUrl}/actual"); // Endpoint para obtener datos del usuario actual
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<dynamic>(json); // Deserializa los datos del usuario actual
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private void StartTimer()
        {
            temporizador = new System.Timers.Timer(1000);
            temporizador.Elapsed += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    tiempoRestante--;
                    NotifyPropertyChanged(nameof(Contador));

                    if (tiempoRestante == 0)
                    {
                        puertaAbierta = false;
                        NotifyPropertyChanged(nameof(PuertaAbierta));
                        NotifyPropertyChanged(nameof(PuertaCerrada));
                        NotifyPropertyChanged(nameof(EstadoPuerta));
                        NotifyPropertyChanged(nameof(MostrarContador));
                        temporizador.Stop();
                    }
                });
            };
            temporizador.Start();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
