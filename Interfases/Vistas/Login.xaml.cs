using Interfases.Servicios;
using Interfases.VistaModel;

namespace Interfases.Vistas
{
    public partial class Login : ContentPage
    {
        // Constructor sin parámetros para el preview o inicialización simple
        public Login()
        {
            InitializeComponent();
            BindingContext = new LoginVM(new AuthService());  // Asignar LoginVM directamente si no se pasa ViewModel
        }

        // Constructor con ViewModel como parámetro
        public Login(LoginVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;  // Asignar correctamente el ViewModel que se pasa al constructor
        }
    }
}