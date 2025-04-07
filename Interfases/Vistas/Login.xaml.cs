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
            BindingContext = new LoginVM(new AuthService(), Navigation);  // Asignar LoginVM directamente si no se pasa ViewModel
        }
    }
}