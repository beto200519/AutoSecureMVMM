using MiAppMaui.Services;
using Interfases.Modelo;
using System.Threading.Tasks;

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

            // Llamar al método de ApiService para hacer el login
            bool isSuccess = await _apiService.LoginAsync(loginData);

            if (!isSuccess)
            {
                // Mostrar error de login fallido
                return false;
            }

            // Si el login es exitoso, puedes almacenar un token o realizar alguna acción
            return true;
        }
    }
}