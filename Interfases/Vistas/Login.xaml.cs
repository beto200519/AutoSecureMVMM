using Interfases.VistaModel;

namespace Interfases.Vistas
{
    public partial class Login : ContentPage
    {
        // Constructor sin parámetros
        public Login()
        {
            InitializeComponent();
        }

        // Constructor con ViewModel como parámetro
        public Login(LoginVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;  // Asigna el ViewModel al BindingContext de la página
        }

        // Método de evento para el clic del botón "Entrar"
        private async void EntrarClic(object sender, EventArgs e)
        {
            var viewModel = (LoginVM)BindingContext;

            if (viewModel == null)
            {
                Console.WriteLine("Error: El BindingContext no está asignado correctamente.");
                return;
            }

            // Ejecuta el comando IniciarSesionAsync del ViewModel
            await viewModel.IniciarSesionAsync();

            // Si hay un mensaje de error, lo mostramos en la interfaz de usuario
            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
            {
                await DisplayAlert("Error", viewModel.ErrorMessage, "OK");
            }
        }
    }
}
