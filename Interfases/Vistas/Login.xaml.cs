using Interfases.VistaModel;

namespace Interfases.Vistas
{
    public partial class Login : ContentPage
    {
        // Constructor sin par�metros
        public Login()
        {
            InitializeComponent();
        }

        // Constructor con ViewModel como par�metro
        public Login(LoginVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;  // Asigna el ViewModel al BindingContext de la p�gina
        }

        // M�todo de evento para el clic del bot�n "Entrar"
        private async void EntrarClic(object sender, EventArgs e)
        {
            var viewModel = (LoginVM)BindingContext;

            if (viewModel == null)
            {
                Console.WriteLine("Error: El BindingContext no est� asignado correctamente.");
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
