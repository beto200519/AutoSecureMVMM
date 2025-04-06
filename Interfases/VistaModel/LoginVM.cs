using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfases.Servicios;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Interfases.VistaModel
{
    public partial class LoginVM : ObservableObject
    {
        private readonly AuthService _authService;

        public LoginVM(AuthService authService)
        {
            _authService = authService;
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
            try
            {
                // Validar campos vacíos
                if (string.IsNullOrWhiteSpace(Correo))
                {
                    ErrorMessage = "El campo de correo electrónico está vacío.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Clave))
                {
                    ErrorMessage = "El campo de contraseña está vacío.";
                    return;
                }

                // Validar formato de correo
                if (!Correo.Contains("@") || !Correo.Contains("."))
                {
                    ErrorMessage = "El formato del correo electrónico no es válido.";
                    return;
                }

                // Intentar iniciar sesión
                bool loginExitoso = await _authService.LoginAsync(Correo, Clave);

                if (loginExitoso)
                {
                    ErrorMessage = string.Empty; // Limpiar errores previos

                    // Validar si el token se guardó correctamente
                    var tokenGuardado = await SecureStorage.GetAsync("auth_token");

                    if (string.IsNullOrEmpty(tokenGuardado))
                    {
                        ErrorMessage = "Error: no se pudo guardar o recuperar el token de acceso.";
                        return;
                    }

                    // Redirigir al usuario
                    await Shell.Current.GoToAsync("//AcesoRemoto");
                }
                else
                {
                    ErrorMessage = "Inicio de sesión fallido. Verifica tus credenciales.";
                }
            }
            catch (Exception ex)
            {
                // Aquí puedes capturar más excepciones si es necesario
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

    }

}