using MiAppMaui.Services;
using Interfases.Modelo;


namespace Interfases.ViewModel
{
    public class LoginVM 
    {
        private readonly ApiService _apiService;

        public string Usuario { get; set; }
        public string Contrasena { get; set; }

        public LoginVM()
        {
            _apiService = new ApiService();
        }

        // Método para realizar el login
        public async Task<bool> LoginAsync()
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(Contrasena))
            {
                // Mostrar error de campos vacíos
                return false;
            }

            var loginData = new LoginModelo
            {
                Usuario = Usuario,
                Contrasena = Contrasena
            };

            return await _apiService.LoginAsync(loginData);
        }
    }
}