using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfases.Servicios;
using System.Threading.Tasks;

namespace Interfases.VistaModel
{
    public partial class LoginVM : ObservableObject
    {
        private readonly ApiService _apiService;

        public LoginVM(ApiService apiService)
        {
            _apiService = apiService;
        }

        [ObservableProperty]
        private string correo;

        [ObservableProperty]
        private string clave;

        [ObservableProperty]
        private string errorMessage;

        [RelayCommand]
        public async Task IniciarSesionAsync()
        {
            // Validación de los campos de correo y clave
            if (string.IsNullOrWhiteSpace(Correo))
            {
                ErrorMessage = "Por favor, ingresa tu correo.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Clave))
            {
                ErrorMessage = "Por favor, ingresa tu contraseña.";
                return;
            }

            // Llamada al servicio para verificar el login
            bool loginExitoso = await _apiService.LoginAsync(Correo, Clave);

            if (loginExitoso)
            {
                ErrorMessage = string.Empty;  // Limpiar mensaje de error
                await Shell.Current.GoToAsync("//AcesoRemoto"); // Navegar a la página principal
            }
            else
            {
                ErrorMessage = "Correo o contraseña incorrectos.";
            }
        }
    }
}
