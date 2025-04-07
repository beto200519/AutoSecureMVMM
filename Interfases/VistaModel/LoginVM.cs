using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfases.Servicios;
using Interfases.Modelos;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Interfases.Vistas;

namespace Interfases.VistaModel
{
    public partial class LoginVM : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly INavigation _navigation;

        public LoginVM(AuthService authService, INavigation navigation)
        {
            _authService = authService;
            _navigation = navigation;
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
            Console.WriteLine($"Correo: {Correo}");
            Console.WriteLine($"Clave: {Clave}");
            try
            {
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

                if (!Correo.Contains("@") || !Correo.Contains("."))
                {
                    ErrorMessage = "El formato del correo electrónico no es válido.";
                    return;
                }

                bool loginExitoso = await _authService.LoginAsync(Correo, Clave);

                if (loginExitoso)
                {
                    ErrorMessage = string.Empty;

                    var token = await SecureStorage.GetAsync("jwt_token");

                    if (string.IsNullOrEmpty(token))
                    {
                        ErrorMessage = "Error: no se pudo guardar o recuperar el token de acceso.";
                        return;
                    }

                    // 🧠 Aquí se usa el token para consultar los usuarios
                    var usuarioService = new UsuarioService(token);
                    var usuarios = await usuarioService.ObtenerUsuariosAsync();

                    if (usuarios == null || usuarios.Count == 0)
                    {
                        ErrorMessage = "No se encontraron usuarios en el sistema.";
                        return;
                    }

                    // Buscar usuario que coincida con el correo
                    var usuarioActual = usuarios.FirstOrDefault(u => u.Correo.Equals(Correo, StringComparison.OrdinalIgnoreCase));

                    if (usuarioActual == null)
                    {
                        ErrorMessage = "El usuario no está registrado en el sistema.";
                        return;
                    }

                    // Puedes guardar info adicional aquí si quieres
                    // Ej: await SecureStorage.SetAsync("usuario_nombre", usuarioActual.Nombre);

                    await _navigation.PushAsync(new AppShell());
                }
                else
                {
                    ErrorMessage = "Inicio de sesión fallido. Verifica tus credenciales.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
