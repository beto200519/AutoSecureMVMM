using MiAppMaui.Services;  // Asegúrate de incluir el espacio de nombres de ApiService
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Interfases.Vistas;
using Interfases.Modelo;

namespace Interfases.VistaModel
{
    public class PerfilMV : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        // Propiedades para los datos del perfil
        private string _nombreCompleto;
        private string _email;
        private string _tipoPerfil;
        private string _estadoTexto;
        private string _foto;
        private string _fechaFormateada;
        private string _horarioAcceso;
        private string _diasAcceso;
        private string _puertaAcceso;
        private string _estadoColor;

        // Propiedades para binding a la vista
        public string NombreCompleto
        {
            get => _nombreCompleto;
            set { _nombreCompleto = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string TipoPerfil
        {
            get => _tipoPerfil;
            set { _tipoPerfil = value; OnPropertyChanged(); }
        }

        public string EstadoTexto
        {
            get => _estadoTexto;
            set { _estadoTexto = value; OnPropertyChanged(); }
        }

        public string Foto
        {
            get => _foto;
            set { _foto = value; OnPropertyChanged(); }
        }

        public string FechaFormateada
        {
            get => _fechaFormateada;
            set { _fechaFormateada = value; OnPropertyChanged(); }
        }

        public string HorarioAcceso
        {
            get => _horarioAcceso;
            set { _horarioAcceso = value; OnPropertyChanged(); }
        }

        public string DiasAcceso
        {
            get => _diasAcceso;
            set { _diasAcceso = value; OnPropertyChanged(); }
        }

        public string PuertaAcceso
        {
            get => _puertaAcceso;
            set { _puertaAcceso = value; OnPropertyChanged(); }
        }

        public string EstadoColor
        {
            get => _estadoColor;
            set { _estadoColor = value; OnPropertyChanged(); }
        }

        public string MensajeAccion { get; set; }

        // Comando para cerrar sesión
        public ICommand CerrarSesionCommand { get; }

        // Constructor público sin parámetros
        public PerfilMV(ApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

            // Inicialización de los comandos
            CerrarSesionCommand = new Command(async () => await CerrarSesion());

            // Obtener los datos del perfil
            ObtenerPerfil();
        }

        public PerfilMV()
        {
        }

        // Método para obtener los datos del perfil desde la API
        private async Task ObtenerPerfil()
{
    try
    {
        // Asegúrate de especificar el tipo T aquí. Si la respuesta es un objeto perfil, especifica el tipo del perfil.
        var perfil = await _apiService.GetProfileDataAsync<PerfilModelo>("perfil"); // Cambia Perfil por el tipo correcto

        if (perfil != null)
        {
            // Asignar los valores del perfil obtenidos de la API
            NombreCompleto = perfil.NombreCompleto;
            Email = perfil.Email;
            TipoPerfil = perfil.TipoPerfil;
            EstadoTexto = perfil.EstadoTexto;
            Foto = perfil.Foto;
            FechaFormateada = perfil.FechaFormateada;
            HorarioAcceso = perfil.HorarioAcceso;
            DiasAcceso = perfil.DiasAcceso;
            PuertaAcceso = perfil.PuertaAcceso;
            EstadoColor = perfil.EstadoColor;
        }
        else
        {
            MensajeAccion = "No se pudo obtener el perfil.";
        }
    }
    catch (Exception ex)
    {
        MensajeAccion = $"Error al obtener los datos del perfil: {ex.Message}";
    }
}


        // Método para cerrar sesión y redirigir
        private async Task CerrarSesion()
        {
            try
            {
                var success = await _apiService.PostDataAsync<object>("logout", null); // Cambia "logout" a tu endpoint correspondiente

                if (success)
                {
                    MensajeAccion = "Has cerrado sesión correctamente.";
                    RedirigirAPaginaDeLogin();
                }
                else
                {
                    MensajeAccion = "Hubo un problema al cerrar sesión.";
                }
            }
            catch (Exception ex)
            {
                MensajeAccion = $"Error en el cierre de sesión: {ex.Message}";
            }
        }

        // Redirige a la página de login
        private void RedirigirAPaginaDeLogin()
        {
            // Lógica de redirección a la página de login usando navegación
            // Si estás usando Xamarin.Forms o MAUI, puedes hacer algo como esto:
            Application.Current.MainPage = new NavigationPage(new Login());
        }

        // Notificación de cambios
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
